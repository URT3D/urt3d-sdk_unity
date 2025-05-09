using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities.Json
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Custom JSON converter for handling Metadata objects and their deserialization/serialization.
    /// Manages the proper conversion between JSON and Metadata objects in the Urt3d SDK.
    /// </summary>
    public class ConverterMetadata : Converter<Metadata>
    {
        /// <summary>
        /// Gets the singleton instance of the ConverterMetadata.
        /// Ensures only one instance of the converter exists throughout the application.
        /// </summary>
        public static ConverterMetadata Instance
        {
            get
            {
                s_self ??= new ConverterMetadata();
                return s_self;
            }
        }
        private static ConverterMetadata s_self;

        /// <summary>
        /// Reads JSON and converts it to a Metadata object.
        /// Handles explicit property mapping to match the WriteJson method.
        /// </summary>
        /// <param name="reader">The JsonReader to read from</param>
        /// <param name="objectType">Type of the Metadata object</param>
        /// <param name="value">Existing Metadata value, if any</param>
        /// <param name="hasExistingValue">Indicates if an existing value was provided</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        /// <returns>The deserialized Metadata object</returns>
        public override Metadata ReadJson(JsonReader reader, Type objectType, Metadata value, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // If no existing value, return null as we can't construct a Metadata directly
            if (!hasExistingValue || value == null)
            {
                Log.Error("Cannot deserialize to Metadata. An existing instance is required.");
                return null;
            }

            // TODO GuidDefinition
            
            //
            var guidInstanceToken = jObject[PROPERTY_GUID_INSTANCE];
            if (guidInstanceToken != null && guidInstanceToken.Type != JTokenType.Null)
            {
                if (Guid.TryParse(guidInstanceToken.ToString(), out var guidInstance))
                {
                    value.GuidInstance = guidInstance;
                }
            }

            //
            var nameInstanceToken = jObject[PROPERTY_NAME_INSTANCE];
            if (nameInstanceToken != null && nameInstanceToken.Type != JTokenType.Null)
            {
                var nameInstance = nameInstanceToken.ToString();
                if (!string.IsNullOrEmpty(nameInstance))
                {
                    value.NameInstance = nameInstance;
                }
            }

            return value;
        }

        /// <summary>
        /// Writes a Metadata object to JSON.
        /// </summary>
        /// <param name="writer">The JsonWriter to write to</param>
        /// <param name="value">The Metadata object to serialize</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        public override void WriteJson(JsonWriter writer, Metadata value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var jo = new JObject
            {
                { PROPERTY_GUID_DEFINITION, value.GuidDefinition.ToString() },
                { PROPERTY_GUID_INSTANCE, value.GuidInstance.ToString() },
                { PROPERTY_NAME_INSTANCE, value.NameInstance }
            };

            jo.WriteTo(writer);
        }
    }
}
