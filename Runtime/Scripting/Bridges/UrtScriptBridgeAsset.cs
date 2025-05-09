using Miniscript;

namespace Urt3d.Scripting
{
    /// <summary>
    /// Partial UrtScriptBridge class containing asset property functions
    /// </summary>
    public partial class UrtScriptBridge
    {
        /// <summary>
        /// Register functions for accessing asset information
        /// </summary>
        private void RegisterAssetFunctions()
        {
            // Get asset name
            var getAssetName = Intrinsic.Create("getAssetName");
            getAssetName.code = (_, _) =>
                _assetTarget != null ?
                    new Intrinsic.Result(_assetTarget.Metadata.NameInstance) :
                    new Intrinsic.Result(_targetAsset.GetType().Name);
            _registeredIntrinsics.Add(getAssetName);

            // Get asset GUID
            var getAssetGuid = Intrinsic.Create("getAssetGuid");
            getAssetGuid.code = (_, _) =>
                _assetTarget != null ?
                    new Intrinsic.Result(_assetTarget.Metadata.GuidInstance.ToString()) :
                    new Intrinsic.Result(string.Empty);
            _registeredIntrinsics.Add(getAssetGuid);

            // Get asset type
            var getAssetType = Intrinsic.Create("getAssetType");
            getAssetType.code = (_, _) =>
                _assetTarget != null ?
                    new Intrinsic.Result(_assetTarget.Metadata.TypeInstance.Name) :
                    new Intrinsic.Result(_targetAsset.GetType().Name);
            _registeredIntrinsics.Add(getAssetType);
        }
    }
}
