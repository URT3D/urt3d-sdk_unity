using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing logging functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for logging and debugging
        /// </summary>
        private void RegisterLogFunctions()
        {
            // Debug.Log wrapper
            var debug = Intrinsic.Create("debug");
            debug.AddParam("message");
            debug.code = (context, _) => {
                var message = context.GetVar("message").ToString();
                Debug.Log($"[MiniScript] {message}");
                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(debug);

            // Debug.LogWarning wrapper
            var warn = Intrinsic.Create("warn");
            warn.AddParam("message");
            warn.code = (context, _) => {
                var message = context.GetVar("message").ToString();
                Debug.LogWarning($"[MiniScript] {message}");
                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(warn);

            // Debug.LogError wrapper
            var error = Intrinsic.Create("error");
            error.AddParam("message");
            error.code = (context, _) => {
                var message = context.GetVar("message").ToString();
                Debug.LogError($"[MiniScript] {message}");
                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(error);
        }
    }
}
