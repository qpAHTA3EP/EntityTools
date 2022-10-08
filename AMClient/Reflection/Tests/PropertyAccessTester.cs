using AStar;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ACTP0Tools.Reflection.Tests
{
    public class PropertyAccessTester
    {
        private readonly Type type;
        private readonly Traverse currentRole;
        private readonly object currentRoleObject;
        private readonly Traverse<Graph> traverse;

#if false
// Ошибка времени выполнения доступа к методу, т.к. Astral.Addons.Role - internal
            var myAccessor = type.GetStaticProperty<Astral.Addons.Role>("CurrentRole"); 
#else
        private readonly PropertyAccessor<Graph> myAccessor;
#endif
        private readonly Stopwatch sw = new Stopwatch();

        public PropertyAccessTester()
        {
            type = Assembly.GetEntryAssembly()?.GetType("Astral.Controllers.Roles");
            currentRole = Traverse.Create(type).Property("CurrentRole");
            currentRoleObject = currentRole.GetValue();
            
            traverse = Traverse.Create(currentRoleObject).Property<Graph>("UsedMeshes");
            myAccessor = currentRoleObject.GetProperty<Graph>("UsedMeshes");
        }


        public string Test()
        {
            sw.Start();
            for (int i = 0; i< 1_000_000; i++)
            {
                var g = traverse.Value;
            }
            sw.Stop();

            var traverseMs = sw.ElapsedMilliseconds;
            var traverseTicks = sw.ElapsedTicks;

            sw.Restart();
            for (int i = 0; i< 1_000_000; i++)
            {
                var g = myAccessor.Value;
            }
            sw.Stop();

            var accessorMs = sw.ElapsedMilliseconds;
            var accessorTicks = sw.ElapsedTicks;

            return string.Format("Traverse : {0,10} {1,20}\n" +
                                 "Accessor : {2,10} {3,20}",
                                             traverseMs, traverseTicks,
                                             accessorMs, accessorTicks); 
        }
    }

    public class PropertyTestTarget
    {
        public int Number { get; set; }
    }

    public class SimplePropertyAccessTester
    {
        private Type targetType;
        private Traverse<int> traverse;

#if false
// Ошибка времени выполнения доступа к методу, т.к. Astral.Addons.Role - internal
            var myAccessor = type.GetStaticProperty<Astral.Addons.Role>("CurrentRole"); 
#else
        private PropertyAccessor<int> myAccessor;
#endif
        private readonly Stopwatch sw = new Stopwatch();

        public SimplePropertyAccessTester()
        {
            targetType = typeof(SimplePropertyAccessTester);

        }


        public string Test()
        {
            var target = new PropertyTestTarget();

            traverse = Traverse.Create(target).Property<int>(nameof(PropertyTestTarget.Number));
            myAccessor = target.GetProperty<int>(nameof(PropertyTestTarget.Number));

            sw.Start();
            for (int i = 0; i < 1_000_000; i++)
            {
                var g = traverse.Value;
            }
            sw.Stop();

            var traverseMs = sw.ElapsedMilliseconds;
            var traverseTicks = sw.ElapsedTicks;

            sw.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                var g = myAccessor.Value;
            }
            sw.Stop();

            var accessorMs = sw.ElapsedMilliseconds;
            var accessorTicks = sw.ElapsedTicks;

            sw.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                var g = target.Number;
            }
            sw.Stop();

            var directMs = sw.ElapsedMilliseconds;
            var directTicks = sw.ElapsedTicks;

            return string.Format("{0,-8} : {1,10:0} {2,10:0}\n" +
                                 "{3,-8} : {4,10:0} {5,10:0}\n" +
                                 "{6,-8} : {7,10:0} {8,10:0}",
                                 "Traverse", traverseMs, traverseTicks,
                                 "Accessor", accessorMs, accessorTicks,
                                 "Direct", directMs, directTicks);
        }
    }
}
