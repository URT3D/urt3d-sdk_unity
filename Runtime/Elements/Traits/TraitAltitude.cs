using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that represents and controls the altitude (Y-position) of a Urt3d.
    /// Provides a way to get or set the vertical position in Unity world space.
    /// </summary>
    [Name("Altitude")]
    [Guid("c0c4a4de-7e95-4b9a-bcdc-21fc020f24cb")]
    public class TraitAltitude : Trait<float>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitAltitude(Asset asset) : base(asset: asset,
                                                 value: default,
                                                 tooltip: "Altitude above mean sea level")
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.Destroy"/>
        public override void Destroy()
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.OnGet"/>
        protected override float OnGet()
        {
            return Actor.transform.position.y;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(float value)
        {
            var position = Actor.transform.position;
            position.y = value;
            Actor.transform.position = position;
        }
    }
}
