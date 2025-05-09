using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing advanced transform functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register advanced transform functions
        /// </summary>
        private void RegisterAdvancedTransformFunctions()
        {
            // Move asset toward a point
            var moveToward = Intrinsic.Create("moveToward");
            moveToward.AddParam("x");
            moveToward.AddParam("y");
            moveToward.AddParam("z");
            moveToward.AddParam("speed", 1.0);
            moveToward.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();
                var speed = (float)context.GetVar("speed").DoubleValue();

                Debug.Log($"[MiniScript] Moving asset toward ({x}, {y}, {z}) with speed {speed}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(moveToward);

            // Rotate toward a point
            var rotateToward = Intrinsic.Create("rotateToward");
            rotateToward.AddParam("x");
            rotateToward.AddParam("y");
            rotateToward.AddParam("z");
            rotateToward.AddParam("speed", 1.0);
            rotateToward.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();
                var speed = (float)context.GetVar("speed").DoubleValue();

                Debug.Log($"[MiniScript] Rotating asset toward ({x}, {y}, {z}) with speed {speed}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(rotateToward);

            // Look at a position
            var lookAt = Intrinsic.Create("lookAt");
            lookAt.AddParam("x");
            lookAt.AddParam("y");
            lookAt.AddParam("z");
            lookAt.AddParam("smooth", ValNumber.zero); // false
            lookAt.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();
                var smooth = context.GetVar("smooth").BoolValue();

                Debug.Log($"[MiniScript] Looking at position ({x}, {y}, {z}) with smooth={smooth}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(lookAt);

            // Get direction to another position
            var getDirectionTo = Intrinsic.Create("getDirectionTo");
            getDirectionTo.AddParam("x");
            getDirectionTo.AddParam("y");
            getDirectionTo.AddParam("z");
            getDirectionTo.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                Debug.Log($"[MiniScript] Getting direction to ({x}, {y}, {z})");

                // Return placeholder direction vector
                var directionMap = new ValMap
                {
                    ["x"] = new ValNumber(0),
                    ["y"] = new ValNumber(0),
                    ["z"] = new ValNumber(1),
                    ["magnitude"] = new ValNumber(1)
                };

                return new Intrinsic.Result(directionMap);
            };
            _registeredIntrinsics.Add(getDirectionTo);

            // Get distance to another position
            var getDistanceTo = Intrinsic.Create("getDistanceTo");
            getDistanceTo.AddParam("x");
            getDistanceTo.AddParam("y");
            getDistanceTo.AddParam("z");
            getDistanceTo.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                Debug.Log($"[MiniScript] Getting distance to ({x}, {y}, {z})");

                // Return placeholder distance
                return new Intrinsic.Result(new ValNumber(10.0));
            };
            _registeredIntrinsics.Add(getDistanceTo);
        }
    }
}
