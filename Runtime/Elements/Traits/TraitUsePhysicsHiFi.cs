using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that enables high-fidelity physics simulation for a Urt3d Actor.
    /// When enabled, the Actor will use continuous collision detection for more accurate
    /// physics at the cost of increased computational overhead.
    /// </summary>
    [Name("Use Physics HiFi")]
    [Guid("63e71704-4d62-49a3-aa5c-e7c8412ac8fe")]
    public class TraitUsePhysicsHiFi : Trait<bool>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitUsePhysicsHiFi(Asset asset) : base(asset:   asset,
                                                       value:   default,
                                                       tooltip: "Enable continuous collision detection for more accurate physics")
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
            foreach (var rb in Actor.GetComponentsInChildren<Rigidbody>())
            {
                rb.collisionDetectionMode = value ? CollisionDetectionMode.Continuous :
                                                    CollisionDetectionMode.Discrete;
            }
        }
    }
}
