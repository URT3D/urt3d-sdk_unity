using Miniscript;
using UnityEngine;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing asset management functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for asset management
        /// </summary>
        private void RegisterAssetManagementFunctions()
        {
            // Find asset by name
            var findAsset = Intrinsic.Create("findAsset");
            findAsset.AddParam("name");
            findAsset.code = (context, _) => {
                var assetName = context.GetVar("name").ToString();

                // This would need to be implemented based on URT3D asset system
                Debug.Log($"[MiniScript] Finding asset with name: {assetName}");

                // Return placeholder map for asset info
                var assetInfo = new ValMap
                {
                    ["name"] = new ValString(assetName),
                    ["found"] = ValNumber.one // Assume found for placeholder
                };

                return new Intrinsic.Result(assetInfo);
            };
            _registeredIntrinsics.Add(findAsset);

            // Find asset by GUID
            var findAssetByGuid = Intrinsic.Create("findAssetByGuid");
            findAssetByGuid.AddParam("guid");
            findAssetByGuid.code = (context, _) => {
                var guid = context.GetVar("guid").ToString();

                // This would need to be implemented based on URT3D asset system
                Debug.Log($"[MiniScript] Finding asset with GUID: {guid}");

                // Return placeholder map for asset info
                var assetInfo = new ValMap
                {
                    ["guid"] = new ValString(guid),
                    ["found"] = ValNumber.one // Assume found for placeholder
                };

                return new Intrinsic.Result(assetInfo);
            };
            _registeredIntrinsics.Add(findAssetByGuid);

            // Instantiate asset
            var instantiateAsset = Intrinsic.Create("instantiateAsset");
            instantiateAsset.AddParam("assetId");
            instantiateAsset.AddParam("x", 0.0);
            instantiateAsset.AddParam("y", 0.0);
            instantiateAsset.AddParam("z", 0.0);
            instantiateAsset.code = (context, _) => {
                var assetId = context.GetVar("assetId").ToString();
                var x = (float)context.GetVar("x").DoubleValue();
                var y = (float)context.GetVar("y").DoubleValue();
                var z = (float)context.GetVar("z").DoubleValue();

                // This would need to be implemented based on URT3D asset system
                Debug.Log($"[MiniScript] Instantiating asset {assetId} at position ({x}, {y}, {z})");

                // Return placeholder instance ID
                return new Intrinsic.Result(new ValString("instance_12345"));
            };
            _registeredIntrinsics.Add(instantiateAsset);

            // Destroy asset instance
            var destroyAsset = Intrinsic.Create("destroyAsset");
            destroyAsset.AddParam("instanceId");
            destroyAsset.code = (context, _) => {
                var instanceId = context.GetVar("instanceId").ToString();

                // This would need to be implemented based on URT3D asset system
                Debug.Log($"[MiniScript] Destroying asset instance: {instanceId}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(destroyAsset);

            // Get asset visibility
            var getAssetVisibility = Intrinsic.Create("getAssetVisibility");
            getAssetVisibility.code = (_, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                // This would need access to visibility trait
                Debug.Log("[MiniScript] Getting asset visibility");

                return new Intrinsic.Result(ValNumber.one); // Default to visible
            };
            _registeredIntrinsics.Add(getAssetVisibility);

            // Set asset visibility
            var setAssetVisibility = Intrinsic.Create("setAssetVisibility");
            setAssetVisibility.AddParam("visible", ValNumber.one); // true
            setAssetVisibility.code = (context, _) => {
                if (_assetTarget == null) {
                    return new Intrinsic.Result(ValNumber.zero);
                }

                var visible = context.GetVar("visible").BoolValue();

                // This would need to be implemented based on URT3D asset system
                Debug.Log($"[MiniScript] Setting asset visibility to: {visible}");

                return new Intrinsic.Result(ValNumber.one);
            };
            _registeredIntrinsics.Add(setAssetVisibility);
        }
    }
}
