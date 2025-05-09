using System;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Defines when a script should be triggered to execute
    /// </summary>
    [Serializable]
    public enum ScriptTriggerType
    {
        /// <summary>
        /// The script will run when the asset is first loaded
        /// </summary>
        OnLoad = 0,
        
        /// <summary>
        /// The script will run every frame during the Update cycle
        /// </summary>
        OnUpdate = 1,
        
        /// <summary>
        /// The script will only run when a custom event with the matching name is triggered
        /// </summary>
        OnCustomEvent = 2
    }
}