using System;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Represents a reference to a MiniScript script attached to a URT3D asset
    /// </summary>
    [Serializable]
    public class Script
    {
        /// <summary>
        /// Unique identifier for this script
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// Name of the script
        /// </summary>
        public string Name;

        /// <summary>
        /// Whether the script is enabled
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// When this script should be triggered
        /// </summary>
        public ScriptTriggerType TriggerType = ScriptTriggerType.OnLoad;

        /// <summary>
        /// Custom event name (for OnCustomEvent trigger type)
        /// </summary>
        public string CustomEventName;

        /// <summary>
        /// 
        /// </summary>
        public string ScriptContent = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Script()
        {
            Guid = Guid.NewGuid();
            Name = "NewScript";
        }

        /// <summary>
        /// Constructor with name and optional path
        /// </summary>
        /// <param name="name">Name of the script</param>
        public Script(string name)
        {
            Guid = Guid.NewGuid();
            Name = name;
        }
    }
}
