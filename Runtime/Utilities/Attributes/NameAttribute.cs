using System;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Attribute that provides a display name for a class.
    /// Used throughout the Urt3d SDK to assign human-readable names to Causes, Effects, and Traits.
    /// </summary>
    public class NameAttribute : Attribute
    {
        private string Value { get; }

        /// <summary>
        /// Creates a new NameAttribute with the specified display name.
        /// </summary>
        /// <param name="name">The human-readable name for the class</param>
        public NameAttribute(string name)
        {
            Value = name;
        }

        /// <summary>
        /// Gets the display name associated with the specified type.
        /// </summary>
        /// <param name="type">The type to get the display name for</param>
        /// <returns>The display name associated with the type, or an empty string if not found</returns>
        public static string Get(Type type)
        {
            TryGet(type, out var val);
            return val;
        }

        /// <summary>
        /// Tries to get the display name associated with the specified type.
        /// </summary>
        /// <param name="type">The type to get the display name for</param>
        /// <param name="name">When this method returns, contains the display name associated with the type,
        /// or an empty string if no display name was found</param>
        /// <returns>true if the display name was found; otherwise, false</returns>
        public static bool TryGet(Type type, out string name)
        {
            var attribute = GetAttribute(type);
            if (attribute == null || string.IsNullOrEmpty(attribute.Value))
            {
                name = string.Empty;
                return false;
            }

            name = attribute.Value;
            return true;
        }

        /// <summary>
        /// Gets the NameAttribute from the specified type.
        /// </summary>
        /// <param name="type">The type to get the attribute from</param>
        /// <returns>The NameAttribute instance, or null if not found</returns>
        private static NameAttribute GetAttribute(ICustomAttributeProvider type)
        {
            var attributes = type.GetCustomAttributes(inherit: false);
            foreach (var attribute in attributes)
            {
                if (attribute is NameAttribute value) return value;
            }
            return null;
        }
    }
}
