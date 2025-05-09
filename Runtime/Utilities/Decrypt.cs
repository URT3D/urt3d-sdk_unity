using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Unity.SharpZipLib.Utils;
using Urt3d.Utilities;

/// <summary>
/// Utility class for extracting and decrypting URT3D asset files.
/// This implementation uses a static approach that doesn't rely on MonoBehaviour
/// lifecycle, making it more reliable during reloads and scene transitions.
/// </summary>
public static class Decrypt
{
    /// <summary>
    /// Extract the contents of a file, handling both encrypted (URt3D) and unencrypted (Spark) formats.
    /// </summary>
    /// <param name="pathToAsset">The path to the asset file to extract</param>
    /// <returns>The path to the extracted directory, or empty string on failure</returns>
    public static async Task<string> Extract(string pathToAsset)
    {
        try
        {
            // Sanity check source file
            if (!File.Exists(pathToAsset))
            {
                Log.Error("Failed to locate asset at: " + pathToAsset);
                return string.Empty;
            }

            // Generate temporary directory for extracted file(s)
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Try to extract without password...
            try 
            {
                ZipFile.ExtractToDirectory(pathToAsset, path, true);
            }
            catch (Exception e1)
            {
                // ...try to obtain decryption key...
                var task = new TaskCompletionSource<string>();
                Network.Instance.Decrypt(pathToAsset, data =>
                {
                    var password = data.isValid ? data.decryption_key : null;
                    
                    // ...try to extract with password...
                    try
                    {
                        ZipUtility.UncompressFromZip(pathToAsset, password, path);
                        task.SetResult(path);
                    }
                    catch (Exception e2)
                    {
                        // ...catch and report all exceptions
                        Log.Error($"{nameof(Decrypt)}: Error(s) while extracting: {pathToAsset}:\n" +
                                  $"  {e1.GetType()}: {e1.Message}\n" +
                                  $"  {e2.GetType()}: {e2.Message}");
                        task.SetResult(string.Empty);
                    }
                });

                return await task.Task;
            }
            return path;
        }
        catch (Exception e)
        {
            Log.Exception(e);
            return string.Empty;
        }
    }
}
