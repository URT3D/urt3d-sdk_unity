using System.IO;
using Newtonsoft.Json;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities.Json
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Base abstract class for all custom JSON converters in the Urt3d SDK.
    /// Extends Newtonsoft.Json.JsonConverter for specific types.
    /// </summary>
    /// <typeparam name="T">The type this converter handles</typeparam>
    public abstract class Converter<T> : JsonConverter<T>
    {
        /// <summary>
        /// Reads JSON string and populates an object with the deserialized data.
        /// Used as a workaround since JsonConvert.PopulateObject doesn't support custom JsonConverters.
        /// </summary>
        /// <param name="json">The JSON string to deserialize</param>
        /// <param name="obj">The object to populate with deserialized data</param>
        public void ReadJson(string json, T obj)
        {
            using var readerStr = new StringReader(json);
            using var readerJson = new JsonTextReader(readerStr);
            var serializer = JsonSerializer.Create(new JsonSerializerSettings());
            ReadJson(readerJson, typeof(T), obj, true, serializer);
        }

        #region Protected Constants

        // Common property names
        protected const string PROPERTY_GUID            = "Guid";
        protected const string PROPERTY_GUID_DEFINITION = "GuidDefinition";
        protected const string PROPERTY_GUID_INSTANCE   = "GuidInstance";
        protected const string PROPERTY_NAME            = "Name";
        protected const string PROPERTY_NAME_DEFINITION = "NameDefinition";
        protected const string PROPERTY_NAME_INSTANCE   = "NameInstance";
        protected const string PROPERTY_TYPE            = "Type";
        protected const string PROPERTY_TYPE_DEFINITION = "TypeDefinition";
        protected const string PROPERTY_VALUE           = "Value";

        // Property names for Asset
        protected const string PROPERTY_METADATA = "Metadata";
        protected const string PROPERTY_TRAITS   = "Traits";
        protected const string PROPERTY_SCRIPTS  = "Scripts";

        // Fallback values
        protected const string UNKNOWN_STRING = "Unknown";

        #endregion
    }
}
