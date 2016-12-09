using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System
{
    public static class EnumHelpers
    {
        public static string EnumDisplayDescriptionFor(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member.First().GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            if (displayName != null)
            {
                return displayName.Description;
            }

            return item.ToString();
        }

        public static string GetEnumDescription(this Enum item)
        {
            var descriptionAttr = (DescriptionAttribute)item.GetType().GetMember(item.ToString()).First().GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();

            if (descriptionAttr != null)
            {
                return descriptionAttr.Description;
            }

            return item.ToString();
        }


        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var attributes = type.GetTypeInfo().GetCustomAttributes<T>();
            return (attributes.Count() > 0) ? (T)attributes.FirstOrDefault() : null;
        }
    }
}
