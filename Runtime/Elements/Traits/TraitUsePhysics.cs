using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that determines whether a Urt3d Actor uses physics simulation.
    /// When enabled, the Actor will interact with the physics system,
    /// responding to gravity, collisions, and forces.
    /// </summary>
    [Name("Use Physics")]
    [Guid("904b4cba-2f67-4e62-9285-918fc44b8419")]
    public class TraitUsePhysics : Trait<bool>
    {
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
            Value = value; // TODO
        }
    }
}
