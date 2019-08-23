using System;
using System.Data;
using System.Text;

namespace MySql.Data.MySqlClient
{
	public sealed class MySqlHelper
	{
		private enum CharClass : byte
		{
			None,
			Quote,
			Backslash
		}

		private static string stringOfBackslashChars = "\\¥Š₩∖﹨＼";

		private static string stringOfQuoteChars = "\"'`´ʹʺʻʼˈˊˋ˙̀́‘’‚′‵❛❜＇";

		private static MySqlHelper.CharClass[] charClassArray = MySqlHelper.makeCharClassArray();

		private MySqlHelper()
		{
		}

		public static int ExecuteNonQuery(MySqlConnection connection, string commandText, params MySqlParameter[] commandParameters)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			mySqlCommand.Connection = connection;
			mySqlCommand.CommandText = commandText;
			mySqlCommand.CommandType = CommandType.Text;
			if (commandParameters != null)
			{
				for (int i = 0; i < commandParameters.Length; i++)
				{
					MySqlParameter value = commandParameters[i];
					mySqlCommand.Parameters.Add(value);
				}
			}
			int result = mySqlCommand.ExecuteNonQuery();
			mySqlCommand.Parameters.Clear();
			return result;
		}

		public static int ExecuteNonQuery(string connectionString, string commandText, params MySqlParameter[] parms)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				result = MySqlHelper.ExecuteNonQuery(mySqlConnection, commandText, parms);
			}
			return result;
		}

		public static DataRow ExecuteDataRow(string connectionString, string commandText, params MySqlParameter[] parms)
		{
			DataSet dataSet = MySqlHelper.ExecuteDataset(connectionString, commandText, parms);
			if (dataSet == null)
			{
				return null;
			}
			if (dataSet.Tables.Count == 0)
			{
				return null;
			}
			if (dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return dataSet.Tables[0].Rows[0];
		}

		public static DataSet ExecuteDataset(string connectionString, string commandText)
		{
			return MySqlHelper.ExecuteDataset(connectionString, commandText, null);
		}

		public static DataSet ExecuteDataset(string connectionString, string commandText, params MySqlParameter[] commandParameters)
		{
			DataSet result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				result = MySqlHelper.ExecuteDataset(mySqlConnection, commandText, commandParameters);
			}
			return result;
		}

		public static DataSet ExecuteDataset(MySqlConnection connection, string commandText)
		{
			return MySqlHelper.ExecuteDataset(connection, commandText, null);
		}

		public static DataSet ExecuteDataset(MySqlConnection connection, string commandText, params MySqlParameter[] commandParameters)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			mySqlCommand.Connection = connection;
			mySqlCommand.CommandText = commandText;
			mySqlCommand.CommandType = CommandType.Text;
			if (commandParameters != null)
			{
				for (int i = 0; i < commandParameters.Length; i++)
				{
					MySqlParameter value = commandParameters[i];
					mySqlCommand.Parameters.Add(value);
				}
			}
			MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
			DataSet dataSet = new DataSet();
			mySqlDataAdapter.Fill(dataSet);
			mySqlCommand.Parameters.Clear();
			return dataSet;
		}

		public static void UpdateDataSet(string connectionString, string commandText, DataSet ds, string tablename)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			mySqlConnection.Open();
			MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(commandText, mySqlConnection);
			MySqlCommandBuilder mySqlCommandBuilder = new MySqlCommandBuilder(mySqlDataAdapter);
			mySqlCommandBuilder.ToString();
			mySqlDataAdapter.Update(ds, tablename);
			mySqlConnection.Close();
		}

		private static MySqlDataReader ExecuteReader(MySqlConnection connection, MySqlTransaction transaction, string commandText, MySqlParameter[] commandParameters, bool ExternalConn)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			mySqlCommand.Connection = connection;
			mySqlCommand.Transaction = transaction;
			mySqlCommand.CommandText = commandText;
			mySqlCommand.CommandType = CommandType.Text;
			if (commandParameters != null)
			{
				for (int i = 0; i < commandParameters.Length; i++)
				{
					MySqlParameter value = commandParameters[i];
					mySqlCommand.Parameters.Add(value);
				}
			}
			MySqlDataReader result;
			if (ExternalConn)
			{
				result = mySqlCommand.ExecuteReader();
			}
			else
			{
				result = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			}
			mySqlCommand.Parameters.Clear();
			return result;
		}

		public static MySqlDataReader ExecuteReader(string connectionString, string commandText)
		{
			return MySqlHelper.ExecuteReader(connectionString, commandText, null);
		}

		public static MySqlDataReader ExecuteReader(MySqlConnection connection, string commandText)
		{
			return MySqlHelper.ExecuteReader(connection, null, commandText, null, true);
		}

		public static MySqlDataReader ExecuteReader(string connectionString, string commandText, params MySqlParameter[] commandParameters)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			mySqlConnection.Open();
			return MySqlHelper.ExecuteReader(mySqlConnection, null, commandText, commandParameters, false);
		}

		public static MySqlDataReader ExecuteReader(MySqlConnection connection, string commandText, params MySqlParameter[] commandParameters)
		{
			return MySqlHelper.ExecuteReader(connection, null, commandText, commandParameters, true);
		}

		public static object ExecuteScalar(string connectionString, string commandText)
		{
			return MySqlHelper.ExecuteScalar(connectionString, commandText, null);
		}

		public static object ExecuteScalar(string connectionString, string commandText, params MySqlParameter[] commandParameters)
		{
			object result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
			{
				mySqlConnection.Open();
				result = MySqlHelper.ExecuteScalar(mySqlConnection, commandText, commandParameters);
			}
			return result;
		}

		public static object ExecuteScalar(MySqlConnection connection, string commandText)
		{
			return MySqlHelper.ExecuteScalar(connection, commandText, null);
		}

		public static object ExecuteScalar(MySqlConnection connection, string commandText, params MySqlParameter[] commandParameters)
		{
			MySqlCommand mySqlCommand = new MySqlCommand();
			mySqlCommand.Connection = connection;
			mySqlCommand.CommandText = commandText;
			mySqlCommand.CommandType = CommandType.Text;
			if (commandParameters != null)
			{
				for (int i = 0; i < commandParameters.Length; i++)
				{
					MySqlParameter value = commandParameters[i];
					mySqlCommand.Parameters.Add(value);
				}
			}
			object result = mySqlCommand.ExecuteScalar();
			mySqlCommand.Parameters.Clear();
			return result;
		}

		private static MySqlHelper.CharClass[] makeCharClassArray()
		{
			MySqlHelper.CharClass[] array = new MySqlHelper.CharClass[65536];
			string text = MySqlHelper.stringOfBackslashChars;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				array[(int)c] = MySqlHelper.CharClass.Backslash;
			}
			string text2 = MySqlHelper.stringOfQuoteChars;
			for (int j = 0; j < text2.Length; j++)
			{
				char c2 = text2[j];
				array[(int)c2] = MySqlHelper.CharClass.Quote;
			}
			return array;
		}

		private static bool needsQuoting(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (MySqlHelper.charClassArray[(int)c] != MySqlHelper.CharClass.None)
				{
					return true;
				}
			}
			return false;
		}

		public static string EscapeString(string value)
		{
			if (!MySqlHelper.needsQuoting(value))
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				MySqlHelper.CharClass charClass = MySqlHelper.charClassArray[(int)c];
				if (charClass != MySqlHelper.CharClass.None)
				{
					stringBuilder.Append("\\");
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public static string DoubleQuoteString(string value)
		{
			if (!MySqlHelper.needsQuoting(value))
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				MySqlHelper.CharClass charClass = MySqlHelper.charClassArray[(int)c];
				if (charClass == MySqlHelper.CharClass.Quote)
				{
					stringBuilder.Append(c);
				}
				else if (charClass == MySqlHelper.CharClass.Backslash)
				{
					stringBuilder.Append("\\");
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}
	}
}
