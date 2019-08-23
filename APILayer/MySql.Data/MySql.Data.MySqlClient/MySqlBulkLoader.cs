using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MySql.Data.MySqlClient
{
	public class MySqlBulkLoader
	{
		private const string defaultFieldTerminator = "\t";

		private const string defaultLineTerminator = "\n";

		private const char defaultEscapeCharacter = '\\';

		private string fieldTerminator;

		private string lineTerminator;

		private string charSet;

		private string tableName;

		private int numLinesToIgnore;

		private MySqlConnection connection;

		private string filename;

		private int timeout;

		private bool local;

		private string linePrefix;

		private char fieldQuotationCharacter;

		private bool fieldQuotationOptional;

		private char escapeChar;

		private MySqlBulkLoaderPriority priority;

		private MySqlBulkLoaderConflictOption conflictOption;

		private List<string> columns;

		private List<string> expressions;

		public MySqlConnection Connection
		{
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		public string FieldTerminator
		{
			get
			{
				return this.fieldTerminator;
			}
			set
			{
				this.fieldTerminator = value;
			}
		}

		public string LineTerminator
		{
			get
			{
				return this.lineTerminator;
			}
			set
			{
				this.lineTerminator = value;
			}
		}

		public string TableName
		{
			get
			{
				return this.tableName;
			}
			set
			{
				this.tableName = value;
			}
		}

		public string CharacterSet
		{
			get
			{
				return this.charSet;
			}
			set
			{
				this.charSet = value;
			}
		}

		public string FileName
		{
			get
			{
				return this.filename;
			}
			set
			{
				this.filename = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		public bool Local
		{
			get
			{
				return this.local;
			}
			set
			{
				this.local = value;
			}
		}

		public int NumberOfLinesToSkip
		{
			get
			{
				return this.numLinesToIgnore;
			}
			set
			{
				this.numLinesToIgnore = value;
			}
		}

		public string LinePrefix
		{
			get
			{
				return this.linePrefix;
			}
			set
			{
				this.linePrefix = value;
			}
		}

		public char FieldQuotationCharacter
		{
			get
			{
				return this.fieldQuotationCharacter;
			}
			set
			{
				this.fieldQuotationCharacter = value;
			}
		}

		public bool FieldQuotationOptional
		{
			get
			{
				return this.fieldQuotationOptional;
			}
			set
			{
				this.fieldQuotationOptional = value;
			}
		}

		public char EscapeCharacter
		{
			get
			{
				return this.escapeChar;
			}
			set
			{
				this.escapeChar = value;
			}
		}

		public MySqlBulkLoaderConflictOption ConflictOption
		{
			get
			{
				return this.conflictOption;
			}
			set
			{
				this.conflictOption = value;
			}
		}

		public MySqlBulkLoaderPriority Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}

		public List<string> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public List<string> Expressions
		{
			get
			{
				return this.expressions;
			}
		}

		public MySqlBulkLoader(MySqlConnection connection)
		{
			this.Connection = connection;
			this.Local = true;
			this.FieldTerminator = "\t";
			this.LineTerminator = "\n";
			this.FieldQuotationCharacter = '\0';
			this.ConflictOption = MySqlBulkLoaderConflictOption.None;
			this.columns = new List<string>();
			this.expressions = new List<string>();
		}

		public int Load()
		{
			bool flag = false;
			if (this.Connection == null)
			{
				throw new InvalidOperationException(Resources.ConnectionNotSet);
			}
			if (this.connection.State != ConnectionState.Open)
			{
				flag = true;
				this.connection.Open();
			}
			int result;
			try
			{
				string cmdText = this.BuildSqlCommand();
				result = new MySqlCommand(cmdText, this.Connection)
				{
					CommandTimeout = this.Timeout
				}.ExecuteNonQuery();
			}
			finally
			{
				if (flag)
				{
					this.connection.Close();
				}
			}
			return result;
		}

		private string BuildSqlCommand()
		{
			StringBuilder stringBuilder = new StringBuilder("LOAD DATA ");
			if (this.Priority == MySqlBulkLoaderPriority.Low)
			{
				stringBuilder.Append("LOW_PRIORITY ");
			}
			else if (this.Priority == MySqlBulkLoaderPriority.Concurrent)
			{
				stringBuilder.Append("CONCURRENT ");
			}
			if (this.Local)
			{
				stringBuilder.Append("LOCAL ");
			}
			stringBuilder.Append("INFILE ");
			if (Platform.DirectorySeparatorChar == '\\')
			{
				stringBuilder.AppendFormat("'{0}' ", this.FileName.Replace("\\", "\\\\"));
			}
			else
			{
				stringBuilder.AppendFormat("'{0}' ", this.FileName);
			}
			if (this.ConflictOption == MySqlBulkLoaderConflictOption.Ignore)
			{
				stringBuilder.Append("IGNORE ");
			}
			else if (this.ConflictOption == MySqlBulkLoaderConflictOption.Replace)
			{
				stringBuilder.Append("REPLACE ");
			}
			stringBuilder.AppendFormat("INTO TABLE {0} ", this.TableName);
			if (this.CharacterSet != null)
			{
				stringBuilder.AppendFormat("CHARACTER SET {0} ", this.CharacterSet);
			}
			StringBuilder stringBuilder2 = new StringBuilder(string.Empty);
			if (this.FieldTerminator != "\t")
			{
				stringBuilder2.AppendFormat("TERMINATED BY '{0}' ", this.FieldTerminator);
			}
			if (this.FieldQuotationCharacter != '\0')
			{
				stringBuilder2.AppendFormat("{0} ENCLOSED BY '{1}' ", this.FieldQuotationOptional ? "OPTIONALLY" : "", this.FieldQuotationCharacter);
			}
			if (this.EscapeCharacter != '\\' && this.EscapeCharacter != '\0')
			{
				stringBuilder2.AppendFormat("ESCAPED BY '{0}' ", this.EscapeCharacter);
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder.AppendFormat("FIELDS {0}", stringBuilder2.ToString());
			}
			stringBuilder2 = new StringBuilder(string.Empty);
			if (this.LinePrefix != null && this.LinePrefix.Length > 0)
			{
				stringBuilder2.AppendFormat("STARTING BY '{0}' ", this.LinePrefix);
			}
			if (this.LineTerminator != "\n")
			{
				stringBuilder2.AppendFormat("TERMINATED BY '{0}' ", this.LineTerminator);
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder.AppendFormat("LINES {0}", stringBuilder2.ToString());
			}
			if (this.NumberOfLinesToSkip > 0)
			{
				stringBuilder.AppendFormat("IGNORE {0} LINES ", this.NumberOfLinesToSkip);
			}
			if (this.Columns.Count > 0)
			{
				stringBuilder.Append("(");
				stringBuilder.Append(this.Columns[0]);
				for (int i = 1; i < this.Columns.Count; i++)
				{
					stringBuilder.AppendFormat(",{0}", this.Columns[i]);
				}
				stringBuilder.Append(") ");
			}
			if (this.Expressions.Count > 0)
			{
				stringBuilder.Append("SET ");
				stringBuilder.Append(this.Expressions[0]);
				for (int j = 1; j < this.Expressions.Count; j++)
				{
					stringBuilder.AppendFormat(",{0}", this.Expressions[j]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
