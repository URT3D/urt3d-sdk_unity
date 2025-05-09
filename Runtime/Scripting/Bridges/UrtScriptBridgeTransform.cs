using Miniscript;
using UnityEngine;
using Urt3d.Traits;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing transform functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for manipulating transforms
        /// </summary>
        private void RegisterTransformFunctions()
        {
            // Get position
            var getPosition = Intrinsic.Create("getPosition");
            getPosition.code = (_, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var objPos = _assetTarget.GetTrait<TraitPosition3d>();
                if (objPos == null)
                {
                    return new Intrinsic.Result(null, true);
                }

                var value = objPos.Value;

                var positionMap = new ValMap
                {
                    ["x"] = new ValNumber(value.x),
                    ["y"] = new ValNumber(value.y),
                    ["z"] = new ValNumber(value.z)
                };

                return new Intrinsic.Result(positionMap);
            };
            _registeredIntrinsics.Add(getPosition);

            // Set position
            var setPosition = Intrinsic.Create("setPosition");
            setPosition.AddParam("x", 0.0);
            setPosition.AddParam("y", 0.0);
            setPosition.AddParam("z", 0.0);
            setPosition.code = (context, partialResult) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var objPos = _assetTarget.GetTrait<TraitPosition3d>();
                if (objPos == null)
                {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                _ = objPos.SetValue(new Vector3(x, y, z));
                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setPosition);

            // Get rotation
            var getRotation = Intrinsic.Create("getRotation");
            getRotation.code = (_, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var objRot = _assetTarget.GetTrait<TraitRotation3d>();
                if (objRot == null)
                {
                    return new Intrinsic.Result(null, true);
                }

                var rotationMap = new ValMap
                {
                    ["x"] = new ValNumber(objRot.Value.x),
                    ["y"] = new ValNumber(objRot.Value.y),
                    ["z"] = new ValNumber(objRot.Value.z)
                };

                return new Intrinsic.Result(rotationMap);
            };
            _registeredIntrinsics.Add(getRotation);

            // Set rotation
            var setRotation = Intrinsic.Create("setRotation");
            setRotation.AddParam("x", 0.0);
            setRotation.AddParam("y", 0.0);
            setRotation.AddParam("z", 0.0);
            setRotation.code = (context, partialResult) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var objRot = _assetTarget.GetTrait<TraitRotation3d>();
                if (objRot == null)
                {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                _ = objRot.SetValue(new Vector3(x, y, z));
                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setRotation);

            // Get scale
            var getScale = Intrinsic.Create("getScale");
            getScale.code = (_, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(null, true);
                }

                var objScale = _assetTarget.GetTrait<TraitScale3d>();
                if (objScale == null)
                {
                    return new Intrinsic.Result(null, true);
                }

                var scaleMap = new ValMap
                {
                    ["x"] = new ValNumber(objScale.Value.x),
                    ["y"] = new ValNumber(objScale.Value.y),
                    ["z"] = new ValNumber(objScale.Value.z)
                };

                return new Intrinsic.Result(scaleMap);
            };
            _registeredIntrinsics.Add(getScale);

            // Set scale
            var setScale = Intrinsic.Create("setScale");
            setScale.AddParam("x", 1.0);
            setScale.AddParam("y", 1.0);
            setScale.AddParam("z", 1.0);
            setScale.code = (context, partialResult) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var objScale = _assetTarget.GetTrait<TraitScale3d>();
                if (objScale == null)
                {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                _ = objScale.SetValue(new Vector3(x, y, z));
                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setScale);
        }
    }
}
