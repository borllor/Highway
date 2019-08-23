using MySql.Data.MySqlClient.Properties;
using MySql.Data.Types;
using System;
using System.Data;
using System.Globalization;
using System.Text;

namespace MySql.Data.MySqlClient
{
    internal class StoredProcedure : PreparableStatement
    {
        internal const string ParameterPrefix = "_cnet_param_";

        private string outSelect;

        private string resolvedCommandText;

        private bool serverProvidingOutputParameters;

        public bool ServerProvidingOutputParameters
        {
            get
            {
                return this.serverProvidingOutputParameters;
            }
        }

        public override string ResolvedCommandText
        {
            get
            {
                return this.resolvedCommandText;
            }
        }

        public StoredProcedure(MySqlCommand cmd, string text)
            : base(cmd, text)
        {
        }

        private MySqlParameter GetReturnParameter()
        {
            if (base.Parameters != null)
            {
                foreach (MySqlParameter mySqlParameter in base.Parameters)
                {
                    if (mySqlParameter.Direction == ParameterDirection.ReturnValue)
                    {
                        return mySqlParameter;
                    }
                }
            }
            return null;
        }

        internal string GetCacheKey(string spName)
        {
            string str = string.Empty;
            StringBuilder stringBuilder = new StringBuilder(spName);
            stringBuilder.Append("(");
            string text = "";
            foreach (MySqlParameter mySqlParameter in this.command.Parameters)
            {
                if (mySqlParameter.Direction == ParameterDirection.ReturnValue)
                {
                    str = "?=";
                }
                else
                {
                    stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}?", new object[]
					{
						text
					});
                    text = ",";
                }
            }
            stringBuilder.Append(")");
            return str + stringBuilder.ToString();
        }

        private ProcedureCacheEntry GetParameters(string procName)
        {
            string cacheKey = this.GetCacheKey(procName);
            return base.Connection.ProcedureCache.GetProcedure(base.Connection, procName, cacheKey);
        }

        public static string GetFlags(string dtd)
        {
            int num = dtd.Length - 1;
            while (num > 0 && (char.IsLetterOrDigit(dtd[num]) || dtd[num] == ' '))
            {
                num--;
            }
            string s = dtd.Substring(num);
            return StringUtility.ToUpperInvariant(s);
        }

        private string FixProcedureName(string name)
        {
            string[] array = name.Split(new char[]
			{
				'.'
			});
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].StartsWith("`", StringComparison.Ordinal))
                {
                    array[i] = string.Format("`{0}`", array[i]);
                }
            }
            if (array.Length == 1)
            {
                return array[0];
            }
            return string.Format("{0}.{1}", array[0], array[1]);
        }

        private MySqlParameter GetAndFixParameter(string spName, MySqlSchemaRow param, bool realAsFloat, MySqlParameter returnParameter)
        {
            string arg_10_0 = (string)param["PARAMETER_MODE"];
            string parameterName = (string)param["PARAMETER_NAME"];
            if (param["ORDINAL_POSITION"].Equals(0))
            {
                if (returnParameter == null)
                {
                    throw new InvalidOperationException(string.Format(Resources.RoutineRequiresReturnParameter, spName));
                }
                parameterName = returnParameter.ParameterName;
            }
            MySqlParameter parameterFlexible = this.command.Parameters.GetParameterFlexible(parameterName, true);
            if (!parameterFlexible.TypeHasBeenSet)
            {
                string typeName = (string)param["DATA_TYPE"];
                bool unsigned = StoredProcedure.GetFlags(param["DTD_IDENTIFIER"].ToString()).IndexOf("UNSIGNED") != -1;
                parameterFlexible.MySqlDbType = MetaData.NameToType(typeName, unsigned, realAsFloat, base.Connection);
            }
            return parameterFlexible;
        }

        private MySqlParameterCollection CheckParameters(string spName)
        {
            MySqlParameterCollection mySqlParameterCollection = new MySqlParameterCollection(this.command);
            MySqlParameter returnParameter = this.GetReturnParameter();
            ProcedureCacheEntry parameters = this.GetParameters(spName);
            if (parameters.procedure == null || parameters.procedure.Rows.Count == 0)
            {
                throw new InvalidOperationException(string.Format(Resources.RoutineNotFound, spName));
            }
            bool realAsFloat = parameters.procedure.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1;
            foreach (MySqlSchemaRow current in parameters.parameters.Rows)
            {
                mySqlParameterCollection.Add(this.GetAndFixParameter(spName, current, realAsFloat, returnParameter));
            }
            return mySqlParameterCollection;
        }

        public override void Resolve(bool preparing)
        {
            if (this.resolvedCommandText != null)
            {
                return;
            }
            this.serverProvidingOutputParameters = (base.Driver.SupportsOutputParameters && preparing);
            string text = this.commandText;
            if (text.IndexOf(".") == -1 && !string.IsNullOrEmpty(base.Connection.Database))
            {
                text = base.Connection.Database + "." + text;
            }
            text = this.FixProcedureName(text);
            MySqlParameter returnParameter = this.GetReturnParameter();
            MySqlParameterCollection parms = this.command.Connection.Settings.CheckParameters ? this.CheckParameters(text) : base.Parameters;
            string arg = this.SetUserVariables(parms, preparing);
            string arg2 = this.CreateCallStatement(text, returnParameter, parms);
            string arg3 = this.CreateOutputSelect(parms, preparing);
            this.resolvedCommandText = string.Format("{0}{1}{2}", arg, arg2, arg3);
        }

        private string SetUserVariables(MySqlParameterCollection parms, bool preparing)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.serverProvidingOutputParameters)
            {
                return stringBuilder.ToString();
            }
            string text = string.Empty;
            foreach (MySqlParameter mySqlParameter in parms)
            {
                if (mySqlParameter.Direction == ParameterDirection.InputOutput)
                {
                    string arg = "@" + mySqlParameter.BaseName;
                    string arg2 = "@_cnet_param_" + mySqlParameter.BaseName;
                    string text2 = string.Format("SET {0}={1}", arg2, arg);
                    if (this.command.Connection.Settings.AllowBatch && !preparing)
                    {
                        stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[]
						{
							text,
							text2
						});
                        text = "; ";
                    }
                    else
                    {
                        new MySqlCommand(text2, this.command.Connection)
                        {
                            Parameters = 
							{
								mySqlParameter
							}
                        }.ExecuteNonQuery();
                    }
                }
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append("; ");
            }
            return stringBuilder.ToString();
        }

        private string CreateCallStatement(string spName, MySqlParameter returnParameter, MySqlParameterCollection parms)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = string.Empty;
            foreach (MySqlParameter mySqlParameter in parms)
            {
                if (mySqlParameter.Direction != ParameterDirection.ReturnValue)
                {
                    string text2 = "@" + mySqlParameter.BaseName;
                    string text3 = "@_cnet_param_" + mySqlParameter.BaseName;
                    bool flag = mySqlParameter.Direction == ParameterDirection.Input || this.serverProvidingOutputParameters;
                    stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[]
					{
						text,
						flag ? text2 : text3
					});
                    text = ", ";
                }
            }
            if (returnParameter == null)
            {
                return string.Format("CALL {0}({1})", spName, stringBuilder.ToString());
            }
            return string.Format("SET @{0}{1}={2}({3})", new object[]
			{
				"_cnet_param_",
				returnParameter.BaseName,
				spName,
				stringBuilder.ToString()
			});
        }

        private string CreateOutputSelect(MySqlParameterCollection parms, bool preparing)
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            foreach (MySqlParameter parameter in parms)
            {
                if ((parameter.Direction != ParameterDirection.Input) && (((parameter.Direction != ParameterDirection.InputOutput) && (parameter.Direction != ParameterDirection.Output)) || !this.serverProvidingOutputParameters))
                {
                    string text1 = "@" + parameter.BaseName;
                    string str2 = "@_cnet_param_" + parameter.BaseName;
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[] { str, str2 });
                    str = ", ";
                }
            }
            if (builder.Length != 0)
            {
                if (base.command.Connection.Settings.AllowBatch && !preparing)
                {
                    return string.Format(";SELECT {0}", builder.ToString());
                }
                this.outSelect = string.Format("SELECT {0}", builder.ToString());
            }
            return string.Empty;
        }

        internal void ProcessOutputParameters(MySqlDataReader reader)
        {
            this.AdjustOutputTypes(reader);
            if ((reader.CommandBehavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default)
            {
                return;
            }
            reader.Read();
            string text = "@_cnet_param_";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string text2 = reader.GetName(i);
                if (text2.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                {
                    text2 = text2.Remove(0, text.Length);
                }
                MySqlParameter parameterFlexible = this.command.Parameters.GetParameterFlexible(text2, true);
                parameterFlexible.Value = reader.GetValue(i);
            }
        }

        private void AdjustOutputTypes(MySqlDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string text = reader.GetName(i);
                if (text.IndexOf("_cnet_param_") != -1)
                {
                    text = text.Remove(0, "_cnet_param_".Length + 1);
                }
                MySqlParameter parameterFlexible = this.command.Parameters.GetParameterFlexible(text, true);
                IMySqlValue iMySqlValue = MySqlField.GetIMySqlValue(parameterFlexible.MySqlDbType);
                if (iMySqlValue is MySqlBit)
                {
                    MySqlBit mySqlBit = (MySqlBit)iMySqlValue;
                    mySqlBit.ReadAsString = true;
                    reader.ResultSet.SetValueObject(i, mySqlBit);
                }
                else
                {
                    reader.ResultSet.SetValueObject(i, iMySqlValue);
                }
            }
        }

        public override void Close(MySqlDataReader reader)
        {
            base.Close(reader);
            if (string.IsNullOrEmpty(this.outSelect))
            {
                return;
            }
            if ((reader.CommandBehavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default)
            {
                return;
            }
            MySqlCommand mySqlCommand = new MySqlCommand(this.outSelect, this.command.Connection);
            using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader(reader.CommandBehavior))
            {
                this.ProcessOutputParameters(mySqlDataReader);
            }
        }
    }
}
