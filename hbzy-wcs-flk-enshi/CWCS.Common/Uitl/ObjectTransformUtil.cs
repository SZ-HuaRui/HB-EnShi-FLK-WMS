using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Common.Uitl
{
    public class ObjectTransformUtil
    {
		/// <summary>
		/// 深拷贝一个对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sourceObject"></param>
		/// <returns></returns>
		public static T Clone<T>(T sourceObject)
		{
			object newObject;
			try
			{
				Type type = sourceObject.GetType();
				newObject = Activator.CreateInstance(type);
				foreach (PropertyInfo info in type.GetProperties())
				{
					if(info.CanWrite)info.SetValue(newObject,info.GetValue(sourceObject));
				}
			}
			catch
			{
				newObject = default;
			}
			return newObject == null ? default : (T)newObject;
		}
	}
}
