using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing camera control functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for camera control
        /// </summary>
        private void RegisterCameraFunctions()
        {
            // Get main camera
            var getMainCamera = Intrinsic.Create("getMainCamera");
            getMainCamera.code = (_, _) => {
                Debug.Log("[MiniScript] Getting main camera");

                // Return placeholder camera info
                var cameraInfo = new ValMap { ["position"] = new ValMap {
                        ["x"] = new ValNumber(0),
                        ["y"] = new ValNumber(2),
                        ["z"] = new ValNumber(-10)
                    },
                    ["rotation"] = new ValMap {
                        ["x"] = new ValNumber(0),
                        ["y"] = new ValNumber(0),
                        ["z"] = new ValNumber(0)
                    },
                    ["fieldOfView"] = new ValNumber(60)
                };

                return new Intrinsic.Result(cameraInfo);
            };
            _registeredIntrinsics.Add(getMainCamera);

            // Set camera position
            var setCameraPosition = Intrinsic.Create("setCameraPosition");
            setCameraPosition.AddParam("x");
            setCameraPosition.AddParam("y");
            setCameraPosition.AddParam("z");
            setCameraPosition.AddParam("smooth", ValNumber.zero); // false
            setCameraPosition.AddParam("duration", 1.0);
            setCameraPosition.code = (context, _) => {
                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();
                var smooth = context.GetVar("smooth").BoolValue();
                var duration = (float)context.GetVar("duration").DoubleValue();

                Debug.Log(smooth
                    ? $"[MiniScript] Smoothly moving camera to ({x}, {y}, {z}) over {duration} seconds"
                    : $"[MiniScript] Setting camera position to ({x}, {y}, {z})");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setCameraPosition);

            // Set camera rotation
            var setCameraRotation = Intrinsic.Create("setCameraRotation");
            setCameraRotation.AddParam("x");
            setCameraRotation.AddParam("y");
            setCameraRotation.AddParam("z");
            setCameraRotation.AddParam("smooth", ValNumber.zero); // false
            setCameraRotation.AddParam("duration", 1.0);
            setCameraRotation.code = (context, _) => {
                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();
                var smooth = context.GetVar("smooth").BoolValue();
                var duration = (float)context.GetVar("duration").DoubleValue();

                Debug.Log(smooth
                    ? $"[MiniScript] Smoothly rotating camera to ({x}, {y}, {z}) over {duration} seconds"
                    : $"[MiniScript] Setting camera rotation to ({x}, {y}, {z})");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setCameraRotation);

            // Set camera target - look at
            var setCameraTarget = Intrinsic.Create("setCameraTarget");
            setCameraTarget.AddParam("targetAssetId", "");
            setCameraTarget.AddParam("x", 0.0);
            setCameraTarget.AddParam("y", 0.0);
            setCameraTarget.AddParam("z", 0.0);
            setCameraTarget.AddParam("offsetX", 0.0);
            setCameraTarget.AddParam("offsetY", 0.0);
            setCameraTarget.AddParam("offsetZ", 0.0);
            setCameraTarget.code = (context, _) => {
                var targetAssetId = context.GetVar("targetAssetId").ToString();
                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();
                var offsetX = (float)context.GetVar("offsetX").DoubleValue();
                var offsetY = (float)context.GetVar("offsetY").DoubleValue();
                var offsetZ = (float)context.GetVar("offsetZ").DoubleValue();

                Debug.Log(!string.IsNullOrEmpty(targetAssetId)
                    ? $"[MiniScript] Setting camera to follow asset {targetAssetId} with offset ({offsetX}, {offsetY}, {offsetZ})"
                    : $"[MiniScript] Setting camera to look at point ({x}, {y}, {z}) with offset ({offsetX}, {offsetY}, {offsetZ})");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setCameraTarget);

            // Set camera field of view
            var setCameraFOV = Intrinsic.Create("setCameraFOV");
            setCameraFOV.AddParam("fov");
            setCameraFOV.AddParam("smooth", ValNumber.zero); // false
            setCameraFOV.AddParam("duration", 1.0);
            setCameraFOV.code = (context, _) => {
                var fov = (float)context.GetVar("fov").DoubleValue();
                var smooth = context.GetVar("smooth").BoolValue();
                var duration = (float)context.GetVar("duration").DoubleValue();

                Debug.Log(smooth
                    ? $"[MiniScript] Smoothly changing camera FOV to {fov} over {duration} seconds"
                    : $"[MiniScript] Setting camera FOV to {fov}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setCameraFOV);

            // Set post-processing effect
            var setCameraEffect = Intrinsic.Create("setCameraEffect");
            setCameraEffect.AddParam("effectName");
            setCameraEffect.AddParam("value", 0.0);
            setCameraEffect.code = (context, _) => {
                var effectName = context.GetVar("effectName").ToString();
                var value = (float)context.GetVar("value").DoubleValue();

                Debug.Log($"[MiniScript] Setting camera effect {effectName} to {value}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setCameraEffect);
        }
    }
}
