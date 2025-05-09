using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that controls the 2D position of a Urt3d Actor in the horizontal plane.
    /// Maps x/y coordinates in the Trait to x/z coordinates in the Unity world space.
    /// </summary>
    [Name("Position 2D")]
    [Guid("308d6256-e78a-49d7-a493-572be1bcda15")]
    public class TraitPosition2d : Trait<Vector2>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitPosition2d(Asset asset) : base(asset:   asset,
                                                   value:   default,
                                                   tooltip: "Position in x/y space when viewed from above")
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.Destroy"/>
        public override void Destroy()
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.OnGet"/>
        protected override Vector2 OnGet()
        {
            var position = Actor.transform.position;
            return new Vector2(position.x, position.z);
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(Vector2 value)
        {
            var position = Actor.transform.position;
            position.x = value.x;
            position.z = value.y;
            Actor.transform.position = position;
        }
    }

    /// <summary>
    /// Trait that controls the 3D position of a Urt3d Actor in Unity world space.
    /// </summary>
    [Name("Position 3D")]
    [Guid("d2c9d3ba-5e7e-4e69-96ad-4bc94c65f823")]
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public class TraitPosition3d : Trait<Vector3>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitPosition3d(Asset asset) : base(asset:   asset,
                                                   value:   default,
                                                   tooltip: "Position in Unity world space")
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.Destroy"/>
        public override void Destroy()
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.OnGet"/>
        protected override Vector3 OnGet()
        {
            if (Actor == null) return default;

            return Actor.transform.position;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(Vector3 value)
        {
            Actor.transform.position = value;
        }
    }
}
