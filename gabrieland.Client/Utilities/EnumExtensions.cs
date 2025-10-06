using System;
using System.ComponentModel.DataAnnotations;

namespace gabrieland.Client.Utilities
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Devuelve el texto definido en [Display(Name="…")] o ToString().
        /// </summary>
        public static string GetDisplay(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attr = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute))
                        as DisplayAttribute;

            return attr?.Name ?? value.ToString();
        }
    }
}
