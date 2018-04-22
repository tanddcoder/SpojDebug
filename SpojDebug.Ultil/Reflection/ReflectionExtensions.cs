using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SpojDebug.Ultil.Reflection
{
    public static class ReflectionExtensions
    {
        public static string GetDisplayName(this Enum enumVal)
        {
            Type enumType = enumVal.GetType();

            var enumValue = Enum.GetName(enumType, enumVal);

            MemberInfo member = enumType.GetMember(enumValue).FirstOrDefault();

            if (!(member.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute displayAttribute))
            {
                return null;
            }

            var displayName = displayAttribute.ResourceType != null ? displayAttribute.GetName() : displayAttribute.Name;

            return !string.IsNullOrWhiteSpace(displayName) ? displayName : null;
        }
    }
}
