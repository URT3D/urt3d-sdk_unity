using Urt3d.Traits;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Entities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Represents a standard entity within the URT3D system.
    /// An Entity is a base implementation of a Urt3d object that comes pre-configured with
    /// common traits, causes, and effects suitable for general interactive objects.
    /// </summary>
    public class Entity : Asset
    {
        /// <summary>
        /// Initializes the Entity with a standard set of interactive capabilities.
        /// Sets up common traits like position, rotation, and scale, along with physics behaviors.
        /// Registers the Entity to respond to standard interactions like approaching, colliding,
        /// and input-based actions such as jumping and attacking.
        /// Also establishes default animation effects that can be triggered by these interactions.
        /// This creates a fully functional interactive object ready for use in URT3D experiences.
        /// </summary>
        protected override void Initialize()
        {
            // Add Traits

            AddTrait(new TraitAltitude(this));
            AddTrait(new TraitPosition2d(this));
            AddTrait(new TraitPosition3d(this));
            AddTrait(new TraitRotation3d(this));
            AddTrait(new TraitScale3d(this));

            AddTrait(new TraitCollider(this));
            AddTrait(new TraitIsActive(this));
            AddTrait(new TraitUsePhysics(this));
            AddTrait(new TraitUsePhysicsHiFi(this));

            // Add Scripts
            
            // TODO
        }

        /// <summary>
        /// Cleans up resources associated with the Entity when it is destroyed.
        /// This implementation is intentionally empty (NO-OP) because the base Asset class
        /// handles the cleanup of standard traits, causes, and effects.
        /// Override this method in derived classes if additional cleanup is required
        /// for Entity-specific resources or event handlers.
        /// </summary>
        protected override void Deinitialize()
        {
            // NO-OP
        }
    }
}
