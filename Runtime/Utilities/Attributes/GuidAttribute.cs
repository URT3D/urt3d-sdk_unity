using System;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Attribute that provides a stable GUID identifier for a class.
    /// Used throughout the Urt3d SDK to assign unique identifiers to Causes, Effects, and Traits.
    /// </summary>
    public class GuidAttribute : Attribute
    {
        private Guid Value { get; }

        /// <summary>
        /// Creates a new GuidAttribute with the specified GUID string.
        /// </summary>
        /// <param name="guid">A string representation of a GUID, e.g. "00000000-0000-0000-0000-000000000000"</param>
        public GuidAttribute(string guid)
        {
            Value = new Guid(guid);
        }

        /// <summary>
        /// Gets the GUID associated with the specified type.
        /// </summary>
        /// <param name="type">The type to get the GUID for</param>
        /// <returns>The GUID associated with the type, or Guid.Empty if not found</returns>
        public static Guid Get(Type type)
        {
            TryGet(type, out var val);
            return val;
        }

        /// <summary>
        /// Tries to get the GUID associated with the specified type.
        /// </summary>
        /// <param name="type">The type to get the GUID for</param>
        /// <param name="guid">When this method returns, contains the GUID associated with the type,
        /// or Guid.Empty if no GUID was found</param>
        /// <returns>true if the GUID was found; otherwise, false</returns>
        public static bool TryGet(Type type, out Guid guid)
        {
            var attribute = GetAttribute(type);
            if (attribute == null || attribute.Value == Guid.Empty)
            {
                guid = Guid.Empty;
                return false;
            }

            guid = attribute.Value;
            return true;
        }

        /// <summary>
        /// Gets the GuidAttribute from the specified type.
        /// </summary>
        /// <param name="type">The type to get the attribute from</param>
        /// <returns>The GuidAttribute instance, or null if not found</returns>
        private static GuidAttribute GetAttribute(ICustomAttributeProvider type)
        {
            var attributes = type.GetCustomAttributes(inherit: false);
            foreach (var attribute in attributes)
            {
                if (attribute is GuidAttribute value) return value;
            }
            return null;
        }
    }
}
