using UnityEngine;
using FungiGame.UI;

namespace FungiGame.Examples
{
    /// <summary>
    /// Example script showing how to use the Enhanced VR UI system
    /// This demonstrates creating better-looking VR buttons and text prompts
    /// </summary>
    public class VRButtonExample : MonoBehaviour
    {
        [Header("UI Manager")]
        [SerializeField] private VRUIManager uiManager;
        
        [Header("Example Settings")]
        [SerializeField] private Transform spawnLocation;
        [SerializeField] private bool createExampleUI = true;
        
        private GameObject mainMenuPanel;
        private GameObject settingsPanel;
        
        private void Start()
        {
            if (createExampleUI)
            {
                CreateExampleUI();
            }
        }
        
        private void CreateExampleUI()
        {
            if (uiManager == null)
            {
                Debug.LogError("VR UI Manager not assigned!");
                return;
            }
            
            // Create main menu panel
            CreateMainMenu();
            
            // Create settings panel (initially hidden)
            CreateSettingsMenu();
        }
        
        private void CreateMainMenu()
        {
            Vector3 panelPosition = spawnLocation != null ? spawnLocation.position : Vector3.forward * 2f;
            
            // Create main menu panel with buttons
            mainMenuPanel = uiManager.CreateVRPanel("Fungi Game Menu", panelPosition, 
                "Start Game", "Settings", "Instructions", "Exit");
            
            // Get the buttons and assign functionality
            EnhancedVRButton[] buttons = mainMenuPanel.GetComponentsInChildren<EnhancedVRButton>();
            
            if (buttons.Length >= 4)
            {
                // Start Game button
                buttons[0].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnStartGame);
                
                // Settings button  
                buttons[1].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnOpenSettings);
                
                // Instructions button
                buttons[2].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnShowInstructions);
                
