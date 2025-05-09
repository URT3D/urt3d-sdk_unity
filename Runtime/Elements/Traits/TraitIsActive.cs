using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that controls whether a Urt3d Actor is active in the scene.
    /// When set to false, the Actor will be disabled and invisible, but still retained in memory.
    /// </summary>
    [Name("Is Active")]
    [Guid("888f4f01-8cd1-421f-b173-c8d58fc2f980")]
    public class TraitIsActive : Trait<bool>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitIsActive(Asset asset) : base(asset:   asset,
                                                 value:   default,
                                                 tooltip: "Controls whether this object is active in the scene")
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
            return Actor.activeSelf;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(bool value)
        {
            Actor.SetActive(value);
        }
    }
}
