using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// A dynamic enumeration that allows for runtime manipulation of enum-like values.
    /// Unlike standard C# enums, DynamicEnum can have its values changed at runtime.
    /// Provides string-based enum values with index-based access and value validation.
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DynamicEnum
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the current index in the values collection.
        /// Index 0 is the default position if the collection is not empty.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets the current string value from the values collection.
        /// Setting this property will find the matching value in the collection and update the Index.
        /// If the value is not found, a warning is logged and no change occurs.
        /// </summary>
        public string Value
        {
            get
            {
                if (0 < Index && Index < _values.Count)
                {
                    return _values[Index];
                }
                return _default;
            }
            set
            {
                if (_values.Contains(value))
                {
                    Index = _values.IndexOf(value);
                }
                else
                {
                    Log.Warning($"Invalid value for {nameof(DynamicEnum)}: {value}, options are: {string.Join(',', _values)}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection of available string values.
        /// When set, attempts to preserve the current Value if present in the new collection.
        /// If the current Value is not in the new collection, resets to index 0.
        /// </summary>
        public ReadOnlyCollection<string> Values
        {
            get => _values.AsReadOnly();
            set
            {
                if (value.Contains(Value))
                {
                    // Retain value if possible
                    Index = value.IndexOf(Value);
                }
                else if (Index < 0 || Index >= value.Count)
                {
                    // Else retain index if possible
                    Index = 0;
                }
                _values = value.ToList();
            }
        }

        #endregion

        #region Public Methods: Construction and Destruction

        /// <summary>
        /// Creates a new DynamicEnum with an empty values collection.
        /// The default index is 0 and the default value is "Invalid".
        /// </summary>
        public DynamicEnum()
        {
            // NO-OP
        }

        /// <summary>
        /// Creates a new DynamicEnum with the specified values collection.
        /// The default index is set to 0.
        /// </summary>
        /// <param name="labels">The collection of string values for this enum</param>
        public DynamicEnum(IEnumerable<string> labels)
        {
            _values = new(labels);
        }

        /// <summary>
        /// Creates a new DynamicEnum with the specified values collection and initial index.
        /// </summary>
        /// <param name="index">The initial index to select from the values collection</param>
        /// <param name="labels">The collection of string values for this enum</param>
        public DynamicEnum(int index, IEnumerable<string> labels)
        {
            Index = index;
            _values = new(labels);
        }

        #endregion

        #region Private Variables

        private const string _default = "Invalid";
        private List<string> _values = new();

        #endregion
    }
}
