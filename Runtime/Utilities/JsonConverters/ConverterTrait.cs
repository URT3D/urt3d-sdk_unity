using System;
using Newtonsoft.Json;
using Urt3d.Traits;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities.Json
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Custom JSON converter for handling Trait objects and their deserialization/serialization.
    /// Manages the proper conversion between JSON and Trait objects in the Urt3d SDK.
    /// </summary>
    public class ConverterTrait : Converter<Trait>
    {
        /// <summary>
        /// Gets the singleton instance of the ConverterTrait.
        /// Ensures only one instance of the converter exists throughout the application.
        /// </summary>
        public static ConverterTrait Instance
        {
            get
            {
                s_self ??= new ConverterTrait();
                return s_self;
            }
        }
        private static ConverterTrait s_self;

        /// <summary>
        /// Reads JSON and converts it to a Trait object.
        /// </summary>
        /// <param name="reader">The JsonReader to read from</param>
        /// <param name="objectType">Type of the Trait object</param>
        /// <param name="value">Existing Trait value, if any</param>
        /// <param name="hasExistingValue">Indicates if an existing value was provided</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        /// <returns>The deserialized Trait object</returns>
        public override Trait ReadJson(JsonReader reader, Type objectType, Trait value, bool hasExistingValue, JsonSerializer serializer)
        {
            // If no existing value, construct one
            if (!hasExistingValue || value == null)
            {
                value = Activator.CreateInstance(objectType) as Trait;
            }

            return ConverterParam.Instance.ReadJson(reader, objectType, value, hasExistingValue, serializer) as Trait;
        }

        /// <summary>
        /// Writes a Trait object to JSON.
        /// </summary>
        /// <param name="writer">The JsonWriter to write to</param>
        /// <param name="value">The Trait object to serialize</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        public override void WriteJson(JsonWriter writer, Trait value, JsonSerializer serializer)
        {
            ConverterParam.Instance.WriteJson(writer, value, serializer);
        }
    }
}
