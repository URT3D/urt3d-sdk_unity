using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Traits
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Interface defining a Trait, which is a property or characteristic of a Urt3d object.
    /// Traits provide a standardized way to expose and manipulate Urt3d properties.
    /// </summary>
    public interface Trait : Param
    {
        /// <summary>
        /// Reference to the Urt3d object that owns this Trait.
        /// </summary>
        public Asset Asset { get; }
    }

    /// <summary>
    /// Abstract base class for implementing Traits with strongly-typed values.
    /// Provides common functionality for value access, change notification, and lifecycle management.
    /// </summary>
    /// <typeparam name="T">The data type of the value stored by this Trait.</typeparam>
    [SuppressMessage("ReSharper", "PublicConstructorInAbstractClass")]
    public abstract class Trait<T> : Param<T>, Trait
    {
        #region Public Properties

        /// <inheritdoc cref="Trait.Asset"/>
        [JsonIgnore] // Prevent infinite loop
        public Asset Asset { get; private set; }

        /// <inheritdoc cref="Param{T}.Value"/>
        public override T Value
        {
            get
            {
                T value;
                if (_isGetting)
                {
                    value = base.Value;
                }
                else
                {
                    _isGetting = true;
                    value = OnGet();
                    _isGetting = false;
                }
                return value;
            }
            set
            {
                if (_isSetting)
                {
                    base.Value = value;
                }
                else
                {
                    _isSetting = true;
                    OnSet(value);
                    _isSetting = false;
                }
            }
        }
        private bool _isGetting = false;
        private bool _isSetting = false;

        #endregion

        #region Public Methods: Construction and Destruction

        /// <inheritdoc cref="Param{T}"/>
        public Trait(Asset asset, T value, string tooltip) : base(string.Empty, value, tooltip, Guid.Empty)
        {
            Asset = asset;

            var type = GetType();
            if (!GuidAttribute.TryGet(type, out var guid))
            {
                Log.Error($"Failed to locate GUID attribute on Trait of Type: {type}");
            }
            Guid = guid;

            if (!NameAttribute.TryGet(type, out var name))
            {
                Log.Error($"Failed to locate Name attribute on Trait of Type: {type}");
            }
            Name = name;
        }

        /// <inheritdoc cref="Param.Destroy"/>
        public new abstract void Destroy();

        #endregion

        #region Protected Methods: Accessors and Mutators

        /// <summary>
        /// Process access to this Trait's value.
        /// Override this method to implement custom getter logic.
        /// This is called whenever the Value property is accessed.
        /// </summary>
        /// <returns>The current value of the Trait.</returns>
        protected abstract T OnGet();

        /// <summary>
        /// Process mutation of this Trait's value.
        /// Override this method to implement custom setter logic.
        /// This is called whenever the Value property is assigned.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        protected abstract void OnSet(T value);

        #endregion
    }
}
