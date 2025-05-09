using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing timing and flow control functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for timing and flow control
        /// </summary>
        private void RegisterTimingFunctions()
        {
            // Get current time
            var getTime = Intrinsic.Create("getTime");
            getTime.code = (_, _) => {
                Debug.Log("[MiniScript] Getting current time");

                // Return placeholder time info
                var timeMap = new ValMap
                {
                    ["time"] = new ValNumber(100.0), // Time since start
                    ["deltaTime"] = new ValNumber(0.016), // Last frame time
                    ["frameCount"] = new ValNumber(1000) // Current frame number
                };

                return new Intrinsic.Result(timeMap);
            };
            _registeredIntrinsics.Add(getTime);

            // Set timeout
            var setTimeout = Intrinsic.Create("setTimeout");
            setTimeout.AddParam("seconds");
            setTimeout.AddParam("eventName");
            setTimeout.AddParam("data", string.Empty);
            setTimeout.code = (context, _) => {
                var seconds = (float)context.GetVar("seconds").DoubleValue();
                var eventName = context.GetVar("eventName").ToString();
                var data = context.GetVar("data");

                Debug.Log($"[MiniScript] Setting timeout for {seconds} seconds to trigger event {eventName}");

                // Return timer ID for potential cancellation
                return new Intrinsic.Result(new ValString("timer_12345"));
            };
            _registeredIntrinsics.Add(setTimeout);

            // Clear timeout
            var clearTimeout = Intrinsic.Create("clearTimeout");
            clearTimeout.AddParam("timerId");
            clearTimeout.code = (context, _) => {
                var timerId = context.GetVar("timerId").ToString();

                Debug.Log($"[MiniScript] Clearing timeout with ID {timerId}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(clearTimeout);

            // Set interval
            var setInterval = Intrinsic.Create("setInterval");
            setInterval.AddParam("seconds");
            setInterval.AddParam("eventName");
            setInterval.AddParam("data", string.Empty);
            setInterval.code = (context, _) => {
                var seconds = (float)context.GetVar("seconds").DoubleValue();
                var eventName = context.GetVar("eventName").ToString();
                var data = context.GetVar("data");

                Debug.Log($"[MiniScript] Setting interval every {seconds} seconds to trigger event {eventName}");

                // Return interval ID for potential cancellation
                return new Intrinsic.Result(new ValString("interval_12345"));
            };
            _registeredIntrinsics.Add(setInterval);

            // Clear interval
            var clearInterval = Intrinsic.Create("clearInterval");
            clearInterval.AddParam("intervalId");
            clearInterval.code = (context, _) => {
                var intervalId = context.GetVar("intervalId").ToString();

                Debug.Log($"[MiniScript] Clearing interval with ID {intervalId}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(clearInterval);

            // Frame delay - execute on next frame
            var nextFrame = Intrinsic.Create("nextFrame");
            nextFrame.AddParam("eventName");
            nextFrame.AddParam("data", string.Empty);
            nextFrame.code = (context, _) => {
                var eventName = context.GetVar("eventName").ToString();
                var data = context.GetVar("data");

                Debug.Log($"[MiniScript] Scheduling event {eventName} for next frame");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(nextFrame);
        }
    }
}
