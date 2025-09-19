using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace ActiveOfficeLife.Admin.Helpers
{
    public static class EnumDisplayExtensions
    {
        // Dùng cho enum (non-nullable)
        public static string GetDisplayName<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            var type = typeof(TEnum);
            var name = Enum.GetName(type, value);
            if (name is null) return value.ToString();

            var field = type.GetField(name);
            if (field is null) return name;

            // [Display(Name="...")]
            var display = field.GetCustomAttribute<DisplayAttribute>();
            if (display != null) return display.GetName();

            // [Description("...")]
            var desc = field.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null) return desc.Description;

            // [EnumMember(Value="...")]
            var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
            if (!string.IsNullOrWhiteSpace(enumMember?.Value)) return enumMember!.Value!;

            return name;
        }

        // Dùng cho enum nullable
        public static string? GetDisplayNameOrNull<TEnum>(this TEnum? value)
            where TEnum : struct, Enum
            => value.HasValue ? value.Value.GetDisplayName() : null;
        public static IEnumerable<SelectListItem> ToSelectList<TEnum>() where TEnum : struct, Enum => Enum.GetValues(typeof(TEnum))
           .Cast<TEnum>()
           .Select(v => new SelectListItem
           {
               Value = v.ToString(),
               Text = v.GetDisplayName()
           });
    }
}
