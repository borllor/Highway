using System;

namespace MySql.Data.MySqlClient.Authentication
{
	public enum SecBufferType
	{
		SECBUFFER_VERSION,
		SECBUFFER_EMPTY = 0,
		SECBUFFER_DATA,
		SECBUFFER_TOKEN
	}
}
