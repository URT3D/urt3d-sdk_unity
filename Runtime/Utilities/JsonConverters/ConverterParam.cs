using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities.Json
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Custom JSON converter for handling Param objects and their deserialization/serialization.
    /// Manages the proper conversion between JSON and Param objects in the Urt3d SDK.
    /// </summary>
    public class ConverterParam : Converter<Param>
    {
        /// <summary>
        /// Gets the singleton instance of the ConverterParam.
        /// Ensures only one instance of the converter exists throughout the application.
        /// </summary>
        public static ConverterParam Instance
        {
            get
            {
                s_self ??= new ConverterParam();
                return s_self;
            }
        }
        private static ConverterParam s_self;

        /// <summary>
        /// Reads JSON and converts it to a Param object.
        /// </summary>
        /// <param name="reader">The JsonReader to read from</param>
        /// <param name="objectType">Type of the Param object</param>
        /// <param name="value">Existing Param value, if any</param>
        /// <param name="hasExistingValue">Indicates if an existing value was provided</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        /// <returns>The deserialized Param object</returns>
        public override Param ReadJson(JsonReader reader, Type objectType, Param value, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // If no existing value, construct one
            if (!hasExistingValue || value == null)
            {
                value = Activator.CreateInstance(objectType) as Param;
            }

            // Check if we have a value already
            if (value != null)
            {
                try
                {
                    // Get the Value token from JSON
                    var valueToken = jObject[PROPERTY_VALUE];
                    if (valueToken != null && valueToken.Type != JTokenType.Null)
                    {
                        // Determine the type to convert to
                        var valueType = value.Type;
                        if (valueType != null)
                        {
                            // Convert the JSON token to the appropriate type
                            var typedValue = valueToken.ToObject(valueType, serializer);

                            // Set the value
                            value.ValueObject = typedValue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error deserializing Param: {ex.Message}");
                }
            }

            return value;
        }

        /// <summary>
        /// Writes a Param object to JSON.
        /// </summary>
        /// <param name="writer">The JsonWriter to write to</param>
        /// <param name="value">The Param object to serialize</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        public override void WriteJson(JsonWriter writer, Param value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var jo = new JObject
            {
                { PROPERTY_GUID, value.Guid },
                { PROPERTY_VALUE, value.ValueObject == null ? null : JToken.FromObject(value.ValueObject, serializer) }
            };

            jo.WriteTo(writer);
        }
    }
}
