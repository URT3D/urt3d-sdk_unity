using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Provides utility methods for locating Urt3d assets in both local and remote storage.
    /// This class handles the discovery of assets by GUID, with configurable search strategies
    /// for balancing performance, network usage, and file access patterns.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class AssetFinder
    {
        /// <summary>
        /// Defines the possible sources from which to look for assets.
        /// Determines the search strategy when locating assets by GUID.
        /// </summary>
        public enum Source
        {
            /// <summary>
            /// Search only in local storage, without checking remote sources.
            /// This is fastest but will fail if the asset isn't already cached locally.
            /// </summary>
            Local_Only,
            
            /// <summary>
            /// Search only in remote storage, without checking local cache.
            /// Always downloads a fresh copy, potentially consuming unnecessary bandwidth.
            /// </summary>
            Remote_Only,
            
            /// <summary>
            /// First search locally, then fall back to remote storage if the asset isn't found.
            /// Prioritizes speed but ensures assets are always available if they exist remotely.
            /// </summary>
            Local_then_Remote,
            
            /// <summary>
            /// First search remotely for the latest version, then fall back to local storage if remote access fails.
            /// Prioritizes having the most up-to-date assets at the cost of potential network overhead.
            /// </summary>
            Remote_then_Local
        }

        /// <summary>
        /// Checks if a file path points to a valid, existing asset file.
        /// </summary>
        /// <param name="path">The fully qualified path to check for existence and validity</param>
        /// <returns>True if the path is non-empty and the file exists, false otherwise</returns>
        public static bool IsValid(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        /// <summary>
        /// Asynchronously finds the path to an asset based on its GUID, using the default search strategy.
        /// This method uses Remote_then_Local as the default search approach to ensure the most recent
        /// version of an asset is always retrieved when available.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to find</param>
        /// <returns>A Task that resolves to the fully qualified path to the asset, or null if not found</returns>
        public static async Task<string> Find(Guid guid)
        {
            return await Find(guid, Source.Remote_then_Local);
        }

        /// <summary>
        /// Finds the path to an asset based on its GUID, using the default search strategy and providing a callback.
        /// Uses the Remote_then_Local strategy as a default search approach to ensure the most recent
        /// version of an asset is always retrieved when available.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to find</param>
        /// <param name="callback">Action to invoke when the search completes, with the asset path as parameter</param>
        public static void Find(Guid guid, Action<string> callback)
        {
            Find(guid, Source.Remote_then_Local, callback);
        }

        /// <summary>
        /// Asynchronously finds the path to an asset based on its GUID, searching using the specified source strategy.
        /// This method internally uses a callback-based approach and waits for completion, making it suitable for
        /// await-based calling patterns.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to find</param>
        /// <param name="source">The location strategy to use when searching for the asset</param>
        /// <returns>A Task that resolves to the fully qualified path to the asset, or null if not found</returns>
        public static async Task<string> Find(Guid guid, Source source)
        {
            var path = "";
            var isDone = false;
            Find(guid, source, tmp =>
            {
                path = tmp;
                isDone = true;
            });

            while (!isDone)
            {
                await Task.Delay(1);
            }
            return path;
        }

        /// <summary>
        /// Finds the path to an asset based on its GUID, searching using the specified source strategy.
        /// This callback-based method delegates to the appropriate search strategy based on the source parameter
        /// and invokes the provided callback upon completion.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to find</param>
        /// <param name="source">The location strategy to use when searching for the asset</param>
        /// <param name="callback">Action to invoke when the search completes, with the asset path as parameter</param>
        public static void Find(Guid guid, Source source, Action<string> callback)
        {
            switch (source)
            {
                case Source.Local_Only:
                    FindLocal(guid, callback);
                    break;

                case Source.Remote_Only:
                    FindRemote(guid, callback);
                    break;

                case Source.Local_then_Remote:
                    FindLocal(guid, path =>
                    {
                        if (IsValid(path))
                        {
                            callback?.Invoke(path);
                        }
                        else
                        {
                            FindRemote(guid, callback);
                        }
                    });
                    break;

                case Source.Remote_then_Local:
                    FindRemote(guid, path =>
                    {
                        if (IsValid(path))
                        {
                            callback?.Invoke(path);
                        }
                        else
                        {
                            FindLocal(guid, callback);
                        }
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        /// <summary>
        /// Searches for an asset in the local cache directory.
        /// Constructs the appropriate filename pattern based on GUID and searches for matching files.
        /// Invokes the provided callback with the first matching file path, or null if no matches are found.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to find</param>
        /// <param name="callback">Action to invoke when the search completes, with the asset path as parameter</param>
        private static void FindLocal(Guid guid, Action<string> callback)
        {
            var files = Directory.GetFiles(Constants.Paths.Cache, $"{guid}.*");
            callback?.Invoke(files.Any() ? files.First() : null);
        }

        /// <summary>
        /// Searches for an asset in the remote/network location.
        /// Initiates a download request to the Network service and provides progress updates during download.
        /// Invokes the provided callback with the path to the downloaded asset upon completion,
        /// or null if the download fails or the asset is not found remotely.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to find</param>
        /// <param name="callback">Action to invoke when the search completes, with the asset path as parameter</param>
        private static void FindRemote(Guid guid, Action<string> callback)
        {
            Network.Instance.Download(guid, status =>
            {
                // TODO Provide feedback to user as file downloads
            }, data =>
            {
                callback?.Invoke(data.isValid ? data.path : null);
            });
        }
    }
}