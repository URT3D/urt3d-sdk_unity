using Urt3d;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Urt3d.Traits;

/// <summary>
/// Sample class demonstrating how to load URT3D assets programmatically at runtime.
/// This class provides methods for loading assets from both file paths and GUIDs,
/// using both callback-based and async/await approaches.
/// </summary>
public class AssetLoader : MonoBehaviour
{
    /// <summary>
    /// Loads a URT3D asset from a local file path using a callback approach.
    /// </summary>
    /// <param name="path">The full file path to the .urta asset file to load</param>
    /// <remarks>
    /// This method demonstrates the callback pattern for loading assets.
    /// It sets position and rotation using traits after loading.
    /// </remarks>
    public void LoadFromPath(string path)
    {
        Asset.Construct(path, asset =>
        {
            if (asset == null) return;

            // Set initial position and rotation data
            asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
            asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
                
            // Now you can work with the asset
            Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
        });
    }
    
    /// <summary>
    /// Loads a URT3D asset by its GUID using a callback approach.
    /// </summary>
    /// <param name="guidString">The string representation of the asset's GUID</param>
    /// <remarks>
    /// Use this method when you have a GUID reference to a URT3D asset.
    /// The SDK will handle locating and downloading the asset as needed.
    /// </remarks>
    public void LoadFromGuid(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            Asset.Construct(guid, asset =>
            {
                if (asset == null) return;

                // Set initial position and rotation data
                asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
                asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
                    
                // Now you can work with the asset
                Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
            });
        }
    }

    /// <summary>
    /// Asynchronously loads a URT3D asset from a local file path.
    /// </summary>
    /// <param name="path">The full file path to the .urta asset file to load</param>
    /// <returns>A Task that completes when the asset is loaded</returns>
    /// <remarks>
    /// This method uses C#'s async/await pattern for cleaner code flow.
    /// It's useful when you want to perform additional operations in sequence after loading.
    /// </remarks>
    public async Task LoadFromPathAsync(string path)
    {
        var asset = await Asset.Construct(path);
        if (asset != null)
        {
            // Set initial position and rotation data
            asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
            asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
            
            // Now you can work with the asset
            Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
        }
    }
    
    /// <summary>
    /// Asynchronously loads a URT3D asset by its GUID.
    /// </summary>
    /// <param name="guidString">The string representation of the asset's GUID</param>
    /// <returns>A Task that completes when the asset is loaded</returns>
    /// <remarks>
    /// Combines the benefits of GUID-based loading with the async/await pattern.
    /// The SDK will handle locating and downloading the asset as needed.
    /// </remarks>
    public async Task LoadFromGuidAsync(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            var asset = await Asset.Construct(guid);
            if (asset != null)
            {
                // Set initial position and rotation data
                asset.GetTrait<TraitPosition3d>()?.SetValue(Vector3.zero);
                asset.GetTrait<TraitRotation3d>()?.SetValue(Vector3.zero);
                
                // Now you can work with the asset
                Debug.Log($"Loaded asset: {asset.Metadata.NameInstance}");
            }
        }
    }
}