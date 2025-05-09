using System;
using UnityEngine;
using UnityEditor;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Static utility class responsible for drawing the Login section in the Urt3dNetwork window.
    /// Provides username (email) and password fields with a login button for authentication.
    /// </summary>
    public class Urt3dConfig : EditorWindow
    {
        /// <summary>
        /// Opens the Urt3d Network window.
        /// </summary>
        [MenuItem("URT3D/Configure")]
        public static void ShowWindow()
        { 
            GetWindow<Urt3dConfig>("Configure");
        }
        /// <summary>
        /// Draws the login UI elements that can be called from other windows.
        /// This method renders the login form with email validation and password field.
        /// The Login button will only be enabled when a valid email and non-empty password are provided.
        /// </summary>
        private void OnGUI()
        {
            // Title
            EditorGUILayout.Space();
            GUILayout.Label("Configuration", EditorStyles.boldLabel);

            // App Key field
            EditorGUI.BeginChangeCheck();
            Network.AppKey = EditorGUILayout.TextField("App Key", Network.AppKey);

            // Check if app key is valid (non-empty and starts with "URT3D-")
            //var isValidAppKey = !string.IsNullOrEmpty(Network.AppKey) && Network.AppKey.StartsWith("URT3D-");
            var isValidAppKey = !string.IsNullOrEmpty(Network.AppKey);

            // Show warning if app key is invalid
            if (!string.IsNullOrEmpty(Network.AppKey) && !isValidAppKey)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("App Key must start with 'URT3D-'", MessageType.Warning);
            }
            // If not authenticated, show message
            else if (!Network.IsAuthenticated)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Please validate App Key to access URT3D features.", MessageType.Info);
            }

            // Draw action buttons
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate"))
                {
                    // Open the login window
                    LoginWindow.ShowWindow();
                }
                
                // Disable validate button if app key is not valid
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
        /// Editor window that provides login functionality for URT3D services.
        /// Contains fields for username and password with a Login button.
        /// </summary>
        internal class LoginWindow : EditorWindow
        {
            private string username = "";
            private string password = "";
            private bool isLoggingIn = false;
            private string statusMessage = "";
            private MessageType statusType = MessageType.None;
            private bool isAuthenticated = false;
            
            // Variables for displaying the app key list
            private Vector2 listScrollPosition;
            private string[] appKeyList = Array.Empty<string>();
            private bool isLoadingList = false;

            /// <summary>
            /// Opens the Urt3d Login window as a modal dialog with fixed size.
            /// </summary>
            public static void ShowWindow()
            {
                // Create a modal window with fixed size
                var window = CreateInstance<LoginWindow>();
                window.titleContent = new GUIContent("URT3D Login");
                window.position = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 400);
                window.ShowUtility();
            }

            /// <summary>
            /// Called by Unity to render and handle GUI events.
            /// Renders the login form with username and password fields and a Login button.
            /// After successful login, shows the app key list fetched from the server.
            /// </summary>
            private void OnGUI()
            {
                // Login Section
                EditorGUILayout.LabelField("Login", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                // Username field
                EditorGUI.BeginChangeCheck();
                username = EditorGUILayout.TextField("Username", username);

                // Password field
                password = EditorGUILayout.PasswordField("Password", password);

                // Status message if any
                if (!string.IsNullOrEmpty(statusMessage))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox(statusMessage, statusType);
                }

                // Login button
                EditorGUILayout.Space();
                GUI.enabled = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !isLoggingIn;
                
                if (GUILayout.Button("Login"))
                {
                    PerformLogin();
                }
                
                GUI.enabled = true;

                // Only show the app key list if login was successful
                if (isAuthenticated)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("App Keys", EditorStyles.boldLabel);
                    
                    EditorGUILayout.Space();
                    
                    // Begin the scrollable area
                    listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition, GUILayout.Height(200));
                    
                    // Show loading indicator or empty list message
                    if (isLoadingList)
                    {
                        EditorGUILayout.HelpBox("Loading app key list...", MessageType.Info);
                    }
                    else if (appKeyList.Length == 0)
                    {
                        EditorGUILayout.HelpBox("No app keys found.", MessageType.Info);
                    }
                    else
                    {
                        // Display each app key with action buttons
                        foreach (var appKey in appKeyList)
                        {
                            // Create a horizontal layout for the app key and buttons
                            EditorGUILayout.BeginHorizontal();
                            {
                                // Create a custom style that doesn't wrap text
                                var noWrapStyle = new GUIStyle(EditorStyles.label) { wordWrap = false };

                                // Display the app key with a simple non-wrapping label
                                EditorGUILayout.LabelField(appKey, noWrapStyle, GUILayout.Height(20), GUILayout.Width(position.width - 80));
                                
                                // Add "✗" button
                                if (GUILayout.Button("✗", GUILayout.Width(25), GUILayout.Height(20)))
                                {
                                    // Confirm deletion
                                    if (EditorUtility.DisplayDialog("Delete App Key", 
                                        $"Are you sure you want to delete this app key?\n\n{appKey}", 
                                        "Delete", "Cancel"))
                                    {
                                        // Delete the app key
                                        DeleteAppKey(appKey);
                                    }
                                }
                                
                                // Add "✓" button
                                if (GUILayout.Button("✓", GUILayout.Width(25), GUILayout.Height(20)))
                                {
                                    // Set the app key in the Network
                                    Network.AppKey = appKey;
                                    
                                    // Show brief success message before closing
                                    statusMessage = "App Key set successfully!";
                                    statusType = MessageType.Info;
                                    Repaint();
                                    
                                    // Close the window after a brief delay to show the success message
                                    EditorApplication.delayCall += Close;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    
                    // Add a "+" button at the end of the list to create a new app key
                    // This is outside the empty check so it's always displayed
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace(); // Push the button to the center
                        
                        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(25)))
                        {
                            // Create a new app key
                            CreateNewAppKey();
                        }
                        
                        GUILayout.FlexibleSpace(); // Push the button to the center
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    // End the scrollable area
                    EditorGUILayout.EndScrollView();
                }
            }

            /// <summary>
            /// Performs the login operation using the Network class.
            /// </summary>
            private void PerformLogin()
            {
                isLoggingIn = true;
                statusMessage = "Logging in...";
                statusType = MessageType.Info;
                Repaint();
                
                // Use the Network class for login
                Network.Instance.Login(username, password, HandleLoginResponse);
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
                    isAuthenticated = true;
                    
                    statusMessage = "Login successful!";
                    statusType = MessageType.Info;
                    
                    //Fetch the auth token
                    Network.AppKey = Network.AuthToken;

                    // Fetch the app key list
                    //FetchAppKeyList();
                }
                else
                {
                    // Authentication failed with message from server
                    var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                           : "Invalid credentials or server error";
                    statusMessage = $"Login failed: {errorMsg}";
                    statusType = MessageType.Error;
                }
                
                isLoggingIn = false;
                Repaint();
            }

            /// <summary>
            /// Fetches the list of app keys from the server using the Network class.
            /// </summary>
            private void FetchAppKeyList()
            {
                isLoadingList = true;
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
                    appKeyList = new string[response.appKeys.Length];
                    for (var i = 0; i < response.appKeys.Length; i++)
                    {
                        appKeyList[i] = response.appKeys[i].appKey;
                    }
                }
                else
                {
                    // Error fetching the list
                    var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                           : "Error fetching app key list";
                    Debug.LogError(errorMsg);
                }
                
                isLoadingList = false;
                Repaint();
            }
            
            // LoginResponse class removed as we now use Network.LoginData

            /// <summary>
            /// Creates a new app key using the Network class.
            /// </summary>
            private void CreateNewAppKey()
            {
                isLoadingList = true; // Show loading indicator
                statusMessage = "Creating new app key...";
                statusType = MessageType.Info;
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
                    var newList = new string[appKeyList.Length + 1];
                    for (var i = 0; i < appKeyList.Length; i++)
                    {
                        newList[i] = appKeyList[i];
                    }
                    newList[appKeyList.Length] = response.appKey;
                    appKeyList = newList;
                    
                    statusMessage = "New app key created successfully!";
                    statusType = MessageType.Info;
                }
                else
                {
                    // Error creating app key
                    var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                           : "Error creating new app key";
                    statusMessage = $"Error: {errorMsg}";
                    statusType = MessageType.Error;
                }
                
                isLoadingList = false;
                Repaint();
            }
            
            /// <summary>
            /// Deletes an app key using the Network class.
            /// </summary>
            /// <param name="appKeyToDelete">The app key to delete.</param>
            private void DeleteAppKey(string appKeyToDelete)
            {
                isLoadingList = true; // Show loading indicator
                statusMessage = "Deleting app key...";
                statusType = MessageType.Info;
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
                    var newList = new string[appKeyList.Length - 1];
                    var newIndex = 0;
                    for (var i = 0; i < appKeyList.Length; i++)
                    {
                        if (appKeyList[i] != appKeyToDelete)
                        {
                            newList[newIndex] = appKeyList[i];
                            newIndex++;
                        }
                    }
                    appKeyList = newList;
                    
                    statusMessage = "App key deleted successfully!";
                    statusType = MessageType.Info;
                }
                else
                {
                    // Error deleting app key
                    var errorMsg = !string.IsNullOrEmpty(response.message) ? response.message
                                                                           : "Error deleting app key";
                    statusMessage = $"Error: {errorMsg}";
                    statusType = MessageType.Error;
                }
                
                isLoadingList = false;
                Repaint();
            }
        }
    }
}
