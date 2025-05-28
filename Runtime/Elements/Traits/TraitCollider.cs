using System;
using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public enum ColliderType
    {
        None,
        Mesh,
        Cube,
        Sphere
    }

    [Name("Collider")]
    [Guid("d96a6a5a-2c29-4e6a-8437-05259460cc93")]
    public class TraitCollider : Trait<ColliderType>
    {

        private ColliderType _lastCollider = ColliderType.None;

        private GameObject Actor => Asset.Actor;

        /// <inheritdoc cref="Trait{T}"/>
        public TraitCollider(Asset asset) : base(asset: asset,
                                                 value: default,
                                                 tooltip: "Type of collider to generate for this Asset")
        {
            Process(ColliderType.Mesh);
        }

        /// <inheritdoc cref="Trait{T}.Destroy"/>
        public override void Destroy()
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.OnGet"/>
        protected override ColliderType OnGet()
        {
            return _lastCollider;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(ColliderType value)
        {
            Process(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void Process(ColliderType value)
        {
            if (_lastCollider == value) return;

            // Remove previous colliders (if any)
            foreach (var collider in Actor?.GetComponentsInChildren<Collider>())
            {
                if (collider != null)
                {
#if !UNITY_EDITOR
                    GameObject.Destroy(collider);
#else
                    GameObject.DestroyImmediate(collider);
#endif
                }
            }

            // Create new colliders (as applicable)
            foreach (var renderer in Actor?.GetComponentsInChildren<Renderer>())
            {
                switch (value)
                {
                    case ColliderType.None:
                        // NO-OP
                        break;
                    case ColliderType.Mesh:
                        renderer.gameObject.AddComponent<MeshCollider>();
                        break;
                    case ColliderType.Cube:
                        renderer.gameObject.AddComponent<BoxCollider>();
                        break;
                    case ColliderType.Sphere:
                        renderer.gameObject.AddComponent<SphereCollider>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }

            // Cache last value
            _lastCollider = value;
        }
    }
}