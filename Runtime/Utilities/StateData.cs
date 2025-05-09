using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Tracks global application state information such as whether the application is playing or quitting.
    /// Implemented as a singleton to provide centralized access to state information.
    /// </summary>
    public class StateData
    {
        /// <summary>
        /// Gets the thread-safe singleton instance of StateData, creating it if needed.
        /// </summary>
        public static StateData Instance => Singleton.GetInstance<StateData>();

        /// <summary>
        /// Gets a value indicating whether the application is currently playing.
        /// Returns false if the application is in the process of quitting or is in edit mode.
        /// </summary>
        public bool IsPlaying => !isQuitting && Application.isPlaying;

        /// <summary>
        /// Constructor to initialize StateData.
        /// Subscribes to the application quitting event to track application state.
        /// </summary>
        public StateData()
        {
            Application.quitting += OnQuitting;
        }

        /// <summary>
        /// Event handler for application quitting.
        /// Sets the isQuitting flag to true when the application is shutting down.
        /// </summary>
        private void OnQuitting()
        {
            isQuitting = true;
        }
        private bool isQuitting = false;
    }
}