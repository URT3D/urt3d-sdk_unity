using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing API integration functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for API integration
        /// </summary>
        private void RegisterApiIntegrationFunctions()
        {
            // HTTP GET request
            var httpGet = Intrinsic.Create("httpGet");
            httpGet.AddParam("url");
            httpGet.AddParam("headers", string.Empty);
            httpGet.code = (context, _) => {
                var url = context.GetVar("url").ToString();
                var headers = context.GetVar("headers").ToString();

                Debug.Log($"[MiniScript] Making HTTP GET request to {url}");

                // Return placeholder response
                var response = new ValMap
                {
                    ["success"] = ValNumber.one,
                    ["status"] = new ValNumber(200),
                    ["data"] = new ValString("{}")
                };

                return new Intrinsic.Result(response);
            };
            _registeredIntrinsics.Add(httpGet);

            // HTTP POST request
            var httpPost = Intrinsic.Create("httpPost");
            httpPost.AddParam("url");
            httpPost.AddParam("data");
            httpPost.AddParam("headers", string.Empty);
            httpPost.code = (context, _) => {
                var url = context.GetVar("url").ToString();
                var data = context.GetVar("data").ToString();
                var headers = context.GetVar("headers").ToString();

                Debug.Log($"[MiniScript] Making HTTP POST request to {url} with data {data}");

                // Return placeholder response
                var response = new ValMap
                {
                    ["success"] = ValNumber.one,
                    ["status"] = new ValNumber(200),
                    ["data"] = new ValString("{}")
                };

                return new Intrinsic.Result(response);
            };
            _registeredIntrinsics.Add(httpPost);

            // Parse JSON
            var parseJson = Intrinsic.Create("parseJson");
            parseJson.AddParam("jsonString");
            parseJson.code = (context, _) => {
                var jsonString = context.GetVar("jsonString").ToString();

                Debug.Log($"[MiniScript] Parsing JSON: {jsonString}");

                // Return placeholder parsed object
                var parsedObj = new ValMap
                {
                    ["parsed"] = ValNumber.one
                };

                return new Intrinsic.Result(parsedObj);
            };
            _registeredIntrinsics.Add(parseJson);

            // Stringify JSON
            var stringifyJson = Intrinsic.Create("stringifyJson");
            stringifyJson.AddParam("object");
            stringifyJson.code = (context, _) => {
                var obj = context.GetVar("object");

                Debug.Log("[MiniScript] Converting object to JSON string");

                // Return placeholder JSON string
                return new Intrinsic.Result(new ValString("{}"));
            };
            _registeredIntrinsics.Add(stringifyJson);

            // Store data in cloud
            var cloudSave = Intrinsic.Create("cloudSave");
            cloudSave.AddParam("key");
            cloudSave.AddParam("data");
            cloudSave.code = (context, _) => {
                var key = context.GetVar("key").ToString();
                var data = context.GetVar("data");

                Debug.Log($"[MiniScript] Saving data to cloud with key {key}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(cloudSave);

            // Load data from cloud
            var cloudLoad = Intrinsic.Create("cloudLoad");
            cloudLoad.AddParam("key");
            cloudLoad.code = (context, _) => {
                var key = context.GetVar("key").ToString();

                Debug.Log($"[MiniScript] Loading data from cloud with key {key}");

                // Return placeholder loaded data
                var loadedData = new ValMap
                {
                    ["loaded"] = ValNumber.one,
                    ["timestamp"] = new ValNumber(1620000000)
                };

                return new Intrinsic.Result(loadedData);
            };
            _registeredIntrinsics.Add(cloudLoad);
        }
    }
}
