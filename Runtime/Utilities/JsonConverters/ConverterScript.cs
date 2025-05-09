using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urt3d.Scripting;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities.Json
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// 
    /// </summary>
    public class ConverterScript : Converter<Script>
    {
        /// <summary>
        /// Gets the singleton instance of the ConverterEffect.
        /// Ensures only one instance of the converter exists throughout the application.
        /// </summary>
        public static ConverterScript Instance
        {
            get
            {
                s_self ??= new ConverterScript();
                return s_self;
            }
        }
        private static ConverterScript s_self;

        /// <summary>
        /// Reads JSON and converts it to an Effect object.
        /// </summary>
        /// <param name="reader">The JsonReader to read from</param>
        /// <param name="objectType">Type of the Effect object</param>
        /// <param name="value">Existing Effect value, if any</param>
        /// <param name="hasExistingValue">Indicates if an existing value was provided</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        /// <returns>The deserialized Effect object</returns>
        public override Script ReadJson(JsonReader reader, Type objectType, Script value, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // If no existing value, construct one
            if (!hasExistingValue || value == null)
            {
                value = new Script();
            }

            // Deserialize ScriptId
            var scriptIdToken = jObject[PROPERTY_GUID];
            if (scriptIdToken != null && scriptIdToken.Type != JTokenType.Null)
            {
                var guidStr = scriptIdToken.ToString();
                if (Guid.TryParse(guidStr, out var guid))
                {
                    value.Guid = guid;
                }
            }

            // Deserialize Name
            var nameToken = jObject[PROPERTY_NAME];
            if (nameToken != null && nameToken.Type != JTokenType.Null)
            {
                value.Name = nameToken.ToString();
            }

            // Deserialize Enabled
            var enabledToken = jObject[PROPERTY_ENABLED];
            if (enabledToken != null && enabledToken.Type != JTokenType.Null)
            {
                value.Enabled = enabledToken.ToObject<bool>();
            }

            // Deserialize TriggerType
            var triggerTypeToken = jObject[PROPERTY_TRIGGER_TYPE];
            if (triggerTypeToken != null && triggerTypeToken.Type != JTokenType.Null)
            {
                value.TriggerType = triggerTypeToken.ToObject<ScriptTriggerType>();
            }

            // Deserialize CustomEventName
            var customEventNameToken = jObject[PROPERTY_CUSTOM_EVENT_NAME];
            if (customEventNameToken != null && customEventNameToken.Type != JTokenType.Null)
            {
                value.CustomEventName = customEventNameToken.ToString();
            }

            // Deserialize ScriptContent
            var scriptContentToken = jObject[PROPERTY_SCRIPT_CONTENT];
            if (scriptContentToken != null && scriptContentToken.Type != JTokenType.Null)
            {
                value.ScriptContent = scriptContentToken.ToString();
            }

            return value;
        }

        /// <summary>
        /// Writes an Effect object to JSON.
        /// Serializes the name, GUID, parameters, outputs, and output tooltips.
        /// </summary>
        /// <param name="writer">The JsonWriter to write to</param>
        /// <param name="value">The Effect object to serialize</param>
        /// <param name="serializer">The calling JsonSerializer</param>
        public override void WriteJson(JsonWriter writer, Script value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            {
                // Write ScriptId
                writer.WritePropertyName(PROPERTY_GUID);
                writer.WriteValue(value.Guid);

                // Write Name
                writer.WritePropertyName(PROPERTY_NAME);
                writer.WriteValue(value.Name);

                // Write Enabled
                writer.WritePropertyName(PROPERTY_ENABLED);
                writer.WriteValue(value.Enabled);

                // Write TriggerType
                writer.WritePropertyName(PROPERTY_TRIGGER_TYPE);
                writer.WriteValue((int)value.TriggerType);

                // Write CustomEventName
                writer.WritePropertyName(PROPERTY_CUSTOM_EVENT_NAME);
                writer.WriteValue(value.CustomEventName);

                // Write ScriptContent
                writer.WritePropertyName(PROPERTY_SCRIPT_CONTENT);
                writer.WriteValue(value.ScriptContent);
            }
            writer.WriteEndObject();
        }

        #region Constants

        private const string PROPERTY_ENABLED           = "Enabled";
        private const string PROPERTY_TRIGGER_TYPE      = "TriggerType";
        private const string PROPERTY_CUSTOM_EVENT_NAME = "CustomEventName";
        private const string PROPERTY_SCRIPT_CONTENT    = "ScriptContent";

        #endregion
    }
}
