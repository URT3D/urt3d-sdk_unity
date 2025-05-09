using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing input handling functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for input handling
        /// </summary>
        private void RegisterInputFunctions()
        {
            // Get key pressed
            var isKeyPressed = Intrinsic.Create("isKeyPressed");
            isKeyPressed.AddParam("keyName");
            isKeyPressed.code = (context, _) => {
                var keyName = context.GetVar("keyName").ToString();

                Debug.Log($"[MiniScript] Checking if key {keyName} is pressed");

                return new Intrinsic.Result(ValNumber.zero); // Default to not pressed
            };
            _registeredIntrinsics.Add(isKeyPressed);

            // Get mouse position
            var getMousePosition = Intrinsic.Create("getMousePosition");
            getMousePosition.code = (_, _) => {
                Debug.Log("[MiniScript] Getting mouse position");

                // Return placeholder mouse position
                var positionMap = new ValMap
                {
                    ["x"] = new ValNumber(0),
                    ["y"] = new ValNumber(0)
                };

                return new Intrinsic.Result(positionMap);
            };
            _registeredIntrinsics.Add(getMousePosition);

            // Is mouse button down
            var isMouseButtonDown = Intrinsic.Create("isMouseButtonDown");
            isMouseButtonDown.AddParam("button", 0);
            isMouseButtonDown.code = (context, _) => {
                var button = (int)context.GetVar("button").DoubleValue();

                Debug.Log($"[MiniScript] Checking if mouse button {button} is down");

                return new Intrinsic.Result(ValNumber.zero); // Default to not pressed
            };
            _registeredIntrinsics.Add(isMouseButtonDown);

            // Get axis value
            var getAxisValue = Intrinsic.Create("getAxisValue");
            getAxisValue.AddParam("axisName");
            getAxisValue.code = (context, _) => {
                var axisName = context.GetVar("axisName").ToString();

                Debug.Log($"[MiniScript] Getting input axis {axisName} value");

                return new Intrinsic.Result(new ValNumber(0)); // Default to 0
            };
            _registeredIntrinsics.Add(getAxisValue);

            // Raycast from screen point
            var raycastFromScreen = Intrinsic.Create("raycastFromScreen");
            raycastFromScreen.AddParam("screenX");
            raycastFromScreen.AddParam("screenY");
            raycastFromScreen.AddParam("maxDistance", 100.0);
            raycastFromScreen.code = (context, _) => {
                var screenX = (float)context.GetVar("screenX").DoubleValue();
                var screenY = (float)context.GetVar("screenY").DoubleValue();
                var maxDistance = (float)context.GetVar("maxDistance").DoubleValue();

                Debug.Log($"[MiniScript] Raycasting from screen point ({screenX}, {screenY}) with max distance {maxDistance}");

                // Return placeholder hit result
                var hitResult = new ValMap
                {
                    ["hit"] = ValNumber.zero, // No hit for placeholder
                    ["distance"] = new ValNumber(0),
                    ["assetId"] = new ValString(""),
                    ["point"] = new ValMap {
                        ["x"] = new ValNumber(0),
                        ["y"] = new ValNumber(0),
                        ["z"] = new ValNumber(0)
                    }
                };

                return new Intrinsic.Result(hitResult);
            };
            _registeredIntrinsics.Add(raycastFromScreen);
        }
    }
}
