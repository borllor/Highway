using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace MySql.Data.MySqlClient
{
	[Serializable]
	public sealed class MySqlException : DbException
	{
		private int errorCode;

		private bool isFatal;

		public int Number
		{
			get
			{
				return this.errorCode;
			}
		}

		internal bool IsFatal
		{
			get
			{
				return this.isFatal;
			}
		}

		internal bool IsQueryAborted
		{
			get
			{
				return this.errorCode == 1317 || this.errorCode == 1028;
			}
		}

		internal MySqlException()
		{
		}

		internal MySqlException(string msg) : base(msg)
		{
		}

		internal MySqlException(string msg, Exception ex) : base(msg, ex)
		{
		}

		internal MySqlException(string msg, bool isFatal, Exception inner) : base(msg, inner)
		{
			this.isFatal = isFatal;
		}

		internal MySqlException(string msg, int errno, Exception inner) : this(msg, inner)
		{
			this.errorCode = errno;
			this.Data.Add("Server Error Code", errno);
		}

		internal MySqlException(string msg, int errno) : this(msg, errno, null)
		{
		}

		private MySqlException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
