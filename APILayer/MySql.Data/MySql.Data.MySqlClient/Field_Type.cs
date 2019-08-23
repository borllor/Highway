using System;

namespace MySql.Data.MySqlClient
{
	internal enum Field_Type : byte
	{
		DECIMAL,
		BYTE,
		SHORT,
		LONG,
		FLOAT,
		DOUBLE,
		NULL,
		TIMESTAMP,
		LONGLONG,
		INT24,
		DATE,
		TIME,
		DATETIME,
		YEAR,
		NEWDATE,
		ENUM = 247,
		SET,
		TINY_BLOB,
		MEDIUM_BLOB,
		LONG_BLOB,
		BLOB,
		VAR_STRING,
		STRING
	}
}
