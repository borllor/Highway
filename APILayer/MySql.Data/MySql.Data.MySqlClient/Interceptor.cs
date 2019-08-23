using System;

namespace MySql.Data.MySqlClient
{
	internal abstract class Interceptor
	{
		protected MySqlConnection connection;

		protected void LoadInterceptors(string interceptorList)
		{
			if (string.IsNullOrEmpty(interceptorList))
			{
				return;
			}
			string[] array = interceptorList.Split(new char[]
			{
				'|'
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!string.IsNullOrEmpty(text))
				{
					string typeName = this.ResolveType(text);
					Type type = Type.GetType(typeName);
					object o = Activator.CreateInstance(type);
					this.AddInterceptor(o);
				}
			}
		}

		protected abstract void AddInterceptor(object o);

		protected virtual string ResolveType(string nameOrType)
		{
			return nameOrType;
		}
	}
}
