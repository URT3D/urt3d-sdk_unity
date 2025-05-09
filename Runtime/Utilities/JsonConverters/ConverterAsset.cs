using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urt3d.Scripting;
using Urt3d.Traits;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities.Json
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Custom JsonConverter for Urt3d class to handle IReadOnlyCollection properties during deserialization.
    /// This converter properly handles the readonly collections like Traits, CauseInstances, and EffectInstances.
    /// </summary>
    public class ConverterAsset : Converter<Asset>
    {
        #region Public Methods

        /// <summary>
        /// Gets the singleton instance of the ConverterUrt3d.
        /// Ensures only one instance of the converter exists throughout the application.
        /// </summary>
        public static ConverterAsset Instance
        {
            get
            {
                s_self ??= new ConverterAsset();
                return s_self;
            }
        }
        private static ConverterAsset s_self;

        /// <summary>
        /// Reads JSON and converts it to a Urt3d object.
        /// </summary>
        /// <param name="reader">The JsonReader to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="value">The existing value of object being read.</param>
        /// <param name="hasExistingValue">True if existingValue has a value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The deserialized Urt3d object.</returns>
        public override Asset ReadJson(JsonReader reader, Type objectType, Asset value, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // If no existing value, we can't do anything since Urt3d is abstract
            if (!hasExistingValue || value == null)
            {
                throw new JsonSerializationException("Cannot deserialize to Urt3d. An existing instance is required.");
            }

            // Handle Metadata if needed
            ReadMetadata(jObject, value, serializer);

            // Read readonly collections
            ReadTraits(jObject, value, serializer);
            ReadScripts(jObject, value, serializer);

            return value;
        }

        /// <summary>
        /// Writes a Urt3d object to JSON.
        /// Only serializes Metadata, Traits, CauseInstances, and EffectInstances.
        /// </summary>
        /// <param name="writer">The JsonWriter to write to.</param>
        /// <param name="value">The Urt3d value to serialize.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, Asset value, JsonSerializer serializer)
        {
            // Create a new JObject with only the properties we want to serialize
            var jo = new JObject();

            // Add Metadata
            if (value.Metadata != null)
            {
                JToken metadataToken;
                using (var metadataWriter = new JTokenWriter())
                {
                    ConverterMetadata.Instance.WriteJson(metadataWriter, value.Metadata, serializer);
                    metadataToken = metadataWriter.Token;
                }
                jo[PROPERTY_METADATA] = metadataToken;
            }

            // Add Traits
            if (value.Traits is { Count: > 0 })
            {
                var traitsArray = new JArray();
                foreach (var trait in value.Traits)
                {
                    JToken traitToken;
                    using (var traitWriter = new JTokenWriter())
                    {
                        ConverterTrait.Instance.WriteJson(traitWriter, trait, serializer);
                        traitToken = traitWriter.Token;
                    }
                    traitsArray.Add(traitToken);
                }
                jo[PROPERTY_TRAITS] = traitsArray;
            }

            // Add Scripts
            if (value.Scripts is { Count: > 0 })
            {
                var scriptArray = new JArray();
                foreach (var script in value.Scripts)
                {
                    JToken scriptToken;
                    using (var scriptWriter = new JTokenWriter())
                    {
                        ConverterScript.Instance.WriteJson(scriptWriter, script, serializer);
                        scriptToken = scriptWriter.Token;
                    }
                    scriptArray.Add(scriptToken);
                }
                jo[PROPERTY_SCRIPTS] = scriptArray;
            }

            // Write the customized JObject to the output
            jo.WriteTo(writer);
        }

        #endregion

        #region Private Methods: Read JSON

        /// <summary>
        /// Reads the Metadata from JSON and populates the Urt3d object's metadata.
        /// </summary>
        /// <param name="jObject">The JSON object containing metadata</param>
        /// <param name="asset">The Urt3d object to populate</param>
        /// <param name="serializer">The JsonSerializer to use</param>
        private static void ReadMetadata(JObject jObject, Asset asset, JsonSerializer serializer)
        {
            // Handle Metadata which has a property with a private setter
            var metadataToken = jObject[PROPERTY_METADATA];
            if (metadataToken != null && asset.Metadata != null)
            {
                serializer.Populate(metadataToken.CreateReader(), asset.Metadata);
                jObject.Remove(PROPERTY_METADATA);
            }
        }

        /// <summary>
        /// Reads the Traits collection from JSON and updates the existing traits in the Urt3d object.
        /// Matches traits by GUID and updates their properties accordingly.
        /// </summary>
        /// <param name="jObject">The JSON object containing traits data</param>
        /// <param name="asset">The Urt3d object to update</param>
        /// <param name="serializer">The JsonSerializer to use</param>
        private static void ReadTraits(JObject jObject, Asset asset, JsonSerializer serializer)
        {
            // Check if we have Traits in the JSON
            var traitsToken = jObject[PROPERTY_TRAITS];
            if (traitsToken is { Type: JTokenType.Array })
            {
                // Create dictionary to track existing traits by Guid
                var existingTraitsByGuid = new Dictionary<Guid, Trait>();

                // Build dictionary of existing traits by Guid for lookup
                foreach (var trait in asset.Traits)
                {
                    // Add to Guid dictionary
                    if (!existingTraitsByGuid.ContainsKey(trait.Guid))
                    {
                        existingTraitsByGuid.Add(trait.Guid, trait);
                    }
                }

                // Process traits from JSON
                foreach (var traitToken in traitsToken)
                {
                    // Match by Guid only
                    var traitGuidStr = traitToken[PROPERTY_GUID]?.ToString();
                    var traitName = traitToken[PROPERTY_NAME]?.ToString(); // Still needed for error messages
                    Trait existingTrait = null;

                    if (!string.IsNullOrEmpty(traitGuidStr) && Guid.TryParse(traitGuidStr, out var traitGuid))
                    {
                        existingTraitsByGuid.TryGetValue(traitGuid, out existingTrait);
                    }

                    if (existingTrait != null)
                    {
                        // Found matching trait - update it
                        try
                        {
                            // Update the existing trait's properties
                            serializer.Populate(traitToken.CreateReader(), existingTrait);
                        }
                        catch (Exception ex)
                        {
                            Log.Warning($"Failed to update trait '{traitName}': {ex.Message}");
                        }
                    }
                    else
                    {
                        // Trait not found in existing collection - just log a warning
                        var guidText = traitGuidStr ?? UNKNOWN_STRING;
                        Log.Warning($"Trait with Guid '{guidText}' not found in existing collection. Skipping.");
                    }
                }

                // Remove the traits from the JObject so they don't get processed again
                jObject.Remove(PROPERTY_TRAITS);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="asset"></param>
        /// <param name="serializer"></param>
        private static void ReadScripts(JObject jObject, Asset asset, JsonSerializer serializer)
        {
            // Check if we have ScriptInstances in the JSON
            var scriptsToken = jObject[PROPERTY_SCRIPTS];
            if (scriptsToken is not { Type: JTokenType.Array })
            {
                return;
            }

            // Clear existing script instances
            foreach (var script in asset.Scripts)
            {
                asset.RemoveScript(script);
            }

            // Add new script instances from JSON
            foreach (var scriptToken in scriptsToken)
            {
                // Deserialize the script reference
                var scriptReference = scriptToken.ToObject<Script>(serializer);
                if (scriptReference == null)
                {
                    Logger.Warning($"Failed to deserialize Script from JSON: {scriptToken}");
                    continue;
                }

                // Add instance to URT3D
                asset.AddScript(scriptReference);
            }

            // Remove the scripts from the JObject so they don't get processed again
            jObject.Remove(PROPERTY_SCRIPTS);
        }

        #endregion
    }
}
