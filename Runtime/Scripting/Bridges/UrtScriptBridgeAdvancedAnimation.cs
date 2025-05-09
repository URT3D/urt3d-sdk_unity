using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing advanced animation functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for advanced animation control
        /// </summary>
        private void RegisterAdvancedAnimationFunctions()
        {
            // Play animation with specific parameters
            var playAnimationAdvanced = Intrinsic.Create("playAnimationAdvanced");
            playAnimationAdvanced.AddParam("name");
            playAnimationAdvanced.AddParam("crossfadeTime", 0.25);
            playAnimationAdvanced.AddParam("speed", 1.0);
            playAnimationAdvanced.AddParam("layer", 0);
            playAnimationAdvanced.AddParam("weight", 1.0);
            playAnimationAdvanced.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var animationName = context.GetVar("name").ToString();
                var crossfadeTime = (float)context.GetVar("crossfadeTime").DoubleValue();
                var speed = (float)context.GetVar("speed").DoubleValue();
                var layer = (int)context.GetVar("layer").DoubleValue();
                var weight = (float)context.GetVar("weight").DoubleValue();

                Debug.Log($"[MiniScript] Playing animation {animationName} with crossfade {crossfadeTime}, speed {speed}, layer {layer}, weight {weight}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(playAnimationAdvanced);

            // Set animation parameter
            var setAnimationParam = Intrinsic.Create("setAnimationParam");
            setAnimationParam.AddParam("paramName");
            setAnimationParam.AddParam("value");
            setAnimationParam.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var paramName = context.GetVar("paramName").ToString();
                var value = context.GetVar("value");

                Debug.Log($"[MiniScript] Setting animation parameter {paramName} to {value}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setAnimationParam);

            // Get animation parameter
            var getAnimationParam = Intrinsic.Create("getAnimationParam");
            getAnimationParam.AddParam("paramName");
            getAnimationParam.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var paramName = context.GetVar("paramName").ToString();

                Debug.Log($"[MiniScript] Getting animation parameter {paramName}");

                // Return placeholder parameter value
                return new Intrinsic.Result(new ValNumber(0));
            };
            _registeredIntrinsics.Add(getAnimationParam);

            // Get animation state
            var getAnimationState = Intrinsic.Create("getAnimationState");
            getAnimationState.AddParam("animationName", "");
            getAnimationState.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var animationName = context.GetVar("animationName").ToString();

                Debug.Log($"[MiniScript] Getting animation state for {animationName}");

                // Return placeholder animation state
                var stateMap = new ValMap
                {
                    ["isPlaying"] = ValNumber.zero,
                    ["normalizedTime"] = new ValNumber(0),
                    ["length"] = new ValNumber(1),
                    ["speed"] = new ValNumber(1),
                    ["weight"] = new ValNumber(0)
                };

                return new Intrinsic.Result(stateMap);
            };
            _registeredIntrinsics.Add(getAnimationState);

            // Set animation speed
            var setAnimationSpeed = Intrinsic.Create("setAnimationSpeed");
            setAnimationSpeed.AddParam("speed", 1.0);
            setAnimationSpeed.AddParam("animationName", "");
            setAnimationSpeed.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var speed = (float)context.GetVar("speed").DoubleValue();
                var animationName = context.GetVar("animationName").ToString();

                Debug.Log(string.IsNullOrEmpty(animationName)
                    ? $"[MiniScript] Setting all animations speed to {speed}"
                    : $"[MiniScript] Setting animation {animationName} speed to {speed}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setAnimationSpeed);

            // Create animation transition
            var createAnimationTransition = Intrinsic.Create("createAnimationTransition");
            createAnimationTransition.AddParam("fromState");
            createAnimationTransition.AddParam("toState");
            createAnimationTransition.AddParam("duration", 0.25);
            createAnimationTransition.AddParam("offset", 0.0);
            createAnimationTransition.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var fromState = context.GetVar("fromState").ToString();
                var toState = context.GetVar("toState").ToString();
                var duration = (float)context.GetVar("duration").DoubleValue();
                var offset = (float)context.GetVar("offset").DoubleValue();

                Debug.Log($"[MiniScript] Creating animation transition from {fromState} to {toState} with duration {duration} and offset {offset}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(createAnimationTransition);
        }
    }
}
