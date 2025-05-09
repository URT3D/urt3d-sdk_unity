using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// A Param is a generalized, named variable.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public interface Param
    {
        #region Public Properties

        /// <summary>
        /// Unique identifier for this Param.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// The display name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The tooltip text to display when hovering over this parameter in the UI.
        /// </summary>
        public string Tooltip { get; }

        /// <summary>
        /// The data type of this parameter's value.
        /// </summary>
        [JsonIgnore]
        public Type Type { get; }

        #endregion

        #region Public Methods: Operations

        /// <summary>
        /// Gets or sets the parameter's value as a non-typed object.
        /// Use this when the specific type is not known at compile time.
        /// </summary>
        [JsonIgnore] // Don't include raw data in JSON
        public object ValueObject { get; set; }

        /// <summary>
        /// Gets the parameter's value converted to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The parameter value as type T, or default(T) if conversion fails.</returns>
        public T GetValue<T>();

        /// <summary>
        /// Sets the parameter's value from a value of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value was successfully set, false otherwise.</returns>
        public bool SetValue<T>(T value);

        #endregion

        #region Public Methods: Construction and Destruction

        /// <summary>
        /// Creates a deep copy of this parameter.
        /// </summary>
        /// <returns>A new Param instance with the same name, tooltip, and default value.</returns>
        public Param Clone();

        /// <summary>
        /// Releases any resources used by this parameter.
        /// </summary>
        public void Destroy();

        #endregion
    }

    /// <summary>
    /// Generic implementation of the Param interface that provides type-safe access to the parameter's value.
    /// </summary>
    /// <typeparam name="T">The data type of the value stored by this parameter.</typeparam>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public class Param<T> : Param
    {
        #region Public Events

        /// <summary>
        /// Event triggered when this parameter's value changes.
        /// Provides both the new value and the previous value.
        /// </summary>
        [JsonIgnore]
        public Action<(T valueNew, T valueOld)> OnValueChanged;

        #endregion

        #region Public Properties

        /// <inheritdoc cref="Param.Guid"/>
        public Guid Guid { get; protected set; } = Guid.Empty;

        /// <inheritdoc cref="Param.Name"/>
        public string Name { get; protected set; } = string.Empty;

        /// <inheritdoc cref="Param.Tooltip"/>
        public string Tooltip { get; } = string.Empty;

        /// <inheritdoc cref="Param.Type"/>
        public Type Type => typeof(T);

        /// <summary>
        /// Gets or sets the strongly-typed value of this parameter.
        /// Triggers the OnValueChanged event when the value changes.
        /// </summary>
        public virtual T Value
        {
            get => _value;
            set
            {
                var valueOld = _value;
                _value = value;
                if (!Equals(valueOld, value))
                {
                    OnValueChanged?.Invoke((_value, valueOld));
                }
            }
        }

        #endregion

        #region Public Methods: Operations

        /// <inheritdoc cref="Param.ValueObject"/>
        [JsonIgnore] // Don't include raw data in JSON
        public object ValueObject
        {
            get => Value;
            set => Value = (T)value;
        }

        /// <inheritdoc cref="Param.GetValue"/>
        public TVal GetValue<TVal>()
        {
            if (typeof(TVal).IsAssignableFrom(Type))
            {
                return (TVal)ValueObject;
            }
            Log.Error($"Failed to access value, it is of type {Type} not {typeof(TVal)}");
            return default;
        }

        /// <inheritdoc cref="Param.SetValue"/>
        public bool SetValue<TVal>(TVal value)
        {
            if (typeof(TVal).IsAssignableFrom(Type))
            {
                ValueObject = value;
                return true;
            }
            Log.Error($"Failed to mutate value, it is of type {Type} not {typeof(TVal)}");
            return false;
        }

        #endregion

        #region Public Methods: Construction and Destruction

        /// <summary>
        /// Creates a parameter with the specified name.
        /// </summary>
        /// <param name="name">The display name of the parameter.</param>
        public Param(string name, Guid guid) : this(name, default, guid)
        {
            // NO-OP
        }

        /// <summary>
        /// Creates a parameter with the specified name and initial value.
        /// </summary>
        /// <param name="name">The display name of the parameter.</param>
        /// <param name="value">The initial value of the parameter.</param>
        public Param(string name, T value, Guid guid) : this(name, value, $"Param {name} of Type {typeof(T)}", guid)
        {
            // NO-OP
        }

        /// <summary>
        /// Creates a parameter with the specified name, initial value, and tooltip.
        /// </summary>
        /// <param name="name">The display name of the parameter.</param>
        /// <param name="value">The initial value of the parameter.</param>
        /// <param name="tooltip">The tooltip text to display in the UI.</param>
        public Param(string name, T value, string tooltip, Guid guid)
        {
            Name = name;
            Guid = guid;
            _value = value;
            Tooltip = tooltip;
        }

        /// <inheritdoc cref="Param.Clone"/>
        public Param Clone()
        {
            return new Param<T>(Name, Value, Tooltip, Guid);
        }

        /// <inheritdoc cref="Param.Destroy"/>
        public virtual void Destroy()
        {
            // NO-OP
        }

        #endregion

        #region Private Variables

        private T _value = default;

        #endregion
    }
}
