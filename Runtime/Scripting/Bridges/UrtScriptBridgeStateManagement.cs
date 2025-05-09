using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing state management functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for state management
        /// </summary>
        private void RegisterStateManagementFunctions()
        {
            // Set state value
            var setState = Intrinsic.Create("setState");
            setState.AddParam("key");
            setState.AddParam("value");
            setState.AddParam("scope", "local"); // local, asset, scene, global
            setState.code = (context, _) => {
                var key = context.GetVar("key").ToString();
                var value = context.GetVar("value");
                var scope = context.GetVar("scope").ToString();

                Debug.Log($"[MiniScript] Setting {scope} state {key} to {value}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setState);

            // Get state value
            var getState = Intrinsic.Create("getState");
            getState.AddParam("key");
            getState.AddParam("defaultValue", ""); // Using empty string instead of null
            getState.AddParam("scope", "local"); // local, asset, scene, global
            getState.code = (context, _) => {
                var key = context.GetVar("key").ToString();
                var defaultValue = context.GetVar("defaultValue");
                var scope = context.GetVar("scope").ToString();

                Debug.Log($"[MiniScript] Getting {scope} state {key}");

                // Return placeholder - would normally return stored value or defaultValue
                return new Intrinsic.Result(defaultValue);
            };
            _registeredIntrinsics.Add(getState);

            // Check if state exists
            var hasState = Intrinsic.Create("hasState");
            hasState.AddParam("key");
            hasState.AddParam("scope", "local"); // local, asset, scene, global
            hasState.code = (context, _) => {
                var key = context.GetVar("key").ToString();
                var scope = context.GetVar("scope").ToString();

                Debug.Log($"[MiniScript] Checking if {scope} state {key} exists");

                // Return placeholder - would normally check if state exists
                return new Intrinsic.Result(ValNumber.zero);
            };
            _registeredIntrinsics.Add(hasState);

            // Delete state
            var deleteState = Intrinsic.Create("deleteState");
            deleteState.AddParam("key");
            deleteState.AddParam("scope", "local"); // local, asset, scene, global
            deleteState.code = (context, _) => {
                var key = context.GetVar("key").ToString();
                var scope = context.GetVar("scope").ToString();

                Debug.Log($"[MiniScript] Deleting {scope} state {key}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(deleteState);

            // Clear all states in a scope
            var clearStates = Intrinsic.Create("clearStates");
            clearStates.AddParam("scope", "local"); // local, asset, scene, global
            clearStates.code = (context, _) => {
                var scope = context.GetVar("scope").ToString();

                Debug.Log($"[MiniScript] Clearing all {scope} states");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(clearStates);
        }
    }
}
