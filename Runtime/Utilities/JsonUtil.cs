using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urt3d.Scripting;
using Urt3d.Traits;
using Urt3d.Utilities.Json;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Utility class for JSON serialization and deserialization operations.
    /// Provides wrapper methods for Newtonsoft.Json functionality with standardized settings.
    /// </summary>
    public static class JsonUtil
    {
        /// <summary>
        /// Deserializes a JSON string into a new object of type T.
        /// Uses Newtonsoft.Json with standardized settings for consistent behavior.
        /// </summary>
        /// <param name="json">The JSON string to deserialize</param>
        /// <typeparam name="T">The type to deserialize the JSON into</typeparam>
        /// <returns>A new instance of type T populated with values from the JSON string</returns>
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        /// <summary>
        /// Deserializes a JToken into an existing object instance.
        /// Converts the JToken to a string and delegates to the string-based FromJson method.
        /// </summary>
        /// <param name="json">The JToken containing JSON data to deserialize</param>
        /// <param name="obj">The target object to populate with values</param>
        public static void FromJson(JToken json, object obj)
        {
            FromJson(json.ToString(), obj);
        }

        /// <summary>
        /// Deserializes JSON string into an existing object instance.
        /// Populates the target object with values from the JSON string.
        /// </summary>
        /// <param name="json">The JSON string to deserialize</param>
        /// <param name="obj">The target object to populate with values</param>
        public static void FromJson(string json, object obj)
        {
            try
            {
                switch (obj)
                {
                    case Asset asset:
                        ConverterAsset.Instance.ReadJson(json, asset);
                        break;
                    case Trait trait:
                        ConverterTrait.Instance.ReadJson(json, trait);
                        break;
                    case Script script:
                        ConverterScript.Instance.ReadJson(json, script);
                        break;
                    case Param param:
                        ConverterParam.Instance.ReadJson(json, param);
                        break;
                    case Metadata metadata:
                        ConverterMetadata.Instance.ReadJson(json, metadata);
                        break;
                    default:
                        JsonConvert.PopulateObject(json, obj);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        /// <summary>
        /// Serializes an object to a JSON string with indented formatting.
        /// Uses Newtonsoft.Json with settings to handle reference loops and pretty formatting.
        /// </summary>
        /// <param name="obj">The object to serialize to JSON</param>
        /// <returns>A formatted JSON string representation of the object</returns>
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        /// <summary>
        /// Gets the standardized JsonSerializerSettings used across all serialization operations.
        /// Includes custom converters for Urt3d types and consistent formatting settings.
        /// Lazy-initializes the settings on first access.
        /// </summary>
        private static JsonSerializerSettings Settings
        {
            get
            {
                if (s_settings == null)
                {
                    s_settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    s_settings.Converters.Add(new ConverterAsset());
                    s_settings.Converters.Add(new ConverterParam());
                    s_settings.Converters.Add(new ConverterTrait());
                    s_settings.Converters.Add(new ConverterScript());
                    s_settings.Converters.Add(new ConverterMetadata());
                }

                return s_settings;
            }
        }
        private static JsonSerializerSettings s_settings;
    }
}
