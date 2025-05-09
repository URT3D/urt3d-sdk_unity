using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Urt3d.Symbols;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Trait that controls a single-axis rotation of a Urt3d Actor.
    /// For standard objects, this controls the y-axis rotation (yaw).
    /// For Symbol objects, this controls the z-axis rotation.
    /// </summary>
    [Name("Rotation 1D")]
    [Guid("7856d9e1-9fca-4458-b377-c8ef6192e985")]
    public class TraitRotation1D : Trait<float>
    {
        private readonly bool _isSymbol;
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitRotation1D(Asset asset) : base(asset:   asset,
                                                   value:   default,
                                                   tooltip: "One-dimensional rotation")
        {
            _isSymbol = asset is Symbol;
        }

        /// <inheritdoc cref="Trait{T}.Destroy"/>
        public override void Destroy()
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.OnGet"/>
        protected override float OnGet()
        {
            if (Actor == null) return default;

            var euler = Actor.transform.localRotation.eulerAngles;
            return _isSymbol ? euler.z : euler.y;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(float value)
        {
            var euler = Actor.transform.localRotation.eulerAngles;
            if (_isSymbol)
            {
                euler.z = value;
            }
            else
            {
                euler.y = value;
            }
            Actor.transform.localRotation = Quaternion.Euler(euler);
        }
    }

    /// <summary>
    /// Trait that controls the full 3D rotation of a Urt3d Actor using Euler angles.
    /// Provides access to pitch, yaw, and roll (x, y, z) rotation components.
    /// </summary>
    [Name("Rotation 3D")]
    [Guid("ef7b34d6-3743-460b-b730-6f872f4738d4")]
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public class TraitRotation3d : Trait<Vector3>
    {
        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitRotation3d(Asset asset) : base(asset:   asset,
                                                   value:   default,
                                                   tooltip: "Three-dimensional rotation")
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

            return Actor.transform.rotation.eulerAngles;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(Vector3 value)
        {
            Actor.transform.rotation = Quaternion.Euler(value);
        }
    }
}
