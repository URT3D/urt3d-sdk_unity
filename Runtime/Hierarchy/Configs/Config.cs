using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Configs
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Base class for all configuration-type Urt3d objects in the URT3D system.
    /// Config objects are typically non-visual entities that control game settings,
    /// system behaviors, or provide global functionality rather than representing
    /// interactive objects in the 3D world.
    /// </summary>
    public abstract class Config : Asset
    {
        /// <summary>
        /// Initializes the Config asset by setting up its basic configuration behavior.
        /// Unlike standard interactive objects, configurations are typically non-visual
        /// and non-interactive, so this implementation disables all colliders and renderers
        /// associated with the Config's Actor. This makes the Config object invisible and
        /// non-interactive in the 3D world, which is appropriate for settings-type objects.
        /// 
        /// Derived Config classes should call base.Initialize() before adding their
        /// specific initialization logic.
        /// </summary>
        protected override void Initialize()
        {
            var go = (GameObject)Actor;
            foreach (var collider in go.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
            foreach (var renderer in go.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }

        /// <summary>
        /// Enforces mutual-exclusivity of the given Asset type within the system.
        /// This method ensures only one instance of the specified Asset type exists
        /// by destroying all other instances of that type currently loaded in the AssetCache.
        /// Used to implement singleton-like behavior for configuration objects.
        /// </summary>
        /// <typeparam name="T">The type of Asset for which to enforce exclusivity</typeparam>
        protected static void EnforceMutualExclusivity<T>() where T : Asset
        {
            // TODO
        }
    }
}
