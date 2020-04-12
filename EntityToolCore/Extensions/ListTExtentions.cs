using EntityTools.Reflection;
using System.Collections.Generic;

namespace EntityCore.Extensions
{
    public static class ListTExtentions
    {
        public static List<T> Clone<T>(this List<T> @this)
        {
            if(@this != null)
            {
                List<T> listCopy = new List<T>(@this.Count);
                foreach(T t in @this)
                {
                    listCopy.Add(CopyHelper.CreateDeepCopy(t));
                }

                return listCopy;
            }
            return null;
        }
    }
}
