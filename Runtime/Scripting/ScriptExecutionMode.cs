using System;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Defines when scripts should be allowed to execute
    /// </summary>
    [Serializable]
    public enum ScriptExecutionMode
    {
        /// <summary>
        /// Scripts will only execute in Editor mode
        /// </summary>
        EditorOnly = 0,
        
        /// <summary>
        /// Scripts will only execute in Runtime (Play) mode
        /// </summary>
        RuntimeOnly = 1,
        
        /// <summary>
        /// Scripts will execute in both Editor and Runtime modes
        /// </summary>
        Both = 2
    }
}