using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace MySql.Data.MySqlClient
{
	internal class MySqlParameterConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				ConstructorInfo constructor = typeof(MySqlParameter).GetConstructor(new Type[]
				{
					typeof(string),
					typeof(MySqlDbType),
					typeof(int),
					typeof(ParameterDirection),
					typeof(bool),
					typeof(byte),
					typeof(byte),
					typeof(string),
					typeof(DataRowVersion),
					typeof(object)
				});
				MySqlParameter mySqlParameter = (MySqlParameter)value;
				return new InstanceDescriptor(constructor, new object[]
				{
					mySqlParameter.ParameterName,
					mySqlParameter.DbType,
					mySqlParameter.Size,
					mySqlParameter.Direction,
					mySqlParameter.IsNullable,
					mySqlParameter.Precision,
					mySqlParameter.Scale,
					mySqlParameter.SourceColumn,
					mySqlParameter.SourceVersion,
					mySqlParameter.Value
				});
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
