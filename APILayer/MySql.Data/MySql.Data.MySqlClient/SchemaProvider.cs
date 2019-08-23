using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MySql.Data.MySqlClient
{
    internal class SchemaProvider
    {
        // Fields
        protected MySqlConnection connection;
        public static string MetaCollection = "MetaDataCollections";

        // Methods
        public SchemaProvider(MySqlConnection connectionToUse)
        {
            this.connection = connectionToUse;
        }

        internal string[] CleanRestrictions(string[] restrictionValues)
        {
            string[] strArray = null;
            if (restrictionValues != null)
            {
                strArray = (string[])restrictionValues.Clone();
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str = strArray[i];
                    if (str != null)
                    {
                        strArray[i] = str.Trim(new char[] { '`' });
                    }
                }
            }
            return strArray;
        }

        protected static void FillTable(MySqlSchemaCollection dt, object[][] data)
        {
            foreach (object[] objArray in data)
            {
                MySqlSchemaRow row = dt.AddRow();
                for (int i = 0; i < objArray.Length; i++)
                {
                    row[i] = objArray[i];
                }
            }
        }

        private void FindTables(MySqlSchemaCollection schema, string[] restrictions)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, "SHOW TABLE STATUS FROM `{0}`", new object[] { restrictions[1] });
            if (((restrictions != null) && (restrictions.Length >= 3)) && (restrictions[2] != null))
            {
                builder2.AppendFormat(CultureInfo.InvariantCulture, " LIKE '{0}'", new object[] { restrictions[2] });
            }
            builder.Append(builder2.ToString());
            string str = (restrictions[1].ToLower() == "information_schema") ? "SYSTEM VIEW" : "BASE TABLE";
            MySqlCommand command = new MySqlCommand(builder.ToString(), this.connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MySqlSchemaRow row = schema.AddRow();
                    row["TABLE_CATALOG"] = null;
                    row["TABLE_SCHEMA"] = restrictions[1];
                    row["TABLE_NAME"] = reader.GetString(0);
                    row["TABLE_TYPE"] = str;
                    row["ENGINE"] = GetString(reader, 1);
                    row["VERSION"] = reader.GetValue(2);
                    row["ROW_FORMAT"] = GetString(reader, 3);
                    row["TABLE_ROWS"] = reader.GetValue(4);
                    row["AVG_ROW_LENGTH"] = reader.GetValue(5);
                    row["DATA_LENGTH"] = reader.GetValue(6);
                    row["MAX_DATA_LENGTH"] = reader.GetValue(7);
                    row["INDEX_LENGTH"] = reader.GetValue(8);
                    row["DATA_FREE"] = reader.GetValue(9);
                    row["AUTO_INCREMENT"] = reader.GetValue(10);
                    row["CREATE_TIME"] = reader.GetValue(11);
                    row["UPDATE_TIME"] = reader.GetValue(12);
                    row["CHECK_TIME"] = reader.GetValue(13);
                    row["TABLE_COLLATION"] = GetString(reader, 14);
                    row["CHECKSUM"] = reader.GetValue(15);
                    row["CREATE_OPTIONS"] = GetString(reader, 0x10);
                    row["TABLE_COMMENT"] = GetString(reader, 0x11);
                }
            }
        }

        protected virtual MySqlSchemaCollection GetCollections()
        {
            object[][] data = new object[][] { new object[] { "MetaDataCollections", 0, 0 }, new object[] { "DataSourceInformation", 0, 0 }, new object[] { "DataTypes", 0, 0 }, new object[] { "Restrictions", 0, 0 }, new object[] { "ReservedWords", 0, 0 }, new object[] { "Databases", 1, 1 }, new object[] { "Tables", 4, 2 }, new object[] { "Columns", 4, 4 }, new object[] { "Users", 1, 1 }, new object[] { "Foreign Keys", 4, 3 }, new object[] { "IndexColumns", 5, 4 }, new object[] { "Indexes", 4, 3 }, new object[] { "Foreign Key Columns", 4, 3 }, new object[] { "UDF", 1, 1 } };
            MySqlSchemaCollection dt = new MySqlSchemaCollection("MetaDataCollections");
            dt.AddColumn("CollectionName", typeof(string));
            dt.AddColumn("NumberOfRestrictions", typeof(int));
            dt.AddColumn("NumberOfIdentifierParts", typeof(int));
            FillTable(dt, data);
            return dt;
        }

        public virtual MySqlSchemaCollection GetColumns(string[] restrictions)
        {
            MySqlSchemaCollection schemaCollection = new MySqlSchemaCollection("Columns");
            schemaCollection.AddColumn("TABLE_CATALOG", typeof(string));
            schemaCollection.AddColumn("TABLE_SCHEMA", typeof(string));
            schemaCollection.AddColumn("TABLE_NAME", typeof(string));
            schemaCollection.AddColumn("COLUMN_NAME", typeof(string));
            schemaCollection.AddColumn("ORDINAL_POSITION", typeof(ulong));
            schemaCollection.AddColumn("COLUMN_DEFAULT", typeof(string));
            schemaCollection.AddColumn("IS_NULLABLE", typeof(string));
            schemaCollection.AddColumn("DATA_TYPE", typeof(string));
            schemaCollection.AddColumn("CHARACTER_MAXIMUM_LENGTH", typeof(ulong));
            schemaCollection.AddColumn("CHARACTER_OCTET_LENGTH", typeof(ulong));
            schemaCollection.AddColumn("NUMERIC_PRECISION", typeof(ulong));
            schemaCollection.AddColumn("NUMERIC_SCALE", typeof(ulong));
            schemaCollection.AddColumn("CHARACTER_SET_NAME", typeof(string));
            schemaCollection.AddColumn("COLLATION_NAME", typeof(string));
            schemaCollection.AddColumn("COLUMN_TYPE", typeof(string));
            schemaCollection.AddColumn("COLUMN_KEY", typeof(string));
            schemaCollection.AddColumn("EXTRA", typeof(string));
            schemaCollection.AddColumn("PRIVILEGES", typeof(string));
            schemaCollection.AddColumn("COLUMN_COMMENT", typeof(string));
            string columnRestriction = null;
            if ((restrictions != null) && (restrictions.Length == 4))
            {
                columnRestriction = restrictions[3];
                restrictions[3] = null;
            }
            foreach (MySqlSchemaRow row in this.GetTables(restrictions).Rows)
            {
                this.LoadTableColumns(schemaCollection, row["TABLE_SCHEMA"].ToString(), row["TABLE_NAME"].ToString(), columnRestriction);
            }
            this.QuoteDefaultValues(schemaCollection);
            return schemaCollection;
        }

        public virtual MySqlSchemaCollection GetDatabases(string[] restrictions)
        {
            Regex regex = null;
            int num = int.Parse(this.connection.driver.Property("lower_case_table_names"));
            string sql = "SHOW DATABASES";
            if (((num == 0) && (restrictions != null)) && (restrictions.Length >= 1))
            {
                sql = sql + " LIKE '" + restrictions[0] + "'";
            }
            MySqlSchemaCollection schemas = this.QueryCollection("Databases", sql);
            if (((num != 0) && (restrictions != null)) && ((restrictions.Length >= 1) && (restrictions[0] != null)))
            {
                regex = new Regex(restrictions[0], RegexOptions.IgnoreCase);
            }
            MySqlSchemaCollection schemas2 = new MySqlSchemaCollection("Databases");
            schemas2.AddColumn("CATALOG_NAME", typeof(string));
            schemas2.AddColumn("SCHEMA_NAME", typeof(string));
            foreach (MySqlSchemaRow row in schemas.Rows)
            {
                if ((regex == null) || regex.Match(row[0].ToString()).Success)
                {
                    schemas2.AddRow()[1] = row[0];
                }
            }
            return schemas2;
        }

        private MySqlSchemaCollection GetDataSourceInformation()
        {
            MySqlSchemaCollection schemas = new MySqlSchemaCollection("DataSourceInformation");
            schemas.AddColumn("CompositeIdentifierSeparatorPattern", typeof(string));
            schemas.AddColumn("DataSourceProductName", typeof(string));
            schemas.AddColumn("DataSourceProductVersion", typeof(string));
            schemas.AddColumn("DataSourceProductVersionNormalized", typeof(string));
            schemas.AddColumn("GroupByBehavior", typeof(GroupByBehavior));
            schemas.AddColumn("IdentifierPattern", typeof(string));
            schemas.AddColumn("IdentifierCase", typeof(IdentifierCase));
            schemas.AddColumn("OrderByColumnsInSelect", typeof(bool));
            schemas.AddColumn("ParameterMarkerFormat", typeof(string));
            schemas.AddColumn("ParameterMarkerPattern", typeof(string));
            schemas.AddColumn("ParameterNameMaxLength", typeof(int));
            schemas.AddColumn("ParameterNamePattern", typeof(string));
            schemas.AddColumn("QuotedIdentifierPattern", typeof(string));
            schemas.AddColumn("QuotedIdentifierCase", typeof(IdentifierCase));
            schemas.AddColumn("StatementSeparatorPattern", typeof(string));
            schemas.AddColumn("StringLiteralPattern", typeof(string));
            schemas.AddColumn("SupportedJoinOperators", typeof(SupportedJoinOperators));
            DBVersion version = this.connection.driver.Version;
            string str = string.Format("{0:0}.{1:0}.{2:0}", version.Major, version.Minor, version.Build);
            MySqlSchemaRow item = schemas.AddRow();
            item["CompositeIdentifierSeparatorPattern"] = @"\.";
            item["DataSourceProductName"] = "MySQL";
            item["DataSourceProductVersion"] = this.connection.ServerVersion;
            item["DataSourceProductVersionNormalized"] = str;
            item["GroupByBehavior"] = GroupByBehavior.Unrelated;
            item["IdentifierPattern"] = "(^\\`\\p{Lo}\\p{Lu}\\p{Ll}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Nd}@$#_]*$)|(^\\`[^\\`\\0]|\\`\\`+\\`$)|(^\\\" + [^\\\"\\0]|\\\"\\\"+\\\"$)";
            item["IdentifierCase"] = IdentifierCase.Insensitive;
            item["OrderByColumnsInSelect"] = false;
            item["ParameterMarkerFormat"] = "{0}";
            item["ParameterMarkerPattern"] = "(@[A-Za-z0-9_$#]*)";
            item["ParameterNameMaxLength"] = 0x80;
            item["ParameterNamePattern"] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            item["QuotedIdentifierPattern"] = @"(([^\`]|\`\`)*)";
            item["QuotedIdentifierCase"] = IdentifierCase.Sensitive;
            item["StatementSeparatorPattern"] = ";";
            item["StringLiteralPattern"] = "'(([^']|'')*)'";
            item["SupportedJoinOperators"] = 15;
            schemas.Rows.Add(item);
            return schemas;
        }

        private static MySqlSchemaCollection GetDataTypes()
        {
            MySqlSchemaCollection sc = new MySqlSchemaCollection("DataTypes");
            sc.AddColumn("TypeName", typeof(string));
            sc.AddColumn("ProviderDbType", typeof(int));
            sc.AddColumn("ColumnSize", typeof(long));
            sc.AddColumn("CreateFormat", typeof(string));
            sc.AddColumn("CreateParameters", typeof(string));
            sc.AddColumn("DataType", typeof(string));
            sc.AddColumn("IsAutoincrementable", typeof(bool));
            sc.AddColumn("IsBestMatch", typeof(bool));
            sc.AddColumn("IsCaseSensitive", typeof(bool));
            sc.AddColumn("IsFixedLength", typeof(bool));
            sc.AddColumn("IsFixedPrecisionScale", typeof(bool));
            sc.AddColumn("IsLong", typeof(bool));
            sc.AddColumn("IsNullable", typeof(bool));
            sc.AddColumn("IsSearchable", typeof(bool));
            sc.AddColumn("IsSearchableWithLike", typeof(bool));
            sc.AddColumn("IsUnsigned", typeof(bool));
            sc.AddColumn("MaximumScale", typeof(short));
            sc.AddColumn("MinimumScale", typeof(short));
            sc.AddColumn("IsConcurrencyType", typeof(bool));
            sc.AddColumn("IsLiteralSupported", typeof(bool));
            sc.AddColumn("LiteralPrefix", typeof(string));
            sc.AddColumn("LiteralSuffix", typeof(string));
            sc.AddColumn("NativeDataType", typeof(string));
            MySqlBit.SetDSInfo(sc);
            MySqlBinary.SetDSInfo(sc);
            MySqlDateTime.SetDSInfo(sc);
            MySqlTimeSpan.SetDSInfo(sc);
            MySqlString.SetDSInfo(sc);
            MySqlDouble.SetDSInfo(sc);
            MySqlSingle.SetDSInfo(sc);
            MySqlByte.SetDSInfo(sc);
            MySqlInt16.SetDSInfo(sc);
            MySqlInt32.SetDSInfo(sc);
            MySqlInt64.SetDSInfo(sc);
            MySqlDecimal.SetDSInfo(sc);
            MySqlUByte.SetDSInfo(sc);
            MySqlUInt16.SetDSInfo(sc);
            MySqlUInt32.SetDSInfo(sc);
            MySqlUInt64.SetDSInfo(sc);
            return sc;
        }

        public virtual MySqlSchemaCollection GetForeignKeyColumns(string[] restrictions)
        {
            MySqlSchemaCollection fkTable = new MySqlSchemaCollection("Foreign Keys");
            fkTable.AddColumn("CONSTRAINT_CATALOG", typeof(string));
            fkTable.AddColumn("CONSTRAINT_SCHEMA", typeof(string));
            fkTable.AddColumn("CONSTRAINT_NAME", typeof(string));
            fkTable.AddColumn("TABLE_CATALOG", typeof(string));
            fkTable.AddColumn("TABLE_SCHEMA", typeof(string));
            fkTable.AddColumn("TABLE_NAME", typeof(string));
            fkTable.AddColumn("COLUMN_NAME", typeof(string));
            fkTable.AddColumn("ORDINAL_POSITION", typeof(int));
            fkTable.AddColumn("REFERENCED_TABLE_CATALOG", typeof(string));
            fkTable.AddColumn("REFERENCED_TABLE_SCHEMA", typeof(string));
            fkTable.AddColumn("REFERENCED_TABLE_NAME", typeof(string));
            fkTable.AddColumn("REFERENCED_COLUMN_NAME", typeof(string));
            string filterName = null;
            if ((restrictions != null) && (restrictions.Length >= 4))
            {
                filterName = restrictions[3];
                restrictions[3] = null;
            }
            foreach (MySqlSchemaRow row in this.GetTables(restrictions).Rows)
            {
                this.GetForeignKeysOnTable(fkTable, row, filterName, true);
            }
            return fkTable;
        }

        public virtual MySqlSchemaCollection GetForeignKeys(string[] restrictions)
        {
            MySqlSchemaCollection fkTable = new MySqlSchemaCollection("Foreign Keys");
            fkTable.AddColumn("CONSTRAINT_CATALOG", typeof(string));
            fkTable.AddColumn("CONSTRAINT_SCHEMA", typeof(string));
            fkTable.AddColumn("CONSTRAINT_NAME", typeof(string));
            fkTable.AddColumn("TABLE_CATALOG", typeof(string));
            fkTable.AddColumn("TABLE_SCHEMA", typeof(string));
            fkTable.AddColumn("TABLE_NAME", typeof(string));
            fkTable.AddColumn("MATCH_OPTION", typeof(string));
            fkTable.AddColumn("UPDATE_RULE", typeof(string));
            fkTable.AddColumn("DELETE_RULE", typeof(string));
            fkTable.AddColumn("REFERENCED_TABLE_CATALOG", typeof(string));
            fkTable.AddColumn("REFERENCED_TABLE_SCHEMA", typeof(string));
            fkTable.AddColumn("REFERENCED_TABLE_NAME", typeof(string));
            string filterName = null;
            if ((restrictions != null) && (restrictions.Length >= 4))
            {
                filterName = restrictions[3];
                restrictions[3] = null;
            }
            foreach (MySqlSchemaRow row in this.GetTables(restrictions).Rows)
            {
                this.GetForeignKeysOnTable(fkTable, row, filterName, false);
            }
            return fkTable;
        }

        private void GetForeignKeysOnTable(MySqlSchemaCollection fkTable, MySqlSchemaRow tableToParse, string filterName, bool includeColumns)
        {
            string sqlMode = this.GetSqlMode();
            if (filterName != null)
            {
                filterName = StringUtility.ToLowerInvariant(filterName);
            }
            string cmdText = string.Format("SHOW CREATE TABLE `{0}`.`{1}`", tableToParse["TABLE_SCHEMA"], tableToParse["TABLE_NAME"]);
            string input = null;
            MySqlCommand command = new MySqlCommand(cmdText, this.connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                input = StringUtility.ToLowerInvariant(reader.GetString(1));
            }
            MySqlTokenizer tokenizer = new MySqlTokenizer(input)
            {
                AnsiQuotes = sqlMode.IndexOf("ANSI_QUOTES") != -1,
                BackslashEscapes = sqlMode.IndexOf("NO_BACKSLASH_ESCAPES") != -1
            };
            while (true)
            {
                string str5 = tokenizer.NextToken();
                while ((str5 != null) && ((str5 != "constraint") || tokenizer.Quoted))
                {
                    str5 = tokenizer.NextToken();
                }
                if (str5 == null)
                {
                    return;
                }
                ParseConstraint(fkTable, tableToParse, tokenizer, includeColumns);
            }
        }

        public virtual MySqlSchemaCollection GetIndexColumns(string[] restrictions)
        {
            MySqlSchemaCollection schemas = new MySqlSchemaCollection("IndexColumns");
            schemas.AddColumn("INDEX_CATALOG", typeof(string));
            schemas.AddColumn("INDEX_SCHEMA", typeof(string));
            schemas.AddColumn("INDEX_NAME", typeof(string));
            schemas.AddColumn("TABLE_NAME", typeof(string));
            schemas.AddColumn("COLUMN_NAME", typeof(string));
            schemas.AddColumn("ORDINAL_POSITION", typeof(int));
            schemas.AddColumn("SORT_ORDER", typeof(string));
            int num = (restrictions == null) ? 4 : restrictions.Length;
            string[] array = new string[Math.Max(num, 4)];
            if (restrictions != null)
            {
                restrictions.CopyTo(array, 0);
            }
            array[3] = "BASE TABLE";
            foreach (MySqlSchemaRow row in this.GetTables(array).Rows)
            {
                MySqlCommand command = new MySqlCommand(string.Format("SHOW INDEX FROM `{0}`.`{1}`", row["TABLE_SCHEMA"], row["TABLE_NAME"]), this.connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string str2 = GetString(reader, reader.GetOrdinal("KEY_NAME"));
                        string str3 = GetString(reader, reader.GetOrdinal("COLUMN_NAME"));
                        if ((restrictions == null) || ((((restrictions.Length < 4) || (restrictions[3] == null)) || (str2 == restrictions[3])) && (((restrictions.Length < 5) || (restrictions[4] == null)) || (str3 == restrictions[4]))))
                        {
                            MySqlSchemaRow row2 = schemas.AddRow();
                            row2["INDEX_CATALOG"] = null;
                            row2["INDEX_SCHEMA"] = row["TABLE_SCHEMA"];
                            row2["INDEX_NAME"] = str2;
                            row2["TABLE_NAME"] = GetString(reader, reader.GetOrdinal("TABLE"));
                            row2["COLUMN_NAME"] = str3;
                            row2["ORDINAL_POSITION"] = reader.GetValue(reader.GetOrdinal("SEQ_IN_INDEX"));
                            row2["SORT_ORDER"] = reader.GetString("COLLATION");
                        }
                    }
                }
            }
            return schemas;
        }

        public virtual MySqlSchemaCollection GetIndexes(string[] restrictions)
        {
            MySqlSchemaCollection schemas = new MySqlSchemaCollection("Indexes");
            schemas.AddColumn("INDEX_CATALOG", typeof(string));
            schemas.AddColumn("INDEX_SCHEMA", typeof(string));
            schemas.AddColumn("INDEX_NAME", typeof(string));
            schemas.AddColumn("TABLE_NAME", typeof(string));
            schemas.AddColumn("UNIQUE", typeof(bool));
            schemas.AddColumn("PRIMARY", typeof(bool));
            schemas.AddColumn("TYPE", typeof(string));
            schemas.AddColumn("COMMENT", typeof(string));
            int num = (restrictions == null) ? 4 : restrictions.Length;
            string[] array = new string[Math.Max(num, 4)];
            if (restrictions != null)
            {
                restrictions.CopyTo(array, 0);
            }
            array[3] = "BASE TABLE";
            foreach (MySqlSchemaRow row in this.GetTables(array).Rows)
            {
                string sql = string.Format("SHOW INDEX FROM `{0}`.`{1}`", MySqlHelper.DoubleQuoteString((string)row["TABLE_SCHEMA"]), MySqlHelper.DoubleQuoteString((string)row["TABLE_NAME"]));
                foreach (MySqlSchemaRow row2 in this.QueryCollection("indexes", sql).Rows)
                {
                    long num2 = (long)row2["SEQ_IN_INDEX"];
                    if ((num2 == 1L) && (((restrictions == null) || (restrictions.Length != 4)) || ((restrictions[3] == null) || row2["KEY_NAME"].Equals(restrictions[3]))))
                    {
                        MySqlSchemaRow row3 = schemas.AddRow();
                        row3["INDEX_CATALOG"] = null;
                        row3["INDEX_SCHEMA"] = row["TABLE_SCHEMA"];
                        row3["INDEX_NAME"] = row2["KEY_NAME"];
                        row3["TABLE_NAME"] = row2["TABLE"];
                        row3["UNIQUE"] = ((long)row2["NON_UNIQUE"]) == 0L;
                        row3["PRIMARY"] = row2["KEY_NAME"].Equals("PRIMARY");
                        row3["TYPE"] = row2["INDEX_TYPE"];
                        row3["COMMENT"] = row2["COMMENT"];
                    }
                }
            }
            return schemas;
        }

        public virtual MySqlSchemaCollection GetProcedures(string[] restrictions)
        {
            MySqlSchemaCollection schemas = new MySqlSchemaCollection("Procedures");
            schemas.AddColumn("SPECIFIC_NAME", typeof(string));
            schemas.AddColumn("ROUTINE_CATALOG", typeof(string));
            schemas.AddColumn("ROUTINE_SCHEMA", typeof(string));
            schemas.AddColumn("ROUTINE_NAME", typeof(string));
            schemas.AddColumn("ROUTINE_TYPE", typeof(string));
            schemas.AddColumn("DTD_IDENTIFIER", typeof(string));
            schemas.AddColumn("ROUTINE_BODY", typeof(string));
            schemas.AddColumn("ROUTINE_DEFINITION", typeof(string));
            schemas.AddColumn("EXTERNAL_NAME", typeof(string));
            schemas.AddColumn("EXTERNAL_LANGUAGE", typeof(string));
            schemas.AddColumn("PARAMETER_STYLE", typeof(string));
            schemas.AddColumn("IS_DETERMINISTIC", typeof(string));
            schemas.AddColumn("SQL_DATA_ACCESS", typeof(string));
            schemas.AddColumn("SQL_PATH", typeof(string));
            schemas.AddColumn("SECURITY_TYPE", typeof(string));
            schemas.AddColumn("CREATED", typeof(DateTime));
            schemas.AddColumn("LAST_ALTERED", typeof(DateTime));
            schemas.AddColumn("SQL_MODE", typeof(string));
            schemas.AddColumn("ROUTINE_COMMENT", typeof(string));
            schemas.AddColumn("DEFINER", typeof(string));
            StringBuilder builder = new StringBuilder("SELECT * FROM mysql.proc WHERE 1=1");
            if (restrictions != null)
            {
                if ((restrictions.Length >= 2) && (restrictions[1] != null))
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture, " AND db LIKE '{0}'", new object[] { restrictions[1] });
                }
                if ((restrictions.Length >= 3) && (restrictions[2] != null))
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture, " AND name LIKE '{0}'", new object[] { restrictions[2] });
                }
                if ((restrictions.Length >= 4) && (restrictions[3] != null))
                {
                    builder.AppendFormat(CultureInfo.InvariantCulture, " AND type LIKE '{0}'", new object[] { restrictions[3] });
                }
            }
            MySqlCommand command = new MySqlCommand(builder.ToString(), this.connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MySqlSchemaRow row = schemas.AddRow();
                    row["SPECIFIC_NAME"] = reader.GetString("specific_name");
                    row["ROUTINE_CATALOG"] = DBNull.Value;
                    row["ROUTINE_SCHEMA"] = reader.GetString("db");
                    row["ROUTINE_NAME"] = reader.GetString("name");
                    string s = reader.GetString("type");
                    row["ROUTINE_TYPE"] = s;
                    row["DTD_IDENTIFIER"] = (StringUtility.ToLowerInvariant(s) == "function") ? ((object)reader.GetString("returns")) : ((object)DBNull.Value);
                    row["ROUTINE_BODY"] = "SQL";
                    row["ROUTINE_DEFINITION"] = reader.GetString("body");
                    row["EXTERNAL_NAME"] = DBNull.Value;
                    row["EXTERNAL_LANGUAGE"] = DBNull.Value;
                    row["PARAMETER_STYLE"] = "SQL";
                    row["IS_DETERMINISTIC"] = reader.GetString("is_deterministic");
                    row["SQL_DATA_ACCESS"] = reader.GetString("sql_data_access");
                    row["SQL_PATH"] = DBNull.Value;
                    row["SECURITY_TYPE"] = reader.GetString("security_type");
                    row["CREATED"] = reader.GetDateTime("created");
                    row["LAST_ALTERED"] = reader.GetDateTime("modified");
                    row["SQL_MODE"] = reader.GetString("sql_mode");
                    row["ROUTINE_COMMENT"] = reader.GetString("comment");
                    row["DEFINER"] = reader.GetString("definer");
                }
            }
            return schemas;
        }

        private static MySqlSchemaCollection GetReservedWords()
        {
            MySqlSchemaCollection schemas = new MySqlSchemaCollection("ReservedWords");
            schemas.AddColumn(DbMetaDataColumnNames.ReservedWord, typeof(string));
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MySql.Data.MySqlClient.Properties.ReservedWords.txt");
            StreamReader reader = new StreamReader(manifestResourceStream);
            for (string str = reader.ReadLine(); str != null; str = reader.ReadLine())
            {
                foreach (string str2 in str.Split(new char[] { ' ' }))
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        schemas.AddRow()[0] = str2;
                    }
                }
            }
            reader.Dispose();
            manifestResourceStream.Close();
            return schemas;
        }

        protected virtual MySqlSchemaCollection GetRestrictions()
        {
            object[][] data = new object[][] { 
            new object[] { "Users", "Name", "", 0 }, new object[] { "Databases", "Name", "", 0 }, new object[] { "Tables", "Database", "", 0 }, new object[] { "Tables", "Schema", "", 1 }, new object[] { "Tables", "Table", "", 2 }, new object[] { "Tables", "TableType", "", 3 }, new object[] { "Columns", "Database", "", 0 }, new object[] { "Columns", "Schema", "", 1 }, new object[] { "Columns", "Table", "", 2 }, new object[] { "Columns", "Column", "", 3 }, new object[] { "Indexes", "Database", "", 0 }, new object[] { "Indexes", "Schema", "", 1 }, new object[] { "Indexes", "Table", "", 2 }, new object[] { "Indexes", "Name", "", 3 }, new object[] { "IndexColumns", "Database", "", 0 }, new object[] { "IndexColumns", "Schema", "", 1 }, 
            new object[] { "IndexColumns", "Table", "", 2 }, new object[] { "IndexColumns", "ConstraintName", "", 3 }, new object[] { "IndexColumns", "Column", "", 4 }, new object[] { "Foreign Keys", "Database", "", 0 }, new object[] { "Foreign Keys", "Schema", "", 1 }, new object[] { "Foreign Keys", "Table", "", 2 }, new object[] { "Foreign Keys", "Constraint Name", "", 3 }, new object[] { "Foreign Key Columns", "Catalog", "", 0 }, new object[] { "Foreign Key Columns", "Schema", "", 1 }, new object[] { "Foreign Key Columns", "Table", "", 2 }, new object[] { "Foreign Key Columns", "Constraint Name", "", 3 }, new object[] { "UDF", "Name", "", 0 }
         };
            MySqlSchemaCollection dt = new MySqlSchemaCollection("Restrictions");
            dt.AddColumn("CollectionName", typeof(string));
            dt.AddColumn("RestrictionName", typeof(string));
            dt.AddColumn("RestrictionDefault", typeof(string));
            dt.AddColumn("RestrictionNumber", typeof(int));
            FillTable(dt, data);
            return dt;
        }

        public virtual MySqlSchemaCollection GetSchema(string collection, string[] restrictions)
        {
            if (this.connection.State != ConnectionState.Open)
            {
                throw new MySqlException("GetSchema can only be called on an open connection.");
            }
            collection = StringUtility.ToUpperInvariant(collection);
            MySqlSchemaCollection schemaInternal = this.GetSchemaInternal(collection, restrictions);
            if (schemaInternal == null)
            {
                throw new ArgumentException("Invalid collection name");
            }
            return schemaInternal;
        }

        protected virtual MySqlSchemaCollection GetSchemaInternal(string collection, string[] restrictions)
        {
            switch (collection)
            {
                case "METADATACOLLECTIONS":
                    return this.GetCollections();

                case "DATASOURCEINFORMATION":
                    return this.GetDataSourceInformation();

                case "DATATYPES":
                    return GetDataTypes();

                case "RESTRICTIONS":
                    return this.GetRestrictions();

                case "RESERVEDWORDS":
                    return GetReservedWords();

                case "USERS":
                    return this.GetUsers(restrictions);

                case "DATABASES":
                    return this.GetDatabases(restrictions);

                case "UDF":
                    return this.GetUDF(restrictions);
            }
            if (restrictions == null)
            {
                restrictions = new string[2];
            }
            if ((((this.connection != null) && (this.connection.Database != null)) && ((this.connection.Database.Length > 0) && (restrictions.Length > 1))) && (restrictions[1] == null))
            {
                restrictions[1] = this.connection.Database;
            }
            switch (collection)
            {
                case "TABLES":
                    return this.GetTables(restrictions);

                case "COLUMNS":
                    return this.GetColumns(restrictions);

                case "INDEXES":
                    return this.GetIndexes(restrictions);

                case "INDEXCOLUMNS":
                    return this.GetIndexColumns(restrictions);

                case "FOREIGN KEYS":
                    return this.GetForeignKeys(restrictions);

                case "FOREIGN KEY COLUMNS":
                    return this.GetForeignKeyColumns(restrictions);
            }
            return null;
        }

        private string GetSqlMode()
        {
            MySqlCommand command = new MySqlCommand("SELECT @@SQL_MODE", this.connection);
            return command.ExecuteScalar().ToString();
        }

        private static string GetString(MySqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetString(index);
        }

        public virtual MySqlSchemaCollection GetTables(string[] restrictions)
        {
            MySqlSchemaCollection schema = new MySqlSchemaCollection("Tables");
            schema.AddColumn("TABLE_CATALOG", typeof(string));
            schema.AddColumn("TABLE_SCHEMA", typeof(string));
            schema.AddColumn("TABLE_NAME", typeof(string));
            schema.AddColumn("TABLE_TYPE", typeof(string));
            schema.AddColumn("ENGINE", typeof(string));
            schema.AddColumn("VERSION", typeof(ulong));
            schema.AddColumn("ROW_FORMAT", typeof(string));
            schema.AddColumn("TABLE_ROWS", typeof(ulong));
            schema.AddColumn("AVG_ROW_LENGTH", typeof(ulong));
            schema.AddColumn("DATA_LENGTH", typeof(ulong));
            schema.AddColumn("MAX_DATA_LENGTH", typeof(ulong));
            schema.AddColumn("INDEX_LENGTH", typeof(ulong));
            schema.AddColumn("DATA_FREE", typeof(ulong));
            schema.AddColumn("AUTO_INCREMENT", typeof(ulong));
            schema.AddColumn("CREATE_TIME", typeof(DateTime));
            schema.AddColumn("UPDATE_TIME", typeof(DateTime));
            schema.AddColumn("CHECK_TIME", typeof(DateTime));
            schema.AddColumn("TABLE_COLLATION", typeof(string));
            schema.AddColumn("CHECKSUM", typeof(ulong));
            schema.AddColumn("CREATE_OPTIONS", typeof(string));
            schema.AddColumn("TABLE_COMMENT", typeof(string));
            string[] strArray = new string[4];
            if ((restrictions != null) && (restrictions.Length >= 2))
            {
                strArray[0] = restrictions[1];
            }
            MySqlSchemaCollection databases = this.GetDatabases(strArray);
            if (restrictions != null)
            {
                Array.Copy(restrictions, strArray, Math.Min(strArray.Length, restrictions.Length));
            }
            foreach (MySqlSchemaRow row in databases.Rows)
            {
                strArray[1] = row["SCHEMA_NAME"].ToString();
                this.FindTables(schema, strArray);
            }
            return schema;
        }

        public virtual MySqlSchemaCollection GetUDF(string[] restrictions)
        {
            string cmdText = "SELECT name,ret,dl FROM mysql.func";
            if (((restrictions != null) && (restrictions.Length >= 1)) && !string.IsNullOrEmpty(restrictions[0]))
            {
                cmdText = cmdText + string.Format(" WHERE name LIKE '{0}'", restrictions[0]);
            }
            MySqlSchemaCollection schemas = new MySqlSchemaCollection("User-defined Functions");
            schemas.AddColumn("NAME", typeof(string));
            schemas.AddColumn("RETURN_TYPE", typeof(int));
            schemas.AddColumn("LIBRARY_NAME", typeof(string));
            MySqlCommand command = new MySqlCommand(cmdText, this.connection);
            try
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MySqlSchemaRow row = schemas.AddRow();
                        row[0] = reader.GetString(0);
                        row[1] = reader.GetInt32(1);
                        row[2] = reader.GetString(2);
                    }
                }
            }
            catch (MySqlException exception)
            {
                if (exception.Number != 0x476)
                {
                    throw;
                }
                throw new MySqlException(Resources.UnableToEnumerateUDF, exception);
            }
            return schemas;
        }

        public virtual MySqlSchemaCollection GetUsers(string[] restrictions)
        {
            StringBuilder builder = new StringBuilder("SELECT Host, User FROM mysql.user");
            if ((restrictions != null) && (restrictions.Length > 0))
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, " WHERE User LIKE '{0}'", new object[] { restrictions[0] });
            }
            MySqlSchemaCollection schemas = this.QueryCollection("Users", builder.ToString());
            schemas.Columns[0].Name = "HOST";
            schemas.Columns[1].Name = "USERNAME";
            return schemas;
        }

        private void LoadTableColumns(MySqlSchemaCollection schemaCollection, string schema, string tableName, string columnRestriction)
        {
            MySqlCommand command = new MySqlCommand(string.Format("SHOW FULL COLUMNS FROM `{0}`.`{1}`", schema, tableName), this.connection);
            int num = 1;
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string str2 = reader.GetString(0);
                    if ((columnRestriction == null) || (str2 == columnRestriction))
                    {
                        MySqlSchemaRow row = schemaCollection.AddRow();
                        row["TABLE_CATALOG"] = DBNull.Value;
                        row["TABLE_SCHEMA"] = schema;
                        row["TABLE_NAME"] = tableName;
                        row["COLUMN_NAME"] = str2;
                        row["ORDINAL_POSITION"] = num++;
                        row["COLUMN_DEFAULT"] = reader.GetValue(5);
                        row["IS_NULLABLE"] = reader.GetString(3);
                        row["DATA_TYPE"] = reader.GetString(1);
                        row["CHARACTER_MAXIMUM_LENGTH"] = DBNull.Value;
                        row["CHARACTER_OCTET_LENGTH"] = DBNull.Value;
                        row["NUMERIC_PRECISION"] = DBNull.Value;
                        row["NUMERIC_SCALE"] = DBNull.Value;
                        row["CHARACTER_SET_NAME"] = reader.GetValue(2);
                        row["COLLATION_NAME"] = row["CHARACTER_SET_NAME"];
                        row["COLUMN_TYPE"] = reader.GetString(1);
                        row["COLUMN_KEY"] = reader.GetString(4);
                        row["EXTRA"] = reader.GetString(6);
                        row["PRIVILEGES"] = reader.GetString(7);
                        row["COLUMN_COMMENT"] = reader.GetString(8);
                        ParseColumnRow(row);
                    }
                }
            }
        }

        private static void ParseColumnRow(MySqlSchemaRow row)
        {
            string str = row["CHARACTER_SET_NAME"].ToString();
            int index = str.IndexOf('_');
            if (index != -1)
            {
                row["CHARACTER_SET_NAME"] = str.Substring(0, index);
            }
            string str2 = row["DATA_TYPE"].ToString();
            index = str2.IndexOf('(');
            if (index != -1)
            {
                row["DATA_TYPE"] = str2.Substring(0, index);
                int num2 = str2.IndexOf(')', index);
                string str3 = str2.Substring(index + 1, num2 - (index + 1));
                switch (row["DATA_TYPE"].ToString().ToLower())
                {
                    case "char":
                    case "varchar":
                        row["CHARACTER_MAXIMUM_LENGTH"] = str3;
                        return;

                    case "real":
                    case "decimal":
                        {
                            string[] strArray = str3.Split(new char[] { ',' });
                            row["NUMERIC_PRECISION"] = strArray[0];
                            if (strArray.Length == 2)
                            {
                                row["NUMERIC_SCALE"] = strArray[1];
                            }
                            break;
                        }
                }
            }
        }

        private static List<string> ParseColumns(MySqlTokenizer tokenizer)
        {
            List<string> list = new List<string>();
            for (string str = tokenizer.NextToken(); str != ")"; str = tokenizer.NextToken())
            {
                if (str != ",")
                {
                    list.Add(str);
                }
            }
            return list;
        }

        private static void ParseConstraint(MySqlSchemaCollection fkTable, MySqlSchemaRow table, MySqlTokenizer tokenizer, bool includeColumns)
        {
            string str = tokenizer.NextToken();
            MySqlSchemaRow row = fkTable.AddRow();
            string str2 = tokenizer.NextToken();
            if ((str2 == "foreign") && !tokenizer.Quoted)
            {
                tokenizer.NextToken();
                tokenizer.NextToken();
                row["CONSTRAINT_CATALOG"] = table["TABLE_CATALOG"];
                row["CONSTRAINT_SCHEMA"] = table["TABLE_SCHEMA"];
                row["TABLE_CATALOG"] = table["TABLE_CATALOG"];
                row["TABLE_SCHEMA"] = table["TABLE_SCHEMA"];
                row["TABLE_NAME"] = table["TABLE_NAME"];
                row["REFERENCED_TABLE_CATALOG"] = null;
                row["CONSTRAINT_NAME"] = str.Trim(new char[] { '\'', '`' });
                List<string> srcColumns = includeColumns ? ParseColumns(tokenizer) : null;
                while ((str2 != "references") || tokenizer.Quoted)
                {
                    str2 = tokenizer.NextToken();
                }
                string str3 = tokenizer.NextToken();
                string str4 = tokenizer.NextToken();
                if (str4.StartsWith(".", StringComparison.Ordinal))
                {
                    row["REFERENCED_TABLE_SCHEMA"] = str3;
                    row["REFERENCED_TABLE_NAME"] = str4.Substring(1).Trim(new char[] { '\'', '`' });
                    tokenizer.NextToken();
                }
                else
                {
                    row["REFERENCED_TABLE_SCHEMA"] = table["TABLE_SCHEMA"];
                    row["REFERENCED_TABLE_NAME"] = str3.Substring(1).Trim(new char[] { '\'', '`' });
                }
                List<string> targetColumns = includeColumns ? ParseColumns(tokenizer) : null;
                if (includeColumns)
                {
                    ProcessColumns(fkTable, row, srcColumns, targetColumns);
                }
                else
                {
                    fkTable.Rows.Add(row);
                }
            }
        }

        private static void ProcessColumns(MySqlSchemaCollection fkTable, MySqlSchemaRow row, List<string> srcColumns, List<string> targetColumns)
        {
            for (int i = 0; i < srcColumns.Count; i++)
            {
                MySqlSchemaRow row2 = fkTable.AddRow();
                row.CopyRow(row2);
                row2["COLUMN_NAME"] = srcColumns[i];
                row2["ORDINAL_POSITION"] = i;
                row2["REFERENCED_COLUMN_NAME"] = targetColumns[i];
                fkTable.Rows.Add(row2);
            }
        }

        protected MySqlSchemaCollection QueryCollection(string name, string sql)
        {
            MySqlSchemaCollection schemas = new MySqlSchemaCollection(name);
            MySqlDataReader reader = new MySqlCommand(sql, this.connection).ExecuteReader();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                schemas.AddColumn(reader.GetName(i), reader.GetFieldType(i));
            }
            using (reader)
            {
                while (reader.Read())
                {
                    MySqlSchemaRow row = schemas.AddRow();
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        row[j] = reader.GetValue(j);
                    }
                }
            }
            return schemas;
        }

        protected void QuoteDefaultValues(MySqlSchemaCollection schemaCollection)
        {
            if ((schemaCollection != null) && schemaCollection.ContainsColumn("COLUMN_DEFAULT"))
            {
                foreach (MySqlSchemaRow row in schemaCollection.Rows)
                {
                    object obj2 = row["COLUMN_DEFAULT"];
                    if (MetaData.IsTextType(row["DATA_TYPE"].ToString()))
                    {
                        row["COLUMN_DEFAULT"] = string.Format("{0}", obj2);
                    }
                }
            }
        }
    }
}
