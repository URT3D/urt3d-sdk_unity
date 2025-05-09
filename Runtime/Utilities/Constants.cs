using System.IO;
using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Utilities
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Contains constant values used throughout the URT3D SDK.
    /// Organized into nested classes for better categorization and clarity.
    /// This centralized approach ensures consistency across the framework and
    /// makes it easier to modify values in a single location.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// File extensions used by the URT3D SDK.
        /// These are appended to filenames to identify different types of URT3D assets.
        /// Standardized extensions ensure assets can be correctly identified and handled
        /// by the appropriate loaders and processors.
        /// </summary>
        public static class Extensions
        {
            /// <summary>
            /// File extension for URT3D Asset files (.urta).
            /// Asset files contain individual 3D objects with associated behaviors and properties.
            /// </summary>
            public const string Asset = "urta";
            
            /// <summary>
            /// File extension for URT3D Scene files (.urts).
            /// Scene files contain collections of assets arranged in spatial relationships
            /// with navigation paths and interconnected behaviors.
            /// </summary>
            public const string Scene = "urts";
            
            /// <summary>
            /// File extension for URT3D Experience files (.urte).
            /// Experience files define interactive sequences and user journeys
            /// through multiple scenes and assets.
            /// </summary>
            public const string Experience = "urte";

            /// <summary>
            /// File extension for general-purpose URT3D files (.urt3d).
            /// This is the universal container format that can store any type of URT3D content,
            /// including assets, scenes, or experiences, with auto-detection of content type.
            /// </summary>
            public const string Universal = "urt3d";
        }

        /// <summary>
        /// Common file system paths used by the URT3D SDK.
        /// Provides standardized locations for storing and retrieving URT3D assets.
        /// These paths are platform-aware and adapt to the current runtime environment
        /// to ensure consistent file access across different devices and operating systems.
        /// </summary>
        public static class Paths
        {
            /// <summary>
            /// Path to the URT3D cache directory where downloaded assets are stored.
            /// Located within the application's persistent data path for cross-platform compatibility.
            /// This location persists between application sessions and updates.
            /// 
            /// The cache directory is automatically created if it doesn't exist when the SDK initializes.
            /// It stores both downloaded remote assets and locally generated content for efficient access.
            /// </summary>
            public static readonly string Cache = Path.Combine(Application.persistentDataPath, "Cache");

            // Additional paths could be defined here as the SDK expands
            // such as Temp, Config, User, etc.
        }

        /// <summary>
        /// API endpoint URLs for the URT3D backend services.
        /// Used for network operations like downloading, uploading, and authentication.
        /// These endpoints conform to the URT3D API specification and ensure
        /// compatibility with the cloud-based asset management system.
        /// 
        /// Note: In production environments, these URLs may be configurable
        /// to allow connection to private servers or CDNs.
        /// </summary>
        public static class URLs
        {
            /// <summary>
            /// Base URL for all URT3D API endpoints.
            /// This is the root domain and API version prefix applied to all service endpoints.
            /// Changing this value affects all derived endpoint URLs.
            /// </summary>
            private const string Root = "https://api.urt3d.com/v1.1";
            
            /// <summary>
            /// URL for creating new URT3D assets on the server.
            /// This endpoint accepts POST requests with asset metadata and
            /// returns a unique identifier for the newly created asset.
            /// </summary>
            public static readonly string Create = $"{Root}/create.php";
            
            /// <summary>
            /// URL for decrypting protected URT3D assets.
            /// This endpoint handles authentication-based decryption of assets
            /// that have been encrypted for security or license enforcement.
            /// </summary>
            public static readonly string Decrypt = $"{Root}/decrypt.php";
            
            /// <summary>
            /// URL for downloading URT3D assets from the server.
            /// This endpoint accepts requests with asset identifiers and
            /// delivers the corresponding asset files with appropriate headers.
            /// Supports resumable downloads for large assets and connection interruptions.
            /// </summary>
            public static readonly string Download = $"{Root}/download.php";
            
            /// <summary>
            /// URL for authenticating users with the URT3D service.
            /// This endpoint validates credentials and returns authentication tokens
            /// used for accessing protected assets and services.
            /// Implements OAuth2.0 compatible authentication flow.
            /// </summary>
            public static readonly string Login = $"{Root}/login.php";
            
            /// <summary>
            /// URL for uploading URT3D assets to the server.
            /// This endpoint accepts multipart form data containing asset files and metadata.
            /// Supports chunked uploads for large assets and connection interruptions.
            /// </summary>
            public static readonly string Upload = $"{Root}/upload.php";
        }
    }
}