using System.Collections.Generic;
using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing scene management functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for scene management
        /// </summary>
        private void RegisterSceneManagementFunctions()
        {
            // Load scene
            var loadScene = Intrinsic.Create("loadScene");
            loadScene.AddParam("sceneName");
            loadScene.AddParam("additive", ValNumber.zero); // false
            loadScene.code = (context, _) => {
                var sceneName = context.GetVar("sceneName").ToString();
                var additive = context.GetVar("additive").BoolValue();

                Debug.Log($"[MiniScript] Loading scene {sceneName} with additive={additive}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(loadScene);

            // Unload scene
            var unloadScene = Intrinsic.Create("unloadScene");
            unloadScene.AddParam("sceneName");
            unloadScene.code = (context, _) => {
                var sceneName = context.GetVar("sceneName").ToString();

                Debug.Log($"[MiniScript] Unloading scene {sceneName}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(unloadScene);

            // Get active scene
            var getActiveScene = Intrinsic.Create("getActiveScene");
            getActiveScene.code = (_, _) => {
                Debug.Log("[MiniScript] Getting active scene");

                // Return placeholder scene info
                var sceneInfo = new ValMap
                {
                    ["name"] = new ValString("MainScene"),
                    ["path"] = new ValString("Assets/Scenes/MainScene.unity"),
                    ["buildIndex"] = new ValNumber(0)
                };

                return new Intrinsic.Result(sceneInfo);
            };
            _registeredIntrinsics.Add(getActiveScene);

            // Get loaded scenes
            var getLoadedScenes = Intrinsic.Create("getLoadedScenes");
            getLoadedScenes.code = (_, _) => {
                Debug.Log("[MiniScript] Getting loaded scenes");

                // Return placeholder scene list using array constructor
                var scene1 = new ValMap
                {
                    ["name"] = new ValString("MainScene"),
                    ["path"] = new ValString("Assets/Scenes/MainScene.unity"),
                    ["buildIndex"] = new ValNumber(0)
                };

                var scene2 = new ValMap
                {
                    ["name"] = new ValString("UIScene"),
                    ["path"] = new ValString("Assets/Scenes/UIScene.unity"),
                    ["buildIndex"] = new ValNumber(1)
                };

                var scenes = new List<Value> { scene1, scene2 };
                var sceneList = new ValList(scenes);

                return new Intrinsic.Result(sceneList);
            };
            _registeredIntrinsics.Add(getLoadedScenes);

            // Set scene lighting
            var setSceneLighting = Intrinsic.Create("setSceneLighting");
            setSceneLighting.AddParam("intensity", 1.0);
            setSceneLighting.AddParam("color", "");
            setSceneLighting.code = (context, _) => {
                var intensity = (float)context.GetVar("intensity").DoubleValue();
                var color = context.GetVar("color").ToString();

                Debug.Log(string.IsNullOrEmpty(color)
                    ? $"[MiniScript] Setting scene lighting intensity to {intensity}"
                    : $"[MiniScript] Setting scene lighting intensity to {intensity} with color {color}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setSceneLighting);
        }
    }
}
