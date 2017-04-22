using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Utils
{
    ///Class for retrieving property information for a particular type.
    ///The framework itself seems to do some caching with regards to this stuff so I am not going to try to outdo it.
    ///<remarks>
    ///Only returns public instance properties by default.
    ///Reflection is costly, don't use it if you have a choice.
    ///</remarks>
    public static class PropertyGetter
    {
        ///Get all properties of an object matching the passed in binding flags.
        public static IEnumerable<PropertyInfo> GetProperties(this object obj, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            return obj.GetType().GetProperties(flags);
        }

        ///Get the value of a specific property of an object.
        public static T GetPropertyValue<T>(this object obj, string propertyName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            return (T)obj.GetType().GetProperty(propertyName, flags).GetValue(obj);
        }

        ///Set the value of a specific property of an object.
        public static void SetPropertyValue<T>(this object obj, string propertyName, T value, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            obj.GetType().GetProperty(propertyName, flags).SetValue(obj, value);
        }
    }
}
