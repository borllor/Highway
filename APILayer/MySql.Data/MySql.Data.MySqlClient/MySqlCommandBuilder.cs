using MySql.Data.MySqlClient.Properties;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace MySql.Data.MySqlClient
{
	[DesignerCategory("Code"), ToolboxItem(false)]
	public sealed class MySqlCommandBuilder : DbCommandBuilder
	{
		public new MySqlDataAdapter DataAdapter
		{
			get
			{
				return (MySqlDataAdapter)base.DataAdapter;
			}
			set
			{
				base.DataAdapter = value;
			}
		}

		public MySqlCommandBuilder()
		{
			this.QuotePrefix = (this.QuoteSuffix = "`");
		}

		public MySqlCommandBuilder(MySqlDataAdapter adapter) : this()
		{
			this.DataAdapter = adapter;
		}

		public static void DeriveParameters(MySqlCommand command)
		{
			if (command.CommandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException(Resources.CanNotDeriveParametersForTextCommands);
			}
			string text = command.CommandText;
			if (text.IndexOf(".") == -1)
			{
				text = command.Connection.Database + "." + text;
			}
			try
			{
				ProcedureCacheEntry procedure = command.Connection.ProcedureCache.GetProcedure(command.Connection, text, null);
				command.Parameters.Clear();
				foreach (MySqlSchemaRow current in procedure.parameters.Rows)
				{
					MySqlParameter mySqlParameter = new MySqlParameter();
					mySqlParameter.ParameterName = string.Format("@{0}", current["PARAMETER_NAME"]);
					if (current["ORDINAL_POSITION"].Equals(0) && mySqlParameter.ParameterName == "@")
					{
						mySqlParameter.ParameterName = "@RETURN_VALUE";
					}
					mySqlParameter.Direction = MySqlCommandBuilder.GetDirection(current);
					bool unsigned = StoredProcedure.GetFlags(current["DTD_IDENTIFIER"].ToString()).IndexOf("UNSIGNED") != -1;
					bool realAsFloat = procedure.procedure.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1;
					mySqlParameter.MySqlDbType = MetaData.NameToType(current["DATA_TYPE"].ToString(), unsigned, realAsFloat, command.Connection);
					if (current["CHARACTER_MAXIMUM_LENGTH"] != null)
					{
						mySqlParameter.Size = (int)current["CHARACTER_MAXIMUM_LENGTH"];
					}
					if (current["NUMERIC_PRECISION"] != null)
					{
						mySqlParameter.Precision = Convert.ToByte(current["NUMERIC_PRECISION"]);
					}
					if (current["NUMERIC_SCALE"] != null)
					{
						mySqlParameter.Scale = Convert.ToByte(current["NUMERIC_SCALE"]);
					}
					if (mySqlParameter.MySqlDbType == MySqlDbType.Set || mySqlParameter.MySqlDbType == MySqlDbType.Enum)
					{
						mySqlParameter.PossibleValues = MySqlCommandBuilder.GetPossibleValues(current);
					}
					command.Parameters.Add(mySqlParameter);
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new MySqlException(Resources.UnableToDeriveParameters, ex);
			}
		}

		private static List<string> GetPossibleValues(MySqlSchemaRow row)
		{
			string[] array = new string[]
			{
				"ENUM",
				"SET"
			};
			string text = row["DTD_IDENTIFIER"].ToString().Trim();
			int num = 0;
			while (num < 2 && !text.StartsWith(array[num], StringComparison.OrdinalIgnoreCase))
			{
				num++;
			}
			if (num == 2)
			{
				return null;
			}
			text = text.Substring(array[num].Length).Trim();
			text = text.Trim(new char[]
			{
				'(',
				')'
			}).Trim();
			List<string> list = new List<string>();
			MySqlTokenizer mySqlTokenizer = new MySqlTokenizer(text);
			string text2 = mySqlTokenizer.NextToken();
			int num2 = mySqlTokenizer.StartIndex;
			while (true)
			{
				if (text2 == null || text2 == ",")
				{
					int num3 = text.Length - 1;
					if (text2 == ",")
					{
						num3 = mySqlTokenizer.StartIndex;
					}
					string item = text.Substring(num2, num3 - num2).Trim(new char[]
					{
						'\'',
						'"'
					}).Trim();
					list.Add(item);
					num2 = mySqlTokenizer.StopIndex;
				}
				if (text2 == null)
				{
					break;
				}
				text2 = mySqlTokenizer.NextToken();
			}
			return list;
		}

		private static ParameterDirection GetDirection(MySqlSchemaRow row)
		{
			string a = row["PARAMETER_MODE"].ToString();
			if (Convert.ToInt32(row["ORDINAL_POSITION"]) == 0)
			{
				return ParameterDirection.ReturnValue;
			}
			if (a == "IN")
			{
				return ParameterDirection.Input;
			}
			if (a == "OUT")
			{
				return ParameterDirection.Output;
			}
			return ParameterDirection.InputOutput;
		}

		public new MySqlCommand GetDeleteCommand()
		{
			return (MySqlCommand)base.GetDeleteCommand();
		}

		public new MySqlCommand GetUpdateCommand()
		{
			return (MySqlCommand)base.GetUpdateCommand();
		}

		public new MySqlCommand GetInsertCommand()
		{
			return (MySqlCommand)base.GetInsertCommand(false);
		}

		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			if (unquotedIdentifier == null)
			{
				throw new ArgumentNullException("unquotedIdentifier");
			}
			if (unquotedIdentifier.StartsWith(this.QuotePrefix) && unquotedIdentifier.EndsWith(this.QuoteSuffix))
			{
				return unquotedIdentifier;
			}
			unquotedIdentifier = unquotedIdentifier.Replace(this.QuotePrefix, this.QuotePrefix + this.QuotePrefix);
			return string.Format("{0}{1}{2}", this.QuotePrefix, unquotedIdentifier, this.QuoteSuffix);
		}

		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			if (quotedIdentifier == null)
			{
				throw new ArgumentNullException("quotedIdentifier");
			}
			if (!quotedIdentifier.StartsWith(this.QuotePrefix) || !quotedIdentifier.EndsWith(this.QuoteSuffix))
			{
				return quotedIdentifier;
			}
			if (quotedIdentifier.StartsWith(this.QuotePrefix))
			{
				quotedIdentifier = quotedIdentifier.Substring(1);
			}
			if (quotedIdentifier.EndsWith(this.QuoteSuffix))
			{
				quotedIdentifier = quotedIdentifier.Substring(0, quotedIdentifier.Length - 1);
			}
			quotedIdentifier = quotedIdentifier.Replace(this.QuotePrefix + this.QuotePrefix, this.QuotePrefix);
			return quotedIdentifier;
		}

		protected override DataTable GetSchemaTable(DbCommand sourceCommand)
		{
			DataTable schemaTable = base.GetSchemaTable(sourceCommand);
			foreach (DataRow dataRow in schemaTable.Rows)
			{
				if (dataRow["BaseSchemaName"].Equals(sourceCommand.Connection.Database))
				{
					dataRow["BaseSchemaName"] = null;
				}
			}
			return schemaTable;
		}

		protected override string GetParameterName(string parameterName)
		{
			StringBuilder stringBuilder = new StringBuilder(parameterName);
			stringBuilder.Replace(" ", "");
			stringBuilder.Replace("/", "_per_");
			stringBuilder.Replace("-", "_");
			stringBuilder.Replace(")", "_cb_");
			stringBuilder.Replace("(", "_ob_");
			stringBuilder.Replace("%", "_pct_");
			stringBuilder.Replace("<", "_lt_");
			stringBuilder.Replace(">", "_gt_");
			stringBuilder.Replace(".", "_pt_");
			return string.Format("@{0}", stringBuilder.ToString());
		}

		protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
		{
			((MySqlParameter)parameter).MySqlDbType = (MySqlDbType)row["ProviderType"];
		}

		protected override string GetParameterName(int parameterOrdinal)
		{
			return string.Format("@p{0}", parameterOrdinal.ToString(CultureInfo.InvariantCulture));
		}

		protected override string GetParameterPlaceholder(int parameterOrdinal)
		{
			return string.Format("@p{0}", parameterOrdinal.ToString(CultureInfo.InvariantCulture));
		}

		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
			MySqlDataAdapter mySqlDataAdapter = adapter as MySqlDataAdapter;
			if (adapter != base.DataAdapter)
			{
				mySqlDataAdapter.RowUpdating += new MySqlRowUpdatingEventHandler(this.RowUpdating);
				return;
			}
			mySqlDataAdapter.RowUpdating -= new MySqlRowUpdatingEventHandler(this.RowUpdating);
		}

		private void RowUpdating(object sender, MySqlRowUpdatingEventArgs args)
		{
			base.RowUpdatingHandler(args);
		}
	}
}
