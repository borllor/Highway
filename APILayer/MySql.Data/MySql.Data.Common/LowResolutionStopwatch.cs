using System;

namespace MySql.Data.Common
{
	internal class LowResolutionStopwatch
	{
		private long millis;

		private long startTime;

		public static readonly long Frequency = 1000L;

		public static readonly bool isHighResolution = false;

		public long ElapsedMilliseconds
		{
			get
			{
				return this.millis;
			}
		}

		public TimeSpan Elapsed
		{
			get
			{
				return new TimeSpan(0, 0, 0, 0, (int)this.millis);
			}
		}

		public LowResolutionStopwatch()
		{
			this.millis = 0L;
		}

		public void Start()
		{
			this.startTime = (long)Environment.TickCount;
		}

		public void Stop()
		{
			long num = (long)Environment.TickCount;
			long num2 = (num < this.startTime) ? (2147483647L - this.startTime + num) : (num - this.startTime);
			this.millis += num2;
		}

		public void Reset()
		{
			this.millis = 0L;
			this.startTime = 0L;
		}

		public static LowResolutionStopwatch StartNew()
		{
			LowResolutionStopwatch lowResolutionStopwatch = new LowResolutionStopwatch();
			lowResolutionStopwatch.Start();
			return lowResolutionStopwatch;
		}

		public static long GetTimestamp()
		{
			return (long)Environment.TickCount;
		}

		private bool IsRunning()
		{
			return this.startTime != 0L;
		}
	}
}
