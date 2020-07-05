using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntityTools.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Tools.Tests
{
    [TestClass]
    public class StringToPatternComparerTests
    {
        [TestMethod]
        [DataRow("SomeTestString")]
        [DataRow("75039-123!@#!%")]
        [DataRow("")]
        public void Get__Empty__NotNull_False(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("");

            Assert.IsNotNull(predicate);
            Assert.IsFalse(predicate(value));
        }

        [TestMethod]
        [DataRow("SomeTestString")]
        [DataRow("75039-123!@#!%")]
        [DataRow("")]
        public void Get__Wildcard_Simple__NotNull_True(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("*");

            Assert.IsNotNull(predicate);
            Assert.IsTrue(predicate(value));
        }

        [TestMethod]
        [DataRow("SomeTestString")]
        [DataRow("75039-123!@#!%")]
        [DataRow("")]
        public void Get__Wildcard2_Simple__NotNull_True(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("**");

            Assert.IsNotNull(predicate);
            Assert.IsTrue(predicate(value));
        }

        [TestMethod]
        [DataRow(";asldfkj;lasdf")]
        [DataRow("SomeTest")]
        [DataRow("So_meTest")]
        [DataRow("SOMEtest")]
        public void Get__WildcardSome_Simple__NotNull_False(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("*Some");

            Assert.IsNotNull(predicate);
            Assert.IsFalse(predicate(value));
        }

        [TestMethod]
        [DataRow("Some")]
        [DataRow("TestSome")]
        public void Get__WildcardSome_Simple__NotNull_True(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("*Some");

            Assert.IsNotNull(predicate);
            Assert.IsTrue(predicate(value));
        }

        [TestMethod]
        [DataRow("So_me")]
        [DataRow("")]
        [DataRow("SOME")]
        public void Get__WildcardSomeWildcard_Simple__NotNull_False(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("*Some*");

            Assert.IsNotNull(predicate);
            Assert.IsFalse(predicate(value));
        }

        [TestMethod]
        [DataRow("Some")]
        [DataRow("TestSome")]
        [DataRow("TestSome_")]
        public void Get__WildcardSomeWildcard_Simple__NotNull_True(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("*Some*");

            Assert.IsNotNull(predicate);
            Assert.IsFalse(predicate(value));
        }

        [TestMethod]
        [DataRow("_Some")]
        [DataRow("_Some_")]
        public void Get__SomeWildcard_Simple__NotNull_False(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("Some*");

            Assert.IsNotNull(predicate);
            Assert.IsFalse(predicate(value));
        }

        [TestMethod]
        [DataRow("Some")]
        [DataRow("SomeTest")]
        [DataRow("Some_Test")]
        public void Get__SomeWildcard_Simple__NotNull_True(string value)
        {
            Predicate<string> predicate = StringToPatternComparer.Get("Some*");

            Assert.IsNotNull(predicate);
            Assert.IsTrue(predicate(value));
        }
    }
}