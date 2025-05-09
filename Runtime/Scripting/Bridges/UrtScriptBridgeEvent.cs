using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing event system functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for the event system
        /// </summary>
        private void RegisterEventFunctions()
        {
            var triggerEvent = Intrinsic.Create("triggerEvent");
            triggerEvent.AddParam("eventName");
            triggerEvent.AddParam("data", string.Empty);
            triggerEvent.code = (context, _) => {
                var eventName = context.GetVar("eventName").ToString();
                var data = context.GetVar("data");

                // This would need to be implemented based on URT3D event system
                Debug.Log($"[MiniScript] Trigger event: {eventName} with data: {data}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(triggerEvent);
        }
    }
}
