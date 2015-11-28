using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Gunde.Core.Reflection
{
    public static class ReflectionExtensions
    {
        public static T GetCustomAttribute<T>(this MethodInfo method, bool inherit = true) where T : Attribute
        {
            return method.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
        }

        public static string GetDisplayName(this MethodInfo method)
        {
            var attribute = method.GetCustomAttribute<DisplayNameAttribute>();

            return attribute?.DisplayName ?? method.Name;
        }

        public static string GetDisplayName(this Type type)
        {
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();

            return attribute?.DisplayName ?? type.Name;
        }
    }
}