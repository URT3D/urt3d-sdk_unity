using System.Collections.Generic;
using Miniscript;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Bridge class that exposes URT3D APIs to MiniScript
    /// </summary>
    public partial class UrtScriptBridge
    {
        private readonly object _targetAsset;
        private readonly Asset _assetTarget; // Will be null if not an Asset
        private readonly List<Intrinsic> _registeredIntrinsics = new();

        /// <summary>
        /// Constructor with target URT3D asset
        /// </summary>
        /// <param name="targetAsset">The URT3D asset to expose to the script</param>
        public UrtScriptBridge(object targetAsset)
        {
            _targetAsset = targetAsset;
            _assetTarget = targetAsset as Asset;
            InitializeIntrinsics();
        }

        /// <summary>
        /// Initialize all intrinsic functions that will be exposed to MiniScript
        /// </summary>
        private void InitializeIntrinsics()
        {
            // Logging Functions
            RegisterLogFunctions();

            // Asset Properties
            RegisterAssetFunctions();

            // Transform Functions
            RegisterTransformFunctions();

            // Animation Functions
            RegisterAnimationFunctions();

            // Event System
            RegisterEventFunctions();

            // Asset Management Functions
            RegisterAssetManagementFunctions();

            // Advanced Transform Functions
            RegisterAdvancedTransformFunctions();

            // Physics Functions
            RegisterPhysicsFunctions();

            // Material and Visual Effects
            RegisterVisualFunctions();

            // Audio Functions
            RegisterAudioFunctions();

            // Advanced Animation System
            RegisterAdvancedAnimationFunctions();

            // Scene Management
            RegisterSceneManagementFunctions();

            // Input Handling
            RegisterInputFunctions();

            // Timing and Flow Control
            RegisterTimingFunctions();

            // State Management
            RegisterStateManagementFunctions();

            // Trait Management
            RegisterTraitManagementFunctions();

            // Camera Functions
            RegisterCameraFunctions();

            // Networking and Multiplayer
            RegisterNetworkingFunctions();

            // API Integration Functions
            RegisterApiIntegrationFunctions();
        }
    }
}
