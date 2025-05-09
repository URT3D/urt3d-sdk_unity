using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing material and visual effect functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for material and visual effects
        /// </summary>
        private void RegisterVisualFunctions()
        {
            // Set material color
            var setColor = Intrinsic.Create("setColor");
            setColor.AddParam("r", 1.0);
            setColor.AddParam("g", 1.0);
            setColor.AddParam("b", 1.0);
            setColor.AddParam("a", 1.0);
            setColor.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var r = (float)context.GetVar("r").DoubleValue();
                var g = (float)context.GetVar("g").DoubleValue();
                var b = (float)context.GetVar("b").DoubleValue();
                var a = (float)context.GetVar("a").DoubleValue();

                Debug.Log($"[MiniScript] Setting color to ({r}, {g}, {b}, {a})");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setColor);

            // Get material color
            var getColor = Intrinsic.Create("getColor");
            getColor.code = (_, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                Debug.Log("[MiniScript] Getting color");

                // Return placeholder color
                var colorMap = new ValMap
                {
                    ["r"] = new ValNumber(1),
                    ["g"] = new ValNumber(1),
                    ["b"] = new ValNumber(1),
                    ["a"] = new ValNumber(1)
                };

                return new Intrinsic.Result(colorMap);
            };
            _registeredIntrinsics.Add(getColor);

            // Set material texture
            var setTexture = Intrinsic.Create("setTexture");
            setTexture.AddParam("textureId");
            setTexture.AddParam("materialSlot", 0);
            setTexture.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var textureId = context.GetVar("textureId").ToString();
                var materialSlot = (int)context.GetVar("materialSlot").DoubleValue();

                Debug.Log($"[MiniScript] Setting texture {textureId} to slot {materialSlot}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setTexture);

            // Set material property
            var setMaterialProperty = Intrinsic.Create("setMaterialProperty");
            setMaterialProperty.AddParam("propertyName");
            setMaterialProperty.AddParam("value");
            setMaterialProperty.AddParam("materialIndex", 0);
            setMaterialProperty.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var propertyName = context.GetVar("propertyName").ToString();
                var value = context.GetVar("value");
                var materialIndex = (int)context.GetVar("materialIndex").DoubleValue();

                Debug.Log($"[MiniScript] Setting material property {propertyName} to {value} on material index {materialIndex}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setMaterialProperty);

            // Play particle effect
            var playParticles = Intrinsic.Create("playParticles");
            playParticles.AddParam("effectName", "");
            playParticles.AddParam("duration", 5.0);
            playParticles.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var effectName = context.GetVar("effectName").ToString();
                var duration = (float)context.GetVar("duration").DoubleValue();

                Debug.Log($"[MiniScript] Playing particle effect {effectName} for {duration} seconds");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(playParticles);

            // Stop particle effect
            var stopParticles = Intrinsic.Create("stopParticles");
            stopParticles.AddParam("effectName", "");
            stopParticles.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var effectName = context.GetVar("effectName").ToString();

                Debug.Log(string.IsNullOrEmpty(effectName)
                    ? "[MiniScript] Stopping all particle effects"
                    : $"[MiniScript] Stopping particle effect {effectName}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(stopParticles);
        }
    }
}
