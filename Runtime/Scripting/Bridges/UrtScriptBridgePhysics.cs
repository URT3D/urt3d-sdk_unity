using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing physics functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for physics interactions
        /// </summary>
        private void RegisterPhysicsFunctions()
        {
            // Add force to asset
            var addForce = Intrinsic.Create("addForce");
            addForce.AddParam("x", 0.0);
            addForce.AddParam("y", 0.0);
            addForce.AddParam("z", 0.0);
            addForce.AddParam("mode", "force");  // force, impulse, acceleration, etc.
            addForce.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();
                var mode = context.GetVar("mode").ToString();

                Debug.Log($"[MiniScript] Adding {mode} ({x}, {y}, {z}) to asset");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(addForce);

            // Set velocity
            var setVelocity = Intrinsic.Create("setVelocity");
            setVelocity.AddParam("x", 0.0);
            setVelocity.AddParam("y", 0.0);
            setVelocity.AddParam("z", 0.0);
            setVelocity.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                Debug.Log($"[MiniScript] Setting velocity to ({x}, {y}, {z})");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setVelocity);

            // Get velocity
            var getVelocity = Intrinsic.Create("getVelocity");
            getVelocity.code = (_, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                Debug.Log("[MiniScript] Getting velocity");

                // Return placeholder velocity vector
                var velocityMap = new ValMap
                {
                    ["x"] = new ValNumber(0),
                    ["y"] = new ValNumber(0),
                    ["z"] = new ValNumber(0),
                    ["magnitude"] = new ValNumber(0)
                };

                return new Intrinsic.Result(velocityMap);
            };
            _registeredIntrinsics.Add(getVelocity);

            // Raycast from asset
            var raycast = Intrinsic.Create("raycast");
            raycast.AddParam("dirX", 0.0);
            raycast.AddParam("dirY", 0.0);
            raycast.AddParam("dirZ", 1.0);
            raycast.AddParam("maxDistance", 100.0);
            raycast.AddParam("layerMask", "all");
            raycast.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var dirX = (float)context.GetVar("dirX").DoubleValue();
                var dirY = (float)context.GetVar("dirY").DoubleValue();
                var dirZ = (float)context.GetVar("dirZ").DoubleValue();
                var maxDistance = (float)context.GetVar("maxDistance").DoubleValue();
                var layerMask = context.GetVar("layerMask").ToString();

                Debug.Log($"[MiniScript] Raycasting in direction ({dirX}, {dirY}, {dirZ}) with maxDistance {maxDistance}");

                // Return placeholder hit result
                var hitResult = new ValMap
                {
                    ["hit"] = ValNumber.zero, // No hit for placeholder
                    ["distance"] = new ValNumber(0),
                    ["point"] = new ValMap {
                        ["x"] = new ValNumber(0),
                        ["y"] = new ValNumber(0),
                        ["z"] = new ValNumber(0)
                    },
                    ["normal"] = new ValMap {
                        ["x"] = new ValNumber(0),
                        ["y"] = new ValNumber(0),
                        ["z"] = new ValNumber(0)
                    }
                };

                return new Intrinsic.Result(hitResult);
            };
            _registeredIntrinsics.Add(raycast);

            // Set gravity scale
            var setGravity = Intrinsic.Create("setGravity");
            setGravity.AddParam("scale", 1.0);
            setGravity.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var scale = (float)context.GetVar("scale").DoubleValue();

                Debug.Log($"[MiniScript] Setting gravity scale to {scale}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setGravity);

            // Set collision detection
            var setCollision = Intrinsic.Create("setCollision");
            setCollision.AddParam("enabled", ValNumber.one); // true
            setCollision.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var enabled = context.GetVar("enabled").BoolValue();

                Debug.Log($"[MiniScript] Setting collision detection to {enabled}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setCollision);
        }
    }
}
