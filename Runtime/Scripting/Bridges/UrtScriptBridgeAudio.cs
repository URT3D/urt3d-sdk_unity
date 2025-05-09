using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing audio functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for audio control
        /// </summary>
        private void RegisterAudioFunctions()
        {
            // Play sound
            var playSound = Intrinsic.Create("playSound");
            playSound.AddParam("soundId");
            playSound.AddParam("volume", 1.0);
            playSound.AddParam("pitch", 1.0);
            playSound.AddParam("loop", ValNumber.zero); // false
            playSound.code = (context, _) => {
                var soundId = context.GetVar("soundId").ToString();
                var volume = (float)context.GetVar("volume").DoubleValue();
                var pitch = (float)context.GetVar("pitch").DoubleValue();
                var loop = context.GetVar("loop").BoolValue();

                Debug.Log($"[MiniScript] Playing sound {soundId} with volume {volume}, pitch {pitch}, loop {loop}");

                // Return sound instance id for control
                return new Intrinsic.Result(new ValString("sound_instance_12345"));
            };
            _registeredIntrinsics.Add(playSound);

            // Stop sound
            var stopSound = Intrinsic.Create("stopSound");
            stopSound.AddParam("instanceId", "");
            stopSound.code = (context, _) => {
                var instanceId = context.GetVar("instanceId").ToString();

                Debug.Log(string.IsNullOrEmpty(instanceId)
                    ? "[MiniScript] Stopping all sounds"
                    : $"[MiniScript] Stopping sound {instanceId}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(stopSound);

            // Set sound volume
            var setSoundVolume = Intrinsic.Create("setSoundVolume");
            setSoundVolume.AddParam("instanceId");
            setSoundVolume.AddParam("volume", 1.0);
            setSoundVolume.code = (context, _) => {
                var instanceId = context.GetVar("instanceId").ToString();
                var volume = (float)context.GetVar("volume").DoubleValue();

                Debug.Log($"[MiniScript] Setting sound {instanceId} volume to {volume}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setSoundVolume);

            // Set sound pitch
            var setSoundPitch = Intrinsic.Create("setSoundPitch");
            setSoundPitch.AddParam("instanceId");
            setSoundPitch.AddParam("pitch", 1.0);
            setSoundPitch.code = (context, _) => {
                var instanceId = context.GetVar("instanceId").ToString();
                var pitch = (float)context.GetVar("pitch").DoubleValue();

                Debug.Log($"[MiniScript] Setting sound {instanceId} pitch to {pitch}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setSoundPitch);

            // Set sound position
            var setSoundPosition = Intrinsic.Create("setSoundPosition");
            setSoundPosition.AddParam("instanceId");
            setSoundPosition.AddParam("x", 0.0);
            setSoundPosition.AddParam("y", 0.0);
            setSoundPosition.AddParam("z", 0.0);
            setSoundPosition.code = (context, _) => {
                var instanceId = context.GetVar("instanceId").ToString();
                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                Debug.Log($"[MiniScript] Setting sound {instanceId} position to ({x}, {y}, {z})");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setSoundPosition);

            // Set master volume
            var setMasterVolume = Intrinsic.Create("setMasterVolume");
            setMasterVolume.AddParam("volume", 1.0);
            setMasterVolume.code = (context, _) => {
                var volume = (float)context.GetVar("volume").DoubleValue();

                Debug.Log($"[MiniScript] Setting master volume to {volume}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setMasterVolume);
        }
    }
}
