using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing animation functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for controlling animations
        /// </summary>
        private void RegisterAnimationFunctions()
        {
            var playAnimation = Intrinsic.Create("playAnimation");
            playAnimation.AddParam("name");
            playAnimation.code = (context, _) => {
                var animationName = context.GetVar("name").ToString();

                // This would need to be implemented based on how URT3D handles animations
                // For now, just log the action
                Debug.Log($"[MiniScript] Playing animation: {animationName}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(playAnimation);

            var stopAnimation = Intrinsic.Create("stopAnimation");
            stopAnimation.AddParam("name", "");
            stopAnimation.code = (context, _) => {
                var animationName = context.GetVar("name").ToString();

                // This would need to be implemented based on how URT3D handles animations
                // For now, just log the action
                Debug.Log(string.IsNullOrEmpty(animationName)
                    ? "[MiniScript] Stopping all animations"
                    : $"[MiniScript] Stopping animation: {animationName}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(stopAnimation);
        }
    }
}
