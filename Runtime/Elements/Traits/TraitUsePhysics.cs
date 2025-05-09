using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that determines whether a URT3D Actor uses physics simulation.
    /// When enabled, the Actor will interact with the Unity physics system,
    /// responding to gravity, collisions, and forces.
    /// 
    /// This trait serves as a toggle for basic physics behavior, configuring
    /// the underlying Rigidbody component as needed. For more advanced physics
    /// configuration, see the TraitUsePhysicsHiFi class.
    /// </summary>
    [Name("Use Physics")]
    [Guid("904b4cba-2f67-4e62-9285-918fc44b8419")]
    public class TraitUsePhysics : Trait<bool>
    {
        /// <summary>
        /// Gets the GameObject reference from the associated URT3D Asset.
        /// This property provides convenient access to the Actor for physics component operations.
        /// </summary>
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitUsePhysics(Asset asset) : base(asset:   asset,
                                                   value:   default,
                                                   tooltip: "Enable physics simulation for this object")
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.Destroy"/>
        public override void Destroy()
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.OnGet"/>
        protected override bool OnGet()
        {
            return Value;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(bool value)
        {
            // Store the value for later retrieval
            Value = value;
            
            // Handle the Rigidbody component based on the physics state
            var rigidbody = Actor.GetComponent<Rigidbody>();
            
            if (value)
            {
                // Enable physics - ensure Rigidbody exists
                if (rigidbody == null)
                {
                    rigidbody = Actor.AddComponent<Rigidbody>();
                }
                
                // Ensure physics is enabled
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
            }
            else if (rigidbody != null)
            {
                // Disable physics by making the Rigidbody kinematic (ignores forces)
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }
        }
    }
}
