//#define PROFILING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using ACTP0Tools.Patches;
using Astral;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterActionPack = Astral.Quester.Classes.ActionPack;
using QuesterCondition = Astral.Quester.Classes.Condition;
using QuesterProfile = Astral.Quester.Classes.Profile;
using UccAction = Astral.Logic.UCC.Classes.UCCAction;
using UccCondition = Astral.Logic.UCC.Classes.UCCCondition;
using UccProfile = Astral.Logic.UCC.Classes.Profil;

namespace ACTP0Tools.Reflection
{
    public static class CopyHelper
    {
        private static readonly Type tType = typeof(Type);
        private static readonly Type tPropertyInfo = typeof(PropertyInfo);
        private static readonly Type tFieldInfo = typeof(FieldInfo);
        private static readonly Type tMemberInfo = typeof(MemberInfo);
        private static readonly Type tMethodBase = typeof(MethodBase);
        private static readonly Type tMethodInfo = typeof(MethodInfo);
        private static readonly Type tMethodBody = typeof(MethodBody);
        private static readonly Type tAssembly = typeof(Assembly);
        //private static readonly Type tAssemblyExtensions = typeof(AssemblyExtensions);
        private static readonly Type tException = typeof(Exception);
        private static readonly Type tAttribute = typeof(Attribute);
        private static readonly Type tString = typeof(string);
        private static readonly Type tPointer = typeof(Pointer);
        private static readonly MethodInfo memberwise_clone = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

#if false
        #region CreateDeepCopy by writabel Property
        /// <summary>
        /// <see href="https://www.c-sharpcorner.com/UploadFile/ff2f08/deep-copy-of-object-in-C-Sharp/"/>
        /// Методика не работает с индексируемыми свойсствами, с многомерными массивами и вспомогательными объектами <see cref="ACTP0Tools.Reflection"/>
        /// </summary>
        #endregion
#endif


#if true
        #region Reflection deepCopy by fields
        public static T CreateDeepCopy<T>(this T sourceObject)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (ReferenceEquals(sourceObject, null))
                    return sourceObject;
                object obj = sourceObject;
                return (T)CreateDeepCopyInternal(new Dictionary<object, object>(), ref obj);
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(Path.Combine(Astral.Controllers.Directories.LogsPath, DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateDeepCopy<T>(T) worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        private static object CreateDeepCopyInternal(Dictionary<object, object> innerObjects, ref object obj)
        {
            if (innerObjects.TryGetValue(obj, out object existsObject))
                return existsObject;

            var type = obj.GetType();

            if (type.IsPrimitive
                || type.IsValueType
                || type.IsEnum
                || type == tType
                || type == tString)
                return obj;

            if (type.IsArray)
                return CreateArrayDeepCopy(innerObjects, (Array)obj);
            var objCopy = memberwise_clone.Invoke(obj, null);
            innerObjects[obj] = objCopy;

            // BUG добавить копирование авто-свойств
            foreach (var field in type.EnumerateFields())
            {
                if (field.IsInitOnly
                    || field.IsLiteral
                    || field.IsStatic)
                    continue;
                var fieldValue = field.GetValue(obj);
                if (fieldValue != null)
                    field.SetValue(objCopy, CreateDeepCopyInternal(innerObjects, ref fieldValue));
            }

            return objCopy;
        }

        private static Array CreateArrayDeepCopy(Dictionary<object, object> innerObjects, Array array)
        {
            Array arrayCopy = (Array)array.Clone();
            innerObjects[array] = arrayCopy;

            int[] indices = new int[arrayCopy.Rank];
            for (int rank = 0; rank < indices.Length; ++rank)
                indices[rank] = arrayCopy.GetLowerBound(rank);
            MakeArrayRowDeepCopy(innerObjects, arrayCopy, indices, 0);
            return arrayCopy;
        }

        private static void MakeArrayRowDeepCopy(Dictionary<object, object> innerObjects, Array array, int[] indices, int rank)
        {
            int nextRank = rank + 1;
            int upperBound = array.GetUpperBound(rank);

            while (indices[rank] <= upperBound)
            {
                var value = array.GetValue(indices);
                if (value != null)
                    array.SetValue(CreateDeepCopyInternal(innerObjects, ref value), indices);

                if (nextRank < array.Rank)
                    MakeArrayRowDeepCopy(innerObjects, array, indices, nextRank);

                indices[rank] += 1;
            }
            indices[rank] = array.GetLowerBound(rank);
        }
        #endregion  
#endif

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CreateXmlCopy<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                // Don't serialize a null object, simply return the default for that object
                if (ReferenceEquals(source, null))
                {
                    return default;
                }

                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)serializer.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy() worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

#if true
        public static UccProfile CreateXmlCopy(this UccProfile profile)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (profile is null)
                    return null;

                using (var stream = new MemoryStream())
                {
                    var serializer = ACTP0Serializer.UccProfileSerializer;
                    serializer.Serialize(stream, profile);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream) as UccProfile;
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy() worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        public static UccAction CreateXmlCopy(this UccAction uccAction)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (uccAction is null)
                    return null;

                using (var stream = new MemoryStream())
                {
                    var serializer = ACTP0Serializer.UccActionSerializer;
                    serializer.Serialize(stream, uccAction);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream) as UccAction;
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy() worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        public static UccCondition CreateXmlCopy(this UccCondition uccCondition)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (uccCondition is null)
                    return null;

                using (var stream = new MemoryStream())
                {
                    var serializer = ACTP0Serializer.UccConditionSerializer;
                    serializer.Serialize(stream, uccCondition);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream) as UccCondition;
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy() worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        public static QuesterProfile CreateXmlCopy(this QuesterProfile profile)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (profile is null)
                    return null;

                using (var stream = new MemoryStream())
                {
                    var serializer = ACTP0Serializer.QuesterProfileSerializer;
                    serializer.Serialize(stream, profile);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream) as QuesterProfile;
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy(QuesterProfile) worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        public static QuesterAction CreateXmlCopy(this QuesterAction questerAction)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (questerAction is null)
                    return null;

                using (var stream = new MemoryStream())
                {
                    var serializer = ACTP0Serializer.QuesterActionSerializer;
                    serializer.Serialize(stream, questerAction);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream) as QuesterAction;
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy(QuesterAction) worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        public static QuesterActionPack CreateXmlCopy(this QuesterActionPack questerAction)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (questerAction is null)
                    return null;

                using (var stream = new MemoryStream())
                {
                    var serializer = ACTP0Serializer.QuesterActionSerializer;
                    serializer.Serialize(stream, questerAction);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream) as QuesterActionPack;
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy(QuesterAction) worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        public static QuesterCondition CreateXmlCopy(this QuesterCondition questerCondition)
        {
#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                if (questerCondition is null)
                    return null;

                using (var stream = new MemoryStream())
                {
                    var serializer = ACTP0Serializer.QuesterConditionSerializer;
                    serializer.Serialize(stream, questerCondition);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream) as QuesterCondition;
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateXmlCopy(QuesterCondition) worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }
#endif

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CreateBinaryCopy<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

#if PROFILING
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            try
            {
                // Don't serialize a null object, simply return the default for that object
                if (ReferenceEquals(source, default))
                {
                    return default;
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                var builder = new StringBuilder();
                builder.IntroduceException(e);
                var text = builder.ToString();
                File.WriteAllText(
                    Path.Combine(Astral.Controllers.Directories.LogsPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss ") + nameof(CreateDeepCopy) + ".log"), text);
                Logger.WriteLine(Logger.LogType.Debug, text);
                throw;
            }
#if PROFILING
            finally
            {
                stopwatch.Stop();
                Logger.WriteLine(Logger.LogType.Debug, $"CreateBinaryCopy<T>(T) worktime is {stopwatch.ElapsedMilliseconds} ms");
            }
#endif
        }

        public static StringBuilder IntroduceException(this StringBuilder builder, Exception e)
        {
            builder.Append(e).AppendLine();

            if (e.InnerException != null)
                builder.IntroduceException(e.InnerException);

            return builder;
        }
    }
}