                // Exit button
                buttons[3].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnExitGame);
                
                // Customize button colors
                buttons[0].SetColors(
                    new Color(0.2f, 0.6f, 0.2f, 0.8f), // Normal - Green
                    new Color(0.3f, 0.8f, 0.3f, 0.9f), // Hover - Brighter Green
                    new Color(0.1f, 0.5f, 0.1f, 1f)    // Pressed - Darker Green
                );
                
                buttons[3].SetColors(
                    new Color(0.6f, 0.2f, 0.2f, 0.8f), // Normal - Red
                    new Color(0.8f, 0.3f, 0.3f, 0.9f), // Hover - Brighter Red
                    new Color(0.5f, 0.1f, 0.1f, 1f)    // Pressed - Darker Red
                );
            }
            
            // Add floating instruction text
            Vector3 instructionPos = panelPosition + Vector3.up * 200f;
            EnhancedVRText instructionText = uiManager.CreateVRText(
                "Look at buttons to highlight, reach out to press", 
                instructionPos, 
                14f
            );
            
            // Make instruction text face the camera and enable fade-in
            instructionText.transform.SetParent(mainMenuPanel.transform);
        }
        
        private void CreateSettingsMenu()
        {
            Vector3 settingsPosition = spawnLocation != null ? 
                spawnLocation.position + Vector3.right * 300f : 
                Vector3.forward * 2f + Vector3.right * 300f;
            
            // Create settings panel
            settingsPanel = uiManager.CreateVRPanel("Settings", settingsPosition,
                "Audio Settings", "Graphics", "Controls", "Back to Menu");
            
            // Initially hide settings panel
            settingsPanel.SetActive(false);
            
            // Setup settings button functionality
            EnhancedVRButton[] settingsButtons = settingsPanel.GetComponentsInChildren<EnhancedVRButton>();
            if (settingsButtons.Length >= 4)
            {
                settingsButtons[0].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnAudioSettings);
                settingsButtons[1].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnGraphicsSettings);
                settingsButtons[2].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnControlSettings);
                settingsButtons[3].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnBackToMainMenu);
            }
        }
        
        // Button event handlers
        private void OnStartGame()
        {
            Debug.Log("Starting game...");
            // Add your game start logic here
            
            // Example: Load game scene
            // SceneManager.LoadScene("GameScene");
            
            // Or: Hide menu and start fungi game
            if (mainMenuPanel != null)
                uiManager.HidePanel(mainMenuPanel);
        }
        
        private void OnOpenSettings()
        {
            Debug.Log("Opening settings...");
            
            if (mainMenuPanel != null)
                uiManager.HidePanel(mainMenuPanel);
                
            if (settingsPanel != null)
                uiManager.ShowPanel(settingsPanel);
        }
        
        private void OnShowInstructions()
        {
            Debug.Log("Showing instructions...");
            
            // Create floating instruction panel
            Vector3 instructionPos = transform.position + Vector3.forward * 400f;
            
            string instructionText = @"Welcome to Fungi Game!

1. Look around to explore different rooms
2. Reach out and grab fungi specimens  
3. Examine them under the microscope
4. Learn about different fungi species
5. Complete challenges to progress

Use your VR controllers to interact with objects.
Point and click on buttons to navigate menus.";
            
            GameObject instructionPanel = CreateInstructionPanel(instructionText, instructionPos);
            
            // Auto-hide after 10 seconds
            StartCoroutine(HideAfterDelay(instructionPanel, 10f));
        }
        
        private void OnExitGame()
        {
            Debug.Log("Exiting game...");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        private void OnAudioSettings()
        {
            Debug.Log("Audio Settings clicked");
            // Implement audio settings UI
        }
        
        private void OnGraphicsSettings()
        {
            Debug.Log("Graphics Settings clicked");
            // Implement graphics settings UI
        }
        
        private void OnControlSettings()
        {
            Debug.Log("Control Settings clicked");
            // Implement control settings UI
        }
        
        private void OnBackToMainMenu()
        {
            Debug.Log("Back to main menu");
            
            if (settingsPanel != null)
                uiManager.HidePanel(settingsPanel);
                
            if (mainMenuPanel != null)
                uiManager.ShowPanel(mainMenuPanel);
        }
        
        private GameObject CreateInstructionPanel(string text, Vector3 position)
        {
            GameObject panel = new GameObject("InstructionPanel");
            panel.transform.SetParent(transform);
            panel.transform.position = position;
            
            // Create text display
            EnhancedVRText vrText = uiManager.CreateVRText(text, Vector3.zero, 16f);
            vrText.transform.SetParent(panel.transform);
            
            // Add close button
            Vector3 closeButtonPos = Vector3.up * 100f;
            EnhancedVRButton closeButton = uiManager.CreateVRButton("Close", closeButtonPos, 
                () => {
                    uiManager.HidePanel(panel);
                });
            closeButton.transform.SetParent(panel.transform);
            
            return panel;
        }
        
        private System.Collections.IEnumerator HideAfterDelay(GameObject panel, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (panel != null)
            {
                uiManager.HidePanel(panel);
            }
        }
        
        // Example of how to create dynamic buttons based on game state
        public void CreateDynamicFungiButtons(string[] fungiNames, Vector3 basePosition)
        {
            for (int i = 0; i < fungiNames.Length; i++)
            {
                Vector3 buttonPos = basePosition + Vector3.up * (i * -60f);
                string fungiName = fungiNames[i];
                
                EnhancedVRButton fungiButton = uiManager.CreateVRButton(fungiName, buttonPos, 
                    () => {
                        OnFungiSelected(fungiName);
                    });
                
                // Customize button appearance based on fungi type
                if (fungiName.Contains("toxic") || fungiName.Contains("poison"))
                {
                    // Red for dangerous fungi
                    fungiButton.SetColors(
                        new Color(0.6f, 0.1f, 0.1f, 0.8f),
                        new Color(0.8f, 0.2f, 0.2f, 0.9f),
                        new Color(0.5f, 0.05f, 0.05f, 1f)
                    );
                }
                else if (fungiName.Contains("edible") || fungiName.Contains("safe"))
                {
                    // Green for safe fungi
                    fungiButton.SetColors(
                        new Color(0.1f, 0.6f, 0.1f, 0.8f),
                        new Color(0.2f, 0.8f, 0.2f, 0.9f),
                        new Color(0.05f, 0.5f, 0.05f, 1f)
                    );
                }
                // Default blue colors for other fungi types
            }
        }
        
        private void OnFungiSelected(string fungiName)
        {
            Debug.Log($"Selected fungi: {fungiName}");
            // Implement fungi selection logic
            // This could trigger fungi spawning, display detailed info, etc.
        }
        
        // Public methods that can be called from other scripts
        public void ShowMainMenu()
        {
            if (mainMenuPanel != null)
                uiManager.ShowPanel(mainMenuPanel);
        }
        
        public void HideMainMenu()
        {
            if (mainMenuPanel != null)
                uiManager.HidePanel(mainMenuPanel);
        }
        
        public void SetUITheme(VRUIManager.VRTheme newTheme)
        {
            if (uiManager != null)
                uiManager.SetTheme(newTheme);
        }
    }
}