using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that controls the scale of a Urt3d Actor.
    /// Enforces uniform scaling along the x, y, and z axes.
    /// </summary>
    [Name("Scale 1D")]
    [Guid("c712a8d9-430a-42e7-96c9-56c44f29449e")]
    public class TraitScale1d : Trait<float>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitScale1d(Asset asset) : base(asset:   asset,
                                                value:   default,
                                                tooltip: "Uniform scale factor applied to the model")
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
            return Actor.transform.localScale.magnitude;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(float value)
        {
            var ratio = value / Actor.transform.localScale.magnitude;
            if (float.IsFinite(ratio))
            {
                Actor.transform.localScale *= ratio;
            }
        }
    }

    /// <summary>
    /// Trait that controls the 3D scale of a Urt3d Actor.
    /// Allows independent scaling along the x, y, and z axes.
    /// </summary>
    [Name("Scale 3D")]
    [Guid("6e8d7cd9-e88a-47d3-9cab-fccbc954dac4")]
    public class TraitScale3d : Trait<Vector3>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitScale3d(Asset asset) : base(asset:   asset,
                                                value:   default,
                                                tooltip: "3D scale factor applied to the model")
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
            return Actor.transform.localScale;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(Vector3 value)
        {
            Actor.transform.localScale = value;
        }
    }
}
