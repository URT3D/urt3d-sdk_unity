using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Provides network communication functionality for the URT3D SDK.
    /// 
    /// This singleton class handles all network operations including authentication,
    /// asset downloading/uploading, and API interactions with the URT3D backend services.
    /// It implements retry logic, progress tracking, and standardized error handling.
    /// 
    /// The class follows a consistent pattern for all network operations:
    /// 1. Public method that accepts parameters and callbacks
    /// 2. Private coroutine that handles the actual network request
    /// 3. Strongly typed result data classes for each operation
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
    public class Network : EditPlayBridge
    {   
        #region Public Properties

        /// <summary>
        /// Singleton instance of the Network class.
        /// Creates a new GameObject with Network component if one doesn't exist.
        /// This property ensures that only one Network instance exists at any time,
        /// managing all network operations through a consistent interface.
        /// </summary>
        public static Network Instance => Singleton.Instance<Network>();

        /// <summary>
        /// Base domain for all API endpoints.
        /// This is the root domain where the URT3D backend services are hosted.
        /// Combined with ApiVersion to form complete endpoint URLs.
        /// </summary>
        public const string ApiDomain = "https://api.urt3d.com";

        /// <summary>
        /// Version string for the API.
        /// Used to construct endpoint URLs and ensure compatibility with the server.
        /// This allows for versioned API changes without breaking existing client implementations.
        /// </summary>
        public const string ApiVersion = "v1.1";
        
        /// <summary>
        /// URT3D App Key for API access.
        /// This key identifies the application making the API requests.
        /// Required for most API operations to authenticate the client application.
        /// Gets the app key from the Urt3dConfig ScriptableObject to persist across sessions.
        /// </summary>
        public static string AppKey
        {
            get
            {
                if (string.IsNullOrEmpty(Configuration.Instance.AppKey))
                {
                    const string msg = "Failed to locate a valid URT3D App Key. " +
                                       "This is required for network operations (Create, Decrypt, Delete, Download, and Upload). " +
                                       "Please configure one via:\n\nMain Menu → URT3D → Configure";
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Missing App Key", msg, "Okay"); 
#endif
                    Log.Error(msg);
                }
                return Configuration.Instance.AppKey;   
            }
            set => Configuration.Instance.AppKey = value;
        }

        /// <summary>
        /// Returns true if the user is currently authenticated.
        /// Authentication status is set after login operations and validated
        /// through the Validate method. This property should be checked before
        /// attempting operations that require user authentication.
        /// </summary>
        public static bool IsAuthenticated { get; private set; } = false;
        
        /// <summary>
        /// Authentication token used for API operations that require authentication.
        /// This token is obtained after a successful login operation and has a limited lifetime.
        /// It should be included in requests that require user identity verification.
        /// </summary>
        public static string AuthToken { get; private set; } = string.Empty;
        
        #endregion

        #region Public Methods: AppKeyCreate
        
        /// <summary>
        /// Data container for app key creation operation results.
        /// Extends the base Data class with app key specific properties.
        /// </summary>
        public class AppKeyCreateData : Data
        {
            /// <summary>
            /// The created app key if the operation was successful.
            /// This key can be used for subsequent API calls that require application authentication.
            /// The key should be stored securely for future use.
            /// </summary>
            public string appKey = string.Empty;
        }
        
        /// <summary>
        /// Creates a new app key using the provided authentication token.
        /// App keys are used to authenticate application-level API requests without requiring user credentials.
        /// Each app key is associated with the user account that created it and has the same permissions.
        /// </summary>
        /// <param name="authToken">The authentication token obtained from login, used to authorize the key creation.</param>
        /// <param name="callback">Callback when creation is completed, includes success status, message, and the created app key.</param>
        public void AppKeyCreate(string authToken, Action<AppKeyCreateData> callback)
        {
            StartCoroutine(AppKeyCreateCoroutine(authToken, callback));
        }
        
        /// <summary>
        /// Coroutine implementation for app key creation.
        /// Handles the HTTP request, response parsing, and callback invocation.
        /// </summary>
        /// <param name="authToken">The authentication token obtained from login.</param>
        /// <param name="callback">Callback when creation is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator AppKeyCreateCoroutine(string authToken, Action<AppKeyCreateData> callback)
        {
            // Populate web-form data
            var form = new WWWForm();
            form.AddField("authToken", authToken);
            
            // Process server request
            var uri = GetURI("appKeyCreate");
            yield return Process<AppKeyCreateData>(uri, form, result =>
            {
                callback?.Invoke(result);
            });
        }
        
        #endregion
        
        #region Public Methods: AppKeyDelete
        
        /// <summary>
        /// Data container for app key deletion operation results.
        /// Extends the base Data class but doesn't add additional properties
        /// as deletion operations only require success/failure status.
        /// </summary>
        public class AppKeyDeleteData : Data
        {
            // No additional properties needed for deletion operations
        }
        
        /// <summary>
        /// Deletes an app key using the provided authentication token.
        /// This permanently revokes access for the specified app key.
        /// Should be used when an app key is no longer needed or may have been compromised.
        /// </summary>
        /// <param name="authToken">The authentication token obtained from login, used to authorize the deletion.</param>
        /// <param name="appKey">The app key to delete.</param>
        /// <param name="callback">Callback when deletion is completed, includes success status and message.</param>
        public void AppKeyDelete(string authToken, string appKey, Action<AppKeyDeleteData> callback)
        {
            StartCoroutine(AppKeyDeleteCoroutine(authToken, appKey, callback));
        }
        
        /// <summary>
        /// Coroutine implementation for app key deletion.
        /// Handles the HTTP request, response parsing, and callback invocation.
        /// </summary>
        /// <param name="authToken">The authentication token obtained from login.</param>
        /// <param name="appKey">The app key to delete.</param>
        /// <param name="callback">Callback when deletion is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator AppKeyDeleteCoroutine(string authToken, string appKey, Action<AppKeyDeleteData> callback)
        {
            // Populate web-form data
            var form = new WWWForm();
            form.AddField("authToken", authToken);
            form.AddField("appKey", appKey);
            
            // Process server request
            var uri = GetURI("appKeyDelete");
            yield return Process<AppKeyDeleteData>(uri, form, result =>
            {
                callback?.Invoke(result);
            });
        }
        
        #endregion

        #region Public Methods: AppKeyList
        
        /// <summary>
        /// Data container for app key list operation results.
        /// Extends the base Data class with properties to hold the list of app keys.
        /// </summary>
        [Serializable]
        public class AppKeyListData : Data
        {
            /// <summary>
            /// Array of app keys returned from the server.
            /// Contains all app keys associated with the authenticated user account.
            /// May be empty if the user has no app keys or if the request fails.
            /// </summary>
            public AppKeyInfo[] appKeys;

            /// <summary>
            /// Individual app key information.
            /// Contains details about a single app key including its value and metadata.
            /// </summary>
            [Serializable]
            public class AppKeyInfo
            {
                /// <summary>
                /// The app key string.
                /// This is the actual key value used for API authentication.
                /// Should be treated as sensitive information.
                /// </summary>
                public string appKey;
            }
        }
        
        /// <summary>
        /// Retrieves a list of app keys associated with the authenticated user.
        /// This allows users to manage their existing app keys, including
        /// identifying keys to delete or determining if new keys need to be created.
        /// </summary>
        /// <param name="authToken">The authentication token obtained from login, used to authorize the request.</param>
        /// <param name="callback">Callback when retrieval is completed, includes success status, message, and the list of app keys.</param>
        public void AppKeyList(string authToken, Action<AppKeyListData> callback)
        {
            StartCoroutine(AppKeyListCoroutine(authToken, callback));
        }
        
        /// <summary>
        /// Coroutine implementation for app key listing.
        /// Handles the HTTP request, response parsing, and callback invocation.
        /// </summary>
        /// <param name="authToken">The authentication token obtained from login.</param>
        /// <param name="callback">Callback when retrieval is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator AppKeyListCoroutine(string authToken, Action<AppKeyListData> callback)
        {
            // Populate web-form data
            var form = new WWWForm();
            form.AddField("authToken", authToken);
            
            // Process server request
            var uri = GetURI("appKeyList");
            yield return Process<AppKeyListData>(uri, form, result =>
            {
                callback?.Invoke(result);
            });
        }
        
        #endregion
                
        #region Public Methods: Create
        
        /// <summary>
        /// Data container for asset creation operation results.
        /// Extends the base Data class with properties specific to created assets.
        /// </summary>
        [Serializable]
        public class CreateData : Data
        {
            /// <summary>
            /// The GUID of the created asset if the operation was successful.
            /// This identifier is used for all subsequent operations on the asset,
            /// including downloading, updating, and deletion.
            /// </summary>
            public string guid;
        }
        
        /// <summary>
        /// Private data container for raw creation operation responses.
        /// Used internally to parse the server response before converting to the public CreateData format.
        /// </summary>
        [Serializable]
        private class CreateDataRaw : Data
        {
            /// <summary>
            /// Metadata about the created asset.
            /// Contains nested information about the asset, including its unique identifier.
            /// </summary>
            public MetadataInfo metadata = new MetadataInfo();

            /// <summary>
            /// Metadata information for a created asset.
            /// Holds detailed properties about the asset structure and identification.
            /// </summary>
            [Serializable]
            public class MetadataInfo
            {
                /// <summary>
                /// The unique identifier of the created asset.
                /// Used to reference this specific asset in future API calls.
                /// </summary>
                public string guid = string.Empty;
            }
        }

        /// <summary>
        /// Creates a new URT3D asset with the provided parameters.
        /// This method uploads asset components (actor, preview) and metadata to create
        /// a complete URT3D asset on the server. The created asset is assigned a unique
        /// GUID that can be used for future operations.
        /// </summary>
        /// <param name="actor">The actor data for the asset (3D model data in GLB format).</param>
        /// <param name="preview">The preview image data for the asset (JPG or PNG format).</param>
        /// <param name="type">The type of asset being created (e.g., "asset", "scene", "experience").</param>
        /// <param name="name">The display name of the asset.</param>
        /// <param name="bundledAssets">JSON data for bundled assets, describing referenced assets.</param>
        /// <param name="bundledScripts">JSON data for bundled scripts, containing behavior definitions.</param>
        /// <param name="callback">Callback when creation is completed, includes success status, message, and the GUID of the created asset.</param>
        public void Create(string actor, string preview, string type, string name, string bundledAssets, string bundledScripts, Action<CreateData> callback)
        {
            StartCoroutine(CreateCoroutine(actor, preview, type, name, bundledAssets, bundledScripts, callback));
        }
        
        /// <summary>
        /// Coroutine implementation for asset creation.
        /// Handles the HTTP request, response parsing, and callback invocation.
        /// </summary>
        /// <param name="actorPath">The actor data for the asset.</param>
        /// <param name="previewPath">The preview image data for the asset.</param>
        /// <param name="type">The type of asset being created.</param>
        /// <param name="name">The name of the asset.</param>
        /// <param name="bundledAssets">JSON data for bundled assets.</param>
        /// <param name="bundledScripts">JSON data for bundled scripts.</param>
        /// <param name="callback">Callback when creation is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator CreateCoroutine(string actorPath, string previewPath, string type, string name, string bundledAssets, string bundledScripts, Action<CreateData> callback)
        {   
            // Try a different approach - use a custom form with explicit content type
            var uri = GetURI("create");


            // Read actor file data
            byte[] actorData = null;
            if(File.Exists(actorPath))
            {
                try
                {
                    actorData = File.ReadAllBytes(actorPath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading file: {ex.Message}");
                }
            }

            // Read preview file data
            byte[] previewData = null;
            if(File.Exists(previewPath))
            {
                try
                {
                    previewData = File.ReadAllBytes(previewPath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading file: {ex.Message}");
                }
            }

            if(actorData == null || previewData == null)
            {
                Debug.LogError("Invalid actor and/or preview data found.");
                yield break;
            }

            // Populate web-form data
            var formData = new WWWForm();
            formData.AddField("token", AppKey);
            formData.AddField("type", type);
            formData.AddField("name", name);
            formData.AddBinaryData("actor", actorData, Path.GetFileNameWithoutExtension(actorPath));
            formData.AddBinaryData("preview", previewData, Path.GetFileNameWithoutExtension(previewPath));
            formData.AddField("bundledAssets", bundledAssets);
            formData.AddField("bundledScripts", bundledScripts);

            yield return Process(uri, formData, prog =>
            {
                //NO-OP
            }, (CreateData data)=>
            {
                if (data.isValid)
                {
                    Debug.Log(data.message);
                }
                else
                {
                    Debug.LogError(data.message);
                }
                callback?.Invoke(data);
            });
        }
        
        #endregion
                
        #region Public Methods: Decrypt
        
        /// <summary>
        /// Data container for asset decryption operation results.
        /// Extends the base Data class but doesn't add additional properties
        /// as decryption operations only require success/failure status.
        /// </summary>
        public class DecryptData : Data
        {
            public string decryption_key;
        }
        
        /// <summary>
        /// Decrypts a URT3D asset using the provided token and MD5 hash.
        /// This method is used for assets that have been protected with encryption.
        /// The decryption is performed by the server using the app's credentials.
        /// </summary>
        /// <param name="path">Fully qualified path to file to be decrypted. Must be an existing file.</param>
        /// <param name="callback">Callback when decryption is completed, includes success status and message.</param>
        public void Decrypt(string path, Action<DecryptData> callback)
        {
            StartCoroutine(DecryptCoroutine(path, callback));
        }
        
        /// <summary>
        /// Coroutine implementation for asset decryption.
        /// Handles file validation, MD5 hash generation, HTTP request, and callback invocation.
        /// </summary>
        /// <param name="path">Fully qualified path to file to be decrypted.</param>
        /// <param name="callback">Callback when decryption is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator DecryptCoroutine(string path, Action<DecryptData> callback)
        {
            // Validate file path
            if (!File.Exists(path))
            {
                callback?.Invoke(new()
                {
                    isValid = false,
                    message = $"Invalid file path: {path}"
                });
            }

            // Note: Using FileStream with binary mode to ensure consistent MD5 calculation
            // across different platforms (Windows/Mac/Linux) regardless of line ending differences
            StringBuilder hash = new();
            try
            {
                using var md5 = MD5.Create();
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var hashBytes = md5.ComputeHash(stream);
                foreach (var hashByte in hashBytes)
                {
                    hash.Append(hashByte.ToString("x2"));
                }
            }
            catch (Exception e)
            {
                callback?.Invoke(new()
                {
                    isValid = false,
                    message = $"Invalid file path: {e}"
                });
            }

            // Populate web-form data
            var form = new WWWForm();
            form.AddField("token", AppKey);
            form.AddField("md5", hash.ToString());
            
            // Process server request
            var uri = GetURI("decrypt");
            yield return Process<DecryptData>(uri, form, result =>
            {
                callback?.Invoke(result);
            });
        }
        
        #endregion

        #region Public Methods: Delete
        
        /// <summary>
        /// Data container for asset deletion operation results.
        /// Extends the base Data class but doesn't add additional properties
        /// as deletion operations only require success/failure status.
        /// </summary>
        public class DeleteData : Data
        {
            // No additional properties needed for deletion operations
        }
        
        /// <summary>
        /// Deletes a URT3D asset with the specified GUID.
        /// This permanently removes the asset from the server.
        /// This operation cannot be undone, so confirmation should be requested from users.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to delete.</param>
        /// <param name="callback">Callback when deletion is completed, includes success status and message.</param>
        public void Delete(Guid guid, Action<DeleteData> callback)
        {
            StartCoroutine(DeleteCoroutine(guid, callback));
        }
        
        /// <summary>
        /// Coroutine implementation for asset deletion.
        /// Handles the HTTP request, response parsing, and callback invocation.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to delete.</param>
        /// <param name="callback">Callback when deletion is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator DeleteCoroutine(Guid guid, Action<DeleteData> callback)
        {
            // Populate web-form data
            var form = new WWWForm();
            form.AddField("token", AppKey);
            form.AddField("guids", guid.ToString());
            
            // Process server request
            var uri = GetURI("delete");
            yield return Process<DeleteData>(uri, form, result =>
            {
                callback?.Invoke(result);
            });
        }
        
        #endregion

        #region Public Methods: Download
        
        /// <summary>
        /// Data container for file download operation results.
        /// Extends the base Data class with file-specific properties.
        /// </summary>
        public class FileData : Data
        {
            /// <summary>
            /// The fully qualified path where the downloaded file was saved.
            /// This path can be used to access the file after download.
            /// Only valid if isValid is true.
            /// </summary>
            public string path;
        }

        /// <summary>
        /// Downloads a URT3D asset with the specified GUID using App Key for authentication.
        /// This method handles the complete download process, including progress reporting,
        /// file writing, and error handling. Downloads are cached in the specified location
        /// defined by Constants.Paths.Cache.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to download.</param>
        /// <param name="onStatus">Callback for download progress updates (0.0 to 1.0), can be used for UI progress bars.</param>
        /// <param name="onComplete">Callback when download is completed, includes success status, message, and file path.</param>
        public void Download(Guid guid, Action<float> onStatus, Action<FileData> onComplete)
        {
            StartCoroutine(DownloadCoroutine(guid, onStatus, onComplete));
        }
        
        /// <summary>
        /// Coroutine implementation for asset downloading.
        /// Handles the HTTP request, file writing, progress tracking, and callback invocation.
        /// </summary>
        /// <param name="guid">The unique identifier of the asset to download.</param>
        /// <param name="onStatus">Callback for download progress updates.</param>
        /// <param name="onComplete">Callback when download is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator DownloadCoroutine(Guid guid, Action<float> onStatus, Action<FileData> onComplete)
        {
            float size = default; // Get size of specified asset
            //yield return GetSizeCoroutine(guid, tmp => { size = tmp.size; });
            size = Mathf.Max(1f, size); // Prevent divide-by-zero errors

            // Populate web-form data
            var form = new WWWForm();
            form.AddField("token", AppKey);
            form.AddField("guids",   guid.ToString());

            var dir = Constants.Paths.Cache;
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var path = Path.Combine(dir, $"{guid}.{Constants.Extensions.Asset}");

            var uri = GetURI("download");
            yield return Process<FileData>(uri, form, size, path, onStatus, result =>
            {
                if (result.isValid)
                {
                    result.path = path;
                    result.message = "Asset Located Successfully";
                }
                onComplete?.Invoke(result);
            });
        }

        #endregion

        #region Public Methods: Login
        
        /// <summary>
        /// Data container for login operation results.
        /// Extends the base Data class with authentication-specific properties.
        /// </summary>
        [Serializable]
        public class LoginData : Data
        {
            /// <summary>
            /// The app key associated with the authenticated user.
            /// Can be used for application-level authentication.
            /// </summary>
            public string appKey = string.Empty;
            
            /// <summary>
            /// The authentication token to be used for subsequent API calls.
            /// This token has a limited lifetime and is tied to the user's session.
            /// Should be included in requests that require user identity verification.
            /// </summary>
            public string authToken = string.Empty;
        }
        
        /// <summary>
        /// Authenticates a user with the provided credentials.
        /// Upon successful authentication, updates the global IsAuthenticated, AppKey,
        /// and AuthToken properties for use in subsequent API calls. This is typically
        /// the first call made when interacting with the URT3D API.
        /// </summary>
        /// <param name="username">The username (email) for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <param name="callback">Callback when authentication is completed, includes success status, message, app key, and auth token.</param>
        public void Login(string username, string password, Action<LoginData> callback)
        {
#if UNITY_EDITOR
            Debug.Log($"Login Process Triggered");
            EditorCoroutineUtility.StartCoroutineOwnerless(LoginCoroutine(username, password, callback));
            Debug.Log($"Login Process Done");
#else
            StartCoroutine(LoginCoroutine(username, password, callback));
#endif
        }
        
        /// <summary>
        /// Coroutine implementation for user login.
        /// Handles the HTTP request, response parsing, authentication state updates, and callback invocation.
        /// </summary>
        /// <param name="username">The username (email) for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <param name="callback">Callback when authentication is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator LoginCoroutine(string username, string password, Action<LoginData> callback)
        {
            // Populate web-form data
            var form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            
            // Process server request
            var uri = GetURI("login");
            yield return Process<LoginData>(uri, form, result =>
            {
                if (result.isValid)
                {
                    // Store the app key and authentication token for subsequent API calls
                    AppKey = result.appKey;
                    AuthToken = result.authToken;
                    IsAuthenticated = true;
                }
                else
                {
                    // Clear authentication state on failure
                    AuthToken = string.Empty;
                    IsAuthenticated = false;
                }
                
                callback?.Invoke(result);
            });
        }
        
        #endregion

        #region Public Methods: Upload
        
        /// <summary>
        /// Data container for upload operation results.
        /// Extends the base Data class with properties specific to uploaded assets.
        /// </summary>
        [Serializable]
        public class UploadData : Data
        {
            /// <summary>
            /// The GUID of the uploaded asset if the operation was successful.
            /// This identifier is used for all subsequent operations on the asset.
            /// </summary>
            public string guid = string.Empty;
        }
        
        /// <summary>
        /// Uploads a file to the URT3D server.
        /// This method handles the complete upload process, including file reading,
        /// progress reporting, and error handling. The file is uploaded with the current
        /// AppKey for authentication.
        /// </summary>
        /// <param name="filePath">The path to the file to upload. Must be an existing file.</param>
        /// <param name="onStatus">Callback for upload progress updates (0.0 to 1.0), can be used for UI progress bars.</param>
        /// <param name="onComplete">Callback when upload is completed, includes success status, message, and the GUID of the uploaded asset.</param>
        public void Upload(string filePath, Action<float> onStatus, Action<UploadData> onComplete)
        {
            StartCoroutine(UploadCoroutine(filePath, onStatus, onComplete));
        }
        
        /// <summary>
        /// Coroutine implementation for file uploading.
        /// Handles file validation, file reading, HTTP request, progress tracking, and callback invocation.
        /// </summary>
        /// <param name="filePath">The path to the file to upload.</param>
        /// <param name="onStatus">Callback for upload progress updates.</param>
        /// <param name="onComplete">Callback when upload is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator UploadCoroutine(string filePath, Action<float> onStatus, Action<UploadData> onComplete)
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                onComplete?.Invoke(new UploadData { isValid = false, message = "File not found" });
                yield break;
            }
            
            // Read file data
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                onComplete?.Invoke(new UploadData { isValid = false, message = $"Error reading file: {ex.Message}" });
                yield break;
            }
            
            // Get file name and extension
            var fileName = Path.GetFileName(filePath);
            
            // Create a form with the file and token
            var form = new WWWForm();
            form.AddBinaryData("file", fileData, fileName);
            form.AddField("token", AppKey);
            
            // Process server request with progress tracking
            var uri = GetURI("upload");
            yield return Process(uri, form, onStatus, onComplete);
        }
        
        #endregion
        
        #region Public Methods: Validate

        /// <summary>
        /// Data container for validation operation results.
        /// Extends the base Data class but doesn't add additional properties
        /// as validation operations only require success/failure status.
        /// </summary>
        public class ValidateData : Data
        {
            // No additional properties needed for validation operations
        }

        /// <summary>
        /// Validates the current App Key and updates authentication status.
        /// This simplified overload automatically updates the global IsAuthenticated property
        /// based on the validation result. Use this for simple validation without
        /// custom callback handling.
        /// </summary>
        public void Validate()
        {
            Validate(data =>
            {
                IsAuthenticated = data.isValid;
            });
        }

        /// <summary>
        /// Validates the current App Key with a custom callback handler.
        /// This can be used to perform more complex operations based on validation results,
        /// such as UI updates, error handling, or automatic login attempts.
        /// </summary>
        /// <param name="callback">Callback when validation is completed, includes success status and message.</param>
        public void Validate(Action<ValidateData> callback)
        {
            StartCoroutine(ValidateCoroutine(callback));
        }
        
        /// <summary>
        /// Coroutine implementation for app key validation.
        /// Handles the HTTP request, response parsing, and callback invocation.
        /// </summary>
        /// <param name="callback">Callback when validation is completed.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator ValidateCoroutine(Action<ValidateData> callback)
        {
            // Populate web-form data
            var form = new WWWForm();
            form.AddField("token", AppKey);

            // Process server request
            var uri = GetURI("validate");
            yield return Process<ValidateData>(uri, form, result =>
            {
                callback?.Invoke(result);
            });
        }
        
        #endregion
        
        #region Private Structures
        
        /// <summary>
        /// Base data container for network operation results.
        /// Contains common properties for success status and message.
        /// All operation-specific data classes derive from this base class.
        /// </summary>
        public class Data
        {
            /// <summary>
            /// Indicates whether the network operation was successful.
            /// True if the operation completed without errors, false otherwise.
            /// This should be checked before attempting to use any other properties.
            /// </summary>
            public bool isValid = false;
            
            /// <summary>
            /// Contains a human-readable message about the operation result.
            /// May contain error details if the operation failed (isValid is false).
            /// Can be displayed directly to users or logged for debugging.
            /// </summary>
            public string message = string.Empty;
        }
        
        #endregion

        #region Private Methods: Process

        /// <summary>
        /// Processes a standard API request without file download.
        /// Handles retries, error handling, and JSON deserialization.
        /// This is the core method for most API operations that don't involve file transfers.
        /// </summary>
        /// <typeparam name="T">The type of data to return, must derive from Data class.</typeparam>
        /// <param name="uri">The endpoint URI to send the request to.</param>
        /// <param name="form">Web form data to be sent in the request.</param>
        /// <param name="callback">Callback to execute when the request completes.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator Process<T>(string uri, WWWForm form, Action<T> callback) where T : Data, new()
        {   
            // Try multiple times, in case there's a network hiccup
            for (var i = 10; i >= 0; --i)
            {
                // Post data to PHP endpoint on network
                using var www = UnityWebRequest.Post(uri, form);
                
                yield return SendWebRequest(www, () =>
                {
                    // NO-OP
                });

                // Was the server processing successful?
                if (www.result == UnityWebRequest.Result.Success)
                {
                    var data = www.downloadHandler.text;
                    try
                    {
                        var result = JsonUtility.FromJson<T>(data);
                        callback?.Invoke(result);
                    }
                    catch
                    {
                        callback?.Invoke(new T { message = data });
                    }
                    yield break;
                }

                // Operation failed, either try again or bail out
                if (i > 0)
                {
                    yield return null;
                }
                else
                {
                    callback?.Invoke(new T { message = GetError(www) });
                    yield break;
                }
            }
        }
        
        /// <summary>
        /// Processes an API request that uploads a file with progress tracking.
        /// Handles retries, upload progress reporting, and error handling.
        /// Used for operations that involve sending larger data to the server with progress feedback.
        /// </summary>
        /// <typeparam name="T">The type of data to return, must derive from Data class.</typeparam>
        /// <param name="uri">The endpoint URI to send the request to.</param>
        /// <param name="form">Web form data to be sent in the request, including file data.</param>
        /// <param name="onStatus">Callback for upload progress updates (0.0 to 1.0).</param>
        /// <param name="onComplete">Callback to execute when the request completes.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator Process<T>(string uri, WWWForm form, Action<float> onStatus, Action<T> onComplete) where T : Data, new()
        {
            // Try multiple times, in case there's a network hiccup
            for (var i = 10; i >= 0; --i)
            {
                // Post data to PHP endpoint on network
                using var www = UnityWebRequest.Post(uri, form);

                // Send the web request in coroutine
                yield return SendWebRequest(www, () =>
                {
                    onStatus?.Invoke(www.uploadProgress);
                });
                Debug.Log($"Raw response data: \n{uri}\n{www.result}\n{www.downloadHandler.text}\n{GetError(www)}");
                
                // Was the server processing successful?
                if (www.result == UnityWebRequest.Result.Success)
                {
                    var data = www.downloadHandler.text;
                    try
                    {
                        var result = JsonUtility.FromJson<T>(data);
                        onComplete?.Invoke(result);
                    }
                    catch
                    {
                        onComplete?.Invoke(new T { isValid = false, message = data });
                    }
                    yield break;
                }

                // Operation failed, either try again or bail out
                if (i > 0)
                {
                    yield return null;
                }
                else
                {
                    onComplete?.Invoke(new T { isValid = false, message = GetError(www) });
                    yield break;
                }
            }
        }
        
        /// <summary>
        /// Processes an API request that downloads a file.
        /// Handles retries, progress reporting, file writing, and error handling.
        /// Used for operations that retrieve files from the server and save them locally.
        /// </summary>
        /// <typeparam name="T">The type of data to return, must derive from Data class.</typeparam>
        /// <param name="uri">The endpoint URI to send the request to.</param>
        /// <param name="form">Web form data to be sent in the request.</param>
        /// <param name="size">Expected file size in bytes, used for progress calculation.</param>
        /// <param name="path">Local filesystem path where the downloaded file will be saved.</param>
        /// <param name="onStatus">Callback for progress updates (0.0 to 1.0).</param>
        /// <param name="onComplete">Callback to execute when the request completes.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator Process<T>(string uri, WWWForm form, float size, string path, Action<float> onStatus, Action<T> onComplete) where T : Data, new()
        {
            // Try multiple times, in case there's a network hiccup
            for (var i = 10; i >= 0; --i)
            {
                // Post data to PHP script on network
                using var www = UnityWebRequest.Post(uri, form);
                www.downloadHandler = new DownloadHandlerFile(path);

                // Send the web request in coroutine
                yield return SendWebRequest(www, () =>
                {
                    onStatus?.Invoke(www.downloadedBytes / size);
                });

                // Was the server processing successful?
                if (www.result == UnityWebRequest.Result.Success)
                {
                    if (www.downloadedBytes >= 1024)
                    {
                        onComplete?.Invoke(new T { isValid = true });
                    }
                    else
                    {
                        File.Delete(path);
                        onComplete?.Invoke(new T { message = GetError(www) });
                    }
                    yield break;
                }

                // Operation failed, either try again or bail out
                if (i > 0)
                {
                    yield return null;
                }
                else
                {
                    File.Delete(path);
                    onComplete?.Invoke(new T { message = GetError(www) });
                    yield break;
                }
            }
        }

        /// <summary>
        /// Sends a web request and monitors its progress.
        /// Provides a standardized way to handle web requests with progress tracking,
        /// calling the provided callback on each frame while the request is in progress.
        /// </summary>
        /// <param name="www">The UnityWebRequest to send.</param>
        /// <param name="onFrame">Callback to execute on each frame while the request is in progress.</param>
        /// <returns>IEnumerator for Unity's coroutine system.</returns>
        private IEnumerator SendWebRequest(UnityWebRequest www, Action onFrame)
        {
            // Send the web request asynchronously
            yield return www.SendWebRequest();

            // Wait for web request to complete
            while (!www.isDone)
            {
                onFrame?.Invoke();
                yield return null;
            }
        }
        
        #endregion
        
        #region Private Methods: Utilities

        /// <summary>
        /// Gets a more robust error message from UnityWebRequest.
        /// Provides user-friendly error messages for common network issues
        /// instead of raw technical errors.
        /// </summary>
        /// <param name="www">The web request to get the error from.</param>
        /// <returns>A human-readable error message suitable for display to users.</returns>
        private static string GetError(UnityWebRequest www)
        {
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                return "Unable to connect to server, please check your internet connection";
            }
            return www.error;
        }

        /// <summary>
        /// Converts the given endpoint into a fully-qualified URI.
        /// Combines the API domain, version, endpoint name, and extension
        /// into a complete URL for API requests.
        /// </summary>
        /// <param name="endpoint">The API endpoint name without extension.</param>
        /// <returns>A fully qualified URL to the endpoint, ready for HTTP requests.</returns>
        private static string GetURI(string endpoint)
        {
            return $"{ApiDomain}/{ApiVersion}/{endpoint}.php";
        }

        #endregion
    }
}
