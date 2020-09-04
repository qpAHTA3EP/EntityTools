
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace EntityTools.Reflection
{
    public sealed class CopyHelper
    {
        private static readonly Type array_type = typeof(Array);

        private static readonly MethodInfo memberwise_clone = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void MakeArrayRowDeepCopy(Dictionary<object, object> state,
            Array array, int[] indices, int rank)
        {
            int next_rank = rank + 1;
            int upper_bound = array.GetUpperBound(rank);

            while (indices[rank] <= upper_bound)
            {
                object value = array.GetValue(indices);
                if (value != null)
                    array.SetValue(CreateDeepCopyInternal(state, value), indices);

                if (next_rank < array.Rank)
                    MakeArrayRowDeepCopy(state, array, indices, next_rank);

                indices[rank] += 1;
            }
            indices[rank] = array.GetLowerBound(rank);
        }

        private static Array CreateArrayDeepCopy(Dictionary<object, object> state, Array array)
        {
            Array result = (Array)array.Clone();
            int[] indices = new int[result.Rank];
            for (int rank = 0; rank < indices.Length; ++rank)
                indices[rank] = result.GetLowerBound(rank);
            MakeArrayRowDeepCopy(state, result, indices, 0);
            return result;
        }

        private static object CreateDeepCopyInternal(Dictionary<object, object> state,
            object o)
        {
            if (state.TryGetValue(o, out object exist_object))
                return exist_object;

            if (o is Array)
            {
                object array_copy = CreateArrayDeepCopy(state, (Array)o);
                state[o] = array_copy;
                return array_copy;
            }
            else if (o is string)
            {
                object string_copy = string.Copy((string)o);
                state[o] = string_copy;
                return string_copy;
            }
            else
            {
                Type o_type = o.GetType();
                if (o_type.IsPrimitive)
                    return o;
                object copy = memberwise_clone.Invoke(o, null);
                state[o] = copy;
                foreach (FieldInfo f in o_type.GetFields(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    object original = f.GetValue(o);
                    if (original != null)
                        f.SetValue(copy, CreateDeepCopyInternal(state, original));
                }
                return copy;
            }
        }

        public static T CreateDeepCopy<T>(T o)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
#endif
                if (ReferenceEquals(o, null))
                    return o;
                return (T)CreateDeepCopyInternal(new Dictionary<object, object>(), o);
#if PROFILING
            }
            finally
            {
                stopwatch.Stop();
                ETLogger.WriteLine(LogType.Debug, $"CreateDeepCopy() worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CreateXmlCopy<T>(T source)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
#endif
            if (!typeof(T).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                // Don't serialize a null object, simply return the default for that object
                if (ReferenceEquals(source, null))
                {
                    return default(T);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
#if PROFILING
            }
            finally
            {
                stopwatch.Stop();
                ETLogger.WriteLine(LogType.Debug, $"CreateXmlCopy() worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }
    }
}
