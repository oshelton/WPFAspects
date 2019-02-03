using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFAspects.Utils;

namespace UtilTests
{
    [TestClass]
    public class PropertyGetterTests
    {
        private class TestObject
        {
            public static bool StaticVarOne { get; set; } = true;
            private static bool StaticVarTwo { get; set; }

            public int VarOne { get; set; }
            internal int VarTwo { get; set; } = 1;
            protected int VarThree { get; set; } = 2;
            private int VarFour { get; set; } = 3;
        }

        ///Test default PropertyGetter.GetProperties behavior; should only return the VarOne property.
        [TestMethod]
        public void TestDefaultBindingFlags()
        {
            var testObject = new TestObject();
            var properties = testObject.GetProperties();

            Assert.AreEqual(1, properties.Count());
            Assert.AreEqual("VarOne", properties.First().Name);
        }

        ///Test more advanced PropertyGetter.GetProperties behavior; should just return the StaticVarOne property.
        [TestMethod]
        public void TestAdvancedBindingFlags()
        {
            var testObject = new TestObject();
            var properties = testObject.GetProperties(BindingFlags.Static | BindingFlags.Public);

            Assert.AreEqual(1, properties.Count());
            Assert.AreEqual(true, properties.Any(p => p.Name == nameof(TestObject.StaticVarOne)));
        }

        [TestMethod]
        public void TestGetValue()
        {
            var testObject = new TestObject();

            Assert.AreEqual(true, testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
            Assert.AreEqual(0, testObject.GetPropertyValue<int>(nameof(TestObject.VarOne)));
        }

        [TestMethod]
        public void TestSetValue()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(nameof(TestObject.VarOne), 2);
            Assert.AreEqual(2, testObject.VarOne);
            testObject.SetPropertyValue(nameof(testObject.VarTwo), 3, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.AreEqual(3, testObject.VarTwo);

            TestObject.StaticVarOne = true;
            Assert.AreEqual(true, testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
            testObject.SetPropertyValue(nameof(TestObject.StaticVarOne), false, BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(false, testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
            testObject.SetPropertyValue(nameof(TestObject.StaticVarOne), true, BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(true, testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
        }
    }
}
