using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFAspects.Utils;

namespace UtilTests
{
    [TestClass]
    public class PropertyGetterTests
    {
        private class TestObject
        {
            public static bool StaticVarOne { get; set; }
            private static bool StaticVarTwo { get; set; } = true;

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
    }
}
