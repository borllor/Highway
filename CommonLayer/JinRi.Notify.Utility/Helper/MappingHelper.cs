using System;

namespace JinRi.Notify.Utility
{
    public class MappingHelper
    {
        /// <summary>
        /// 将对象映射到目标对象
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <typeparam name="U">原对象</typeparam>
        /// <param name="fromObj">原对象实例</param>
        /// <returns>返回目标对象</returns>
        public static T From<T, U>(U fromObj) where T : new()
        {
            if (fromObj == null) return default(T);
            EmitMapper.ObjectsMapper<U, T> mapper = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<U, T>();
            return mapper.Map(fromObj);
        }
   
        /// <summary>
        /// 将对象映射到目标对象（两个实例都已创建，不用在构造新的实例）
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <typeparam name="U">原对象</typeparam>
        /// <param name="fromObj">原对象实例</param>
        /// <returns>返回目标对象</returns>
        public static T MappingFrom<T, U>(T obj, U fromObj) where T : new()
        {
            T ret = obj;
            if (fromObj == null || obj == null)
            {
                return ret;
            }
            EmitMapper.ObjectsMapper<U, T> tet = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<U, T>();
            T sourceObj = tet.Map(fromObj, ret);
            return sourceObj;
        }
    }
}
