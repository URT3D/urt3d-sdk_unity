using System;
using System.Security.Cryptography;
using System.Text;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// A generic implementation of Trait that allows for dynamic getter and setter callbacks.
    /// Useful for creating traits without needing to create a dedicated class for each trait type.
    /// </summary>
    /// <typeparam name="T">The data type of the value stored by this trait.</typeparam>
    [Name("Anonymous")]
    [Guid("339069ea-58ae-4847-867f-7d262fd86ce7")]
    public class TraitAnonymous<T> : Trait<T>
    {
        public delegate T DelegateGet();
        public delegate void DelegateSet(T value);

        private readonly DelegateGet _delGet;
        private readonly DelegateSet _delSet;

        /// <summary>
        /// Create trait with a display name, no delegates, and an optional initial value.
        /// </summary>
        public TraitAnonymous(Asset asset, string name, T value = default) : this(asset, name, null, null, value)
        {
            // NO-OP
        }

        /// <summary>
        /// Create trait with a display name, just a 'set' delegate, and an optional initial value.
        /// </summary>
        public TraitAnonymous(Asset asset, string name, DelegateSet delSet, T value = default) : this(asset, name, null, delSet, value)
        {
            // NO-OP
        }

        /// <summary>
        /// Create trait with a display name, both 'get' and 'set' delegates, and an optional initial value.
        /// </summary>
        public TraitAnonymous(Asset asset, string name, DelegateGet delGet, DelegateSet delSet, T value = default) : base(asset:   asset,
                                                                                                                          value:   value,
                                                                                                                          tooltip: $"Anonymous {name} of type {typeof(T)}")
        {
            Name = name;
            Guid = GetGuid(name);

            _delGet = delGet;
            _delSet = delSet;
        }

        public override void Destroy()
        {
            // NO-OP
        }

        /// <inheritdoc cref="Trait{T}.OnGet"/>
        protected override T OnGet()
        {
            if (_delGet != null)
            {
                return _delGet.Invoke();
            }
            return Value;
        }

        /// <inheritdoc cref="Trait{T}.OnSet"/>
        protected override void OnSet(T value)
        {
            if (_delSet != null)
            {
                _delSet.Invoke(value);
            }
            else
            {
                Value = value;
            }
        }

        private static Guid GetGuid(string name)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
            return new Guid(hash);
        }
    }
}
