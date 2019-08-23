using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace MySql.Data.MySqlClient
{
	internal sealed class MySqlPool
	{
		private List<Driver> inUsePool;

		private Queue<Driver> idlePool;

		private MySqlConnectionStringBuilder settings;

		private uint minSize;

		private uint maxSize;

		private ProcedureCache procedureCache;

		private bool beingCleared;

		private int available;

		private AutoResetEvent autoEvent;

		public MySqlConnectionStringBuilder Settings
		{
			get
			{
				return this.settings;
			}
			set
			{
				this.settings = value;
			}
		}

		public ProcedureCache ProcedureCache
		{
			get
			{
				return this.procedureCache;
			}
		}

		private bool HasIdleConnections
		{
			get
			{
				return this.idlePool.Count > 0;
			}
		}

		private int NumConnections
		{
			get
			{
				return this.idlePool.Count + this.inUsePool.Count;
			}
		}

		public bool BeingCleared
		{
			get
			{
				return this.beingCleared;
			}
		}

		internal Dictionary<string, string> ServerProperties
		{
			get;
			set;
		}

		private void EnqueueIdle(Driver driver)
		{
			driver.IdleSince = DateTime.Now;
			this.idlePool.Enqueue(driver);
		}

		public MySqlPool(MySqlConnectionStringBuilder settings)
		{
			this.minSize = settings.MinimumPoolSize;
			this.maxSize = settings.MaximumPoolSize;
			this.available = (int)this.maxSize;
			this.autoEvent = new AutoResetEvent(false);
			if (this.minSize > this.maxSize)
			{
				this.minSize = this.maxSize;
			}
			this.settings = settings;
			this.inUsePool = new List<Driver>((int)this.maxSize);
			this.idlePool = new Queue<Driver>((int)this.maxSize);
			int num = 0;
			while ((long)num < (long)((ulong)this.minSize))
			{
				this.EnqueueIdle(this.CreateNewPooledConnection());
				num++;
			}
			this.procedureCache = new ProcedureCache((int)settings.ProcedureCacheSize);
		}

		private Driver GetPooledConnection()
		{
			Driver driver = null;
			lock (((ICollection)this.idlePool).SyncRoot)
			{
				if (this.HasIdleConnections)
				{
					driver = this.idlePool.Dequeue();
				}
			}
			if (driver != null)
			{
				try
				{
					driver.ResetTimeout((int)(this.Settings.ConnectionTimeout * 1000u));
				}
				catch (Exception)
				{
					driver.Close();
					driver = null;
				}
			}
			if (driver != null)
			{
				if (!driver.Ping())
				{
					driver.Close();
					driver = null;
				}
				else if (this.settings.ConnectionReset)
				{
					driver.Reset();
				}
			}
			if (driver == null)
			{
				driver = this.CreateNewPooledConnection();
			}
			lock (((ICollection)this.inUsePool).SyncRoot)
			{
				this.inUsePool.Add(driver);
			}
			return driver;
		}

		private Driver CreateNewPooledConnection()
		{
			Driver driver = Driver.Create(this.settings);
			driver.Pool = this;
			return driver;
		}

		public void ReleaseConnection(Driver driver)
		{
			lock (((ICollection)this.inUsePool).SyncRoot)
			{
				if (this.inUsePool.Contains(driver))
				{
					this.inUsePool.Remove(driver);
				}
			}
			if (driver.ConnectionLifetimeExpired() || this.beingCleared)
			{
				driver.Close();
			}
			else
			{
				lock (((ICollection)this.idlePool).SyncRoot)
				{
					this.EnqueueIdle(driver);
				}
			}
			Interlocked.Increment(ref this.available);
			this.autoEvent.Set();
		}

		public void RemoveConnection(Driver driver)
		{
			lock (((ICollection)this.inUsePool).SyncRoot)
			{
				if (this.inUsePool.Contains(driver))
				{
					this.inUsePool.Remove(driver);
					Interlocked.Increment(ref this.available);
					this.autoEvent.Set();
				}
			}
			if (this.beingCleared && this.NumConnections == 0)
			{
				MySqlPoolManager.RemoveClearedPool(this);
			}
		}

		private Driver TryToGetDriver()
		{
			int num = Interlocked.Decrement(ref this.available);
			if (num < 0)
			{
				Interlocked.Increment(ref this.available);
				return null;
			}
			Driver result;
			try
			{
				Driver pooledConnection = this.GetPooledConnection();
				result = pooledConnection;
			}
			catch (Exception ex)
			{
				MySqlTrace.LogError(-1, ex.Message);
				Interlocked.Increment(ref this.available);
				throw;
			}
			return result;
		}

		public Driver GetConnection()
		{
			int num = (int)(this.settings.ConnectionTimeout * 1000u);
			int i = num;
			DateTime now = DateTime.Now;
			while (i > 0)
			{
				Driver driver = this.TryToGetDriver();
				if (driver != null)
				{
					return driver;
				}
				if (!this.autoEvent.WaitOne(i, false))
				{
					break;
				}
				i = num - (int)DateTime.Now.Subtract(now).TotalMilliseconds;
			}
			throw new MySqlException(Resources.TimeoutGettingConnection);
		}

		internal void Clear()
		{
			lock (((ICollection)this.idlePool).SyncRoot)
			{
				this.beingCleared = true;
				while (this.idlePool.Count > 0)
				{
					Driver driver = this.idlePool.Dequeue();
					driver.Close();
				}
			}
		}

		internal List<Driver> RemoveOldIdleConnections()
		{
			List<Driver> list = new List<Driver>();
			DateTime now = DateTime.Now;
			lock (((ICollection)this.idlePool).SyncRoot)
			{
				while ((long)this.idlePool.Count > (long)((ulong)this.minSize))
				{
					Driver driver = this.idlePool.Peek();
					if (driver.IdleSince.Add(new TimeSpan(0, 0, MySqlPoolManager.maxConnectionIdleTime)).CompareTo(now) >= 0)
					{
						break;
					}
					list.Add(driver);
					this.idlePool.Dequeue();
				}
			}
			return list;
		}
	}
}
