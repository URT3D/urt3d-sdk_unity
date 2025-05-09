using System.Collections.Generic;
using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing trait management functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for trait management
        /// </summary>
        private void RegisterTraitManagementFunctions()
        {
            // Has trait
            var hasTrait = Intrinsic.Create("hasTrait");
            hasTrait.AddParam("traitName");
            hasTrait.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var traitName = context.GetVar("traitName").ToString();

                Debug.Log($"[MiniScript] Checking if asset has trait {traitName}");

                // Would normally check if the trait exists on the asset
                return new Intrinsic.Result(ValNumber.zero);
            };
            _registeredIntrinsics.Add(hasTrait);

            // Get trait value
            var getTrait = Intrinsic.Create("getTrait");
            getTrait.AddParam("traitName");
            getTrait.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var traitName = context.GetVar("traitName").ToString();

                Debug.Log($"[MiniScript] Getting trait {traitName} value");

                // Return placeholder trait value
                // In reality, this would get the actual trait value or null
                return new Intrinsic.Result(ValNumber.zero);
            };
            _registeredIntrinsics.Add(getTrait);

            // Set trait value
            var setTrait = Intrinsic.Create("setTrait");
            setTrait.AddParam("traitName");
            setTrait.AddParam("value");
            setTrait.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var traitName = context.GetVar("traitName").ToString();
                var value = context.GetVar("value");

                Debug.Log($"[MiniScript] Setting trait {traitName} to value {value}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setTrait);

            // Add trait
            var addTrait = Intrinsic.Create("addTrait");
            addTrait.AddParam("traitName");
            addTrait.AddParam("initialValue", ""); // Using empty string instead of null
            addTrait.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var traitName = context.GetVar("traitName").ToString();
                var initialValue = context.GetVar("initialValue");

                Debug.Log($"[MiniScript] Adding trait {traitName} with initial value {initialValue}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(addTrait);

            // Remove trait
            var removeTrait = Intrinsic.Create("removeTrait");
            removeTrait.AddParam("traitName");
            removeTrait.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var traitName = context.GetVar("traitName").ToString();

                Debug.Log($"[MiniScript] Removing trait {traitName}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(removeTrait);

            // List all traits
            var listTraits = Intrinsic.Create("listTraits");
            listTraits.code = (_, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(new ValList());
                }

                Debug.Log("[MiniScript] Listing all traits");

                // Return placeholder trait list using array constructor
                var traits = new List<Value> {
                    new ValString("Position"),
                    new ValString("Rotation"),
                    new ValString("Scale")
                };
                var traitList = new ValList(traits);

                return new Intrinsic.Result(traitList);
            };
            _registeredIntrinsics.Add(listTraits);
        }
    }
}
