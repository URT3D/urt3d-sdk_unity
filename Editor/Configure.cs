using System;
using UnityEngine;
using UnityEditor;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Editor window that handles URT3D configuration, including login functionality and app key management.
    /// </summary>
    public class Urt3dConfig : EditorWindow
    {
        // Login variables
        private string _username = "";
        private string _password = "";
        private bool _isLoggingIn = false;
        private string _statusMessage = "";
        private MessageType _statusType = MessageType.None;
        private bool _isAuthenticated = false;
        
        // App key list variables
        private Vector2 _listScrollPosition;
        private string[] _appKeyList = Array.Empty<string>();
        private bool _isLoadingList = false;
        
        // UI state
        private bool _showAuthSection = false;
        
        /// <summary>
        /// Opens the Urt3d Configuration window.
        /// </summary>
        [MenuItem("URT3D/Configure")]
        public static void ShowWindow()
        { 
            GetWindow<Urt3dConfig>("URT3D Configure");
        }
        
        /// <summary>
        /// Called by Unity to render and handle GUI events.
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.Space();
            
            // Main configuration section always visible
            DrawMainConfigurationSection();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            
            // Authentication or App Key Management section
            if (!_isAuthenticated)
            {
                // Show login section only if the login button was pressed
                if (_showAuthSection)
                {
                    DrawLoginSection();
                }
            }
            else
            {
                // If authenticated, show app key management section
                DrawAppKeyListSection();
            }
        }
        
        /// <summary>
        /// Draws the main configuration section.
        /// </summary>
        private void DrawMainConfigurationSection()
        {
            // Title
            GUILayout.Label("URT3D Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Authentication status
            string status = Network.IsAuthenticated ? "Authenticated" : "Not Authenticated";
            string statusColor = Network.IsAuthenticated ? "<color=green>" : "<color=red>";
            
            GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
            statusStyle.richText = true;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status:", GUILayout.Width(100));
            EditorGUILayout.LabelField(statusColor + status + "</color>", statusStyle);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // App Key field
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current App Key:", GUILayout.Width(100));
            Network.AppKey = EditorGUILayout.TextField(Network.AppKey);
            EditorGUILayout.EndHorizontal();

            // Check if app key is valid (non-empty)
            var isValidAppKey = !string.IsNullOrEmpty(Network.AppKey);

            // Show validation messages
            if (!string.IsNullOrEmpty(Network.AppKey) && !isValidAppKey)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("App Key must start with 'URT3D-'", MessageType.Warning);
            }
            else if (!Network.IsAuthenticated)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Please validate App Key to access URT3D features.", MessageType.Info);
            }

            // Draw action buttons
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            {
                // Authenticate button (on the left)
                if (!_isAuthenticated)
                {
                    if (GUILayout.Button("Authenticate"))
                    {
                        _showAuthSection = true;
                        // Focus on username field in the login section
                        GUI.FocusControl("Username");
                    }
                }
                else
                {
                    if (GUILayout.Button("Logout"))
                    {
                        // Implement logout logic here if Network class supports it
                        _isAuthenticated = false;
                        _showAuthSection = false;
                        _statusMessage = "Logged out successfully.";
                        _statusType = MessageType.Info;
                    }
                }
                
                // Add some space between buttons
                GUILayout.Space(10);
                
                // Validate button (on the right)
                using (new EditorGUI.DisabledGroupScope(!isValidAppKey))
                {
                    if (GUILayout.Button("Validate"))
                    {
                        Network.Instance.Validate();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Draws the login section.
        /// </summary>
        private void DrawLoginSection()
        {
            EditorGUILayout.Space();
            
            // Section header
            GUILayout.Label("Authentication", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Username field
            EditorGUI.BeginChangeCheck();
            _username = EditorGUILayout.TextField("Username", _username);

            // Password field
            _password = EditorGUILayout.PasswordField("Password", _password);

            // Status message if any
            if (!string.IsNullOrEmpty(_statusMessage))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(_statusMessage, _statusType);
            }

            // Login button
            EditorGUILayout.Space();
            using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) || _isLoggingIn))
            {
                if (GUILayout.Button("Login"))
                {
                    PerformLogin();
                }
            }
        }
        
        /// <summary>
        /// Draws the app key management section.
        /// </summary>
        private void DrawAppKeyListSection()
        {
            EditorGUILayout.Space();
            
            // Section header
            GUILayout.Label("App Key Management", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Create new app key button and Refresh button (above the list)
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New App Key", GUILayout.Width(150)))
            {
                CreateNewAppKey();
            }
            
            // Refresh button
            using (new EditorGUI.DisabledGroupScope(_isLoadingList))
            {
                if (GUILayout.Button("Refresh List", GUILayout.Width(100)))
                {
                    FetchAppKeyList();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // App Key List
            EditorGUILayout.LabelField("Available App Keys:", EditorStyles.boldLabel);
            
            // Begin the scrollable area
            _listScrollPosition = EditorGUILayout.BeginScrollView(_listScrollPosition, GUILayout.Height(200));
            
            // Show loading indicator or empty list message
            if (_isLoadingList)
            {
                EditorGUILayout.HelpBox("Loading app key list...", MessageType.Info);
            }
            else if (_appKeyList.Length == 0)
            {
                EditorGUILayout.HelpBox("No app keys found.", MessageType.Info);
            }
            else
            {
                // Display each app key with action buttons
                foreach (var appKey in _appKeyList)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        // Check if this is the current app key
                        bool isCurrentKey = appKey == Network.AppKey;
                        string keyPrefix = isCurrentKey ? "✓ " : "";
                        
                        // Create a custom style that doesn't wrap text
                        var keyStyle = new GUIStyle(EditorStyles.label) { wordWrap = false };
                        
                        if (isCurrentKey)
                        {
                            keyStyle.fontStyle = FontStyle.Bold;
                            keyStyle.normal.textColor = new Color(0.0f, 0.75f, 0.0f);
                        }
                        
                        // Display the app key with a styled label
                        EditorGUILayout.LabelField(keyPrefix + appKey, keyStyle);
                        
                        // Action buttons
                        if (!isCurrentKey)
                        {
                            if (GUILayout.Button("Select", GUILayout.Width(60)))
                            {
                                Network.AppKey = appKey;
                                _statusMessage = "App Key selected!";
                                _statusType = MessageType.Info;
                                Repaint();
                            }
                        }
                        
                        // Delete button
                        GUI.backgroundColor = new Color(1.0f, 0.6f, 0.6f); // Light red color
                        if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        {
                            // Confirm deletion
                            if (EditorUtility.DisplayDialog("Delete App Key", 
                                $"Are you sure you want to delete this app key?\n\n{appKey}", 
                                "Delete", "Cancel"))
                            {
                                DeleteAppKey(appKey);
                            }
                        }
                        GUI.backgroundColor = Color.white; // Reset color
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            
            // End the scrollable area
            EditorGUILayout.EndScrollView();
            
            // Status message if any (below the list)
            if (!string.IsNullOrEmpty(_statusMessage))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(_statusMessage, _statusType);
            }
        }
        
        //
        // LOGIN AND APP KEY MANAGEMENT METHODS
        //
        
        /// <summary>
        /// Performs the login operation using the Network class.
        /// </summary>
        private void PerformLogin()
        {
            _isLoggingIn = true;
            _statusMessage = "Logging in...";
            _statusType = MessageType.Info;
            Repaint();
            
            // Use the Network class for login
            Network.Instance.Login(_username, _password, HandleLoginResponse);
        }
        
        /// <summary>
        /// Handles the response from the login operation.
        /// </summary>
        /// <param name="response">The response data from the login operation.</param>
        private void HandleLoginResponse(Network.LoginData response)
        {
            if (response.isValid)
            {
                // Authentication successful
                _isAuthenticated = true;
                
                _statusMessage = "Login successful!";
                _statusType = MessageType.Info;
                
                // Note: We don't set Network.AppKey = Network.AuthToken anymore
                // The Network class already handles setting the AuthToken internally
                
                // Fetch the app key list and handle AppKey selection there
                FetchAppKeyList();
            }
            else
            {
                // Authentication failed with message from server
                var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                      : "Invalid credentials or server error";
                _statusMessage = $"Login failed: {errorMsg}";
                _statusType = MessageType.Error;
            }
            
            _isLoggingIn = false;
            Repaint();
        }

        /// <summary>
        /// Fetches the list of app keys from the server using the Network class.
        /// </summary>
        private void FetchAppKeyList()
        {
            _isLoadingList = true;
            _statusMessage = "Loading app keys...";
            _statusType = MessageType.Info;
            Repaint();

            // Use the Network class to fetch the app key list
            Network.Instance.AppKeyList(Network.AuthToken, HandleAppKeyListResponse);
        }
        
        /// <summary>
        /// Handles the response from app key list retrieval.
        /// </summary>
        /// <param name="response">The response data from the app key list operation.</param>
        private void HandleAppKeyListResponse(Network.AppKeyListData response)
        {
            if (response.isValid && response.appKeys != null)
            {
                // Convert the array of AppKeyInfo objects to an array of strings
                _appKeyList = new string[response.appKeys.Length];
                for (var i = 0; i < response.appKeys.Length; i++)
                {
                    _appKeyList[i] = response.appKeys[i].appKey;
                }
                
                // Handle AppKey selection based on requirements:
                string currentAppKey = Network.AppKey;
                bool currentAppKeyFound = false;
                
                // 1. Check if the current AppKey is in the available list
                if (!string.IsNullOrEmpty(currentAppKey))
                {
                    foreach (var appKey in _appKeyList)
                    {
                        if (appKey == currentAppKey)
                        {
                            currentAppKeyFound = true;
                            break;
                        }
                    }
                }
                
                // 2. If current AppKey not found in the list, make a new selection
                if (!currentAppKeyFound)
                {
                    // If there are app keys available, select the first one
                    if (_appKeyList.Length > 0)
                    {
                        Network.AppKey = _appKeyList[0];
                        _statusMessage = $"Found {_appKeyList.Length} app key(s). First key selected.";
                    }
                    else
                    {
                        // No app keys available, clear the field
                        Network.AppKey = "";
                        _statusMessage = "No app keys found. Please create a new one.";
                    }
                }
                else
                {
                    _statusMessage = $"Found {_appKeyList.Length} app key(s).";
                }
                
                _statusType = MessageType.Info;
            }
            else
            {
                // Error fetching the list
                var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                      : "Error fetching app key list";
                _statusMessage = $"Error: {errorMsg}";
                _statusType = MessageType.Error;
                Debug.LogError(errorMsg);
            }
            
            _isLoadingList = false;
            Repaint();
        }
        
        /// <summary>
        /// Creates a new app key using the Network class.
        /// </summary>
        private void CreateNewAppKey()
        {
            _isLoadingList = true; // Show loading indicator
            _statusMessage = "Creating new app key...";
            _statusType = MessageType.Info;
            Repaint();
            
            // Use the Network class to create a new app key
            Network.Instance.AppKeyCreate(Network.AuthToken, HandleAppKeyCreateResponse);
        }
        
        /// <summary>
        /// Handles the response from app key creation.
        /// </summary>
        /// <param name="response">The response data from the app key creation operation.</param>
        private void HandleAppKeyCreateResponse(Network.AppKeyCreateData response)
        {
            if (response.isValid)
            {
                // Add the new app key to the list
                var newList = new string[_appKeyList.Length + 1];
                for (var i = 0; i < _appKeyList.Length; i++)
                {
                    newList[i] = _appKeyList[i];
                }
                newList[_appKeyList.Length] = response.appKey;
                _appKeyList = newList;
                
                // Automatically set this as the current app key
                Network.AppKey = response.appKey;
                
                _statusMessage = "New app key created and selected!";
                _statusType = MessageType.Info;
            }
            else
            {
                // Error creating app key
                var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                      : "Error creating new app key";
                _statusMessage = $"Error: {errorMsg}";
                _statusType = MessageType.Error;
            }
            
            _isLoadingList = false;
            Repaint();
        }
        
        /// <summary>
        /// Deletes an app key using the Network class.
        /// </summary>
        /// <param name="appKeyToDelete">The app key to delete.</param>
        private void DeleteAppKey(string appKeyToDelete)
        {
            _isLoadingList = true; // Show loading indicator
            _statusMessage = "Deleting app key...";
            _statusType = MessageType.Info;
            Repaint();
            
            // Use the Network class to delete the app key
            Network.Instance.AppKeyDelete(Network.AuthToken, appKeyToDelete, response => HandleAppKeyDeleteResponse(response, appKeyToDelete));
        }
        
        /// <summary>
        /// Handles the response from app key deletion.
        /// </summary>
        /// <param name="response">The response data from the app key deletion operation.</param>
        /// <param name="appKeyToDelete">The app key that was deleted.</param>
        private void HandleAppKeyDeleteResponse(Network.AppKeyDeleteData response, string appKeyToDelete)
        {
            if (response.isValid)
            {
                // Remove the app key from the list
                var newList = new string[_appKeyList.Length - 1];
                var newIndex = 0;
                for (var i = 0; i < _appKeyList.Length; i++)
                {
                    if (_appKeyList[i] != appKeyToDelete)
                    {
                        newList[newIndex] = _appKeyList[i];
                        newIndex++;
                    }
                }
                _appKeyList = newList;
                
                // If we deleted the current app key, clear it
                if (Network.AppKey == appKeyToDelete)
                {
                    Network.AppKey = "";
                }
                
                _statusMessage = "App key deleted successfully!";
                _statusType = MessageType.Info;
            }
            else
            {
                // Error deleting app key
                var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                      : "Error deleting app key";
                _statusMessage = $"Error: {errorMsg}";
                _statusType = MessageType.Error;
            }
            
            _isLoadingList = false;
            Repaint();
        }
    }
}