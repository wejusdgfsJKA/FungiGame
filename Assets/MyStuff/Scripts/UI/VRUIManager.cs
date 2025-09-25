using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace FungiGame.UI
{
    /// <summary>
    /// VR UI Manager that handles layout, spacing, and visual consistency for VR interface elements
    /// Provides utilities for creating better-looking VR UI panels and button layouts
    /// </summary>
    public class VRUIManager : MonoBehaviour
    {
        [Header("Canvas Settings")]
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private float optimalVRScale = 0.001f; // Optimal scale for VR world space canvas
        
        [Header("Layout Settings")]
        [SerializeField] private float buttonSpacing = 20f;
        [SerializeField] private float panelPadding = 30f;
        [SerializeField] private Vector2 defaultButtonSize = new Vector2(300f, 80f);
        
        [Header("Visual Theme")]
        [SerializeField] private VRTheme currentTheme;
        
        [Header("Animation Settings")]
        [SerializeField] private bool enablePanelAnimations = true;
        [SerializeField] private float panelFadeInDuration = 0.5f;
        [SerializeField] private AnimationCurve panelAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Audio")]
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private AudioClip panelOpenSound;
        [SerializeField] private AudioClip panelCloseSound;
        
        private List<EnhancedVRButton> managedButtons = new List<EnhancedVRButton>();
        private List<EnhancedVRText> managedTexts = new List<EnhancedVRText>();
        private Transform playerCamera;
        
        [System.Serializable]
        public class VRTheme
        {
            [Header("Colors")]
            public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            public Color primaryColor = new Color(0.2f, 0.4f, 0.8f, 1f);
            public Color secondaryColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            public Color accentColor = new Color(0.8f, 0.4f, 0.2f, 1f);
            public Color textColor = Color.white;
            public Color textSecondaryColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            
            [Header("Typography")]
            public TMP_FontAsset primaryFont;
            public float headingFontSize = 24f;
            public float bodyFontSize = 18f;
            public float captionFontSize = 14f;
            
            [Header("Effects")]
            public bool enableGlow = true;
            public bool enableOutlines = true;
            public bool enableShadows = true;
        }
        
        private void Awake()
        {
            SetupCanvas();
            FindPlayerCamera();
            SetupAudio();
        }
        
        private void Start()
        {
            ApplyThemeToExistingElements();
        }
        
        private void SetupCanvas()
        {
            if (uiCanvas == null)
                uiCanvas = GetComponent<Canvas>();
                
            if (uiCanvas != null)
            {
                // Configure canvas for optimal VR rendering
                uiCanvas.renderMode = RenderMode.WorldSpace;
                uiCanvas.worldCamera = Camera.main;
                
                // Set optimal scale for VR
                transform.localScale = Vector3.one * optimalVRScale;
                
                // Setup canvas scaler
                if (canvasScaler == null)
                    canvasScaler = uiCanvas.GetComponent<CanvasScaler>();
                    
                if (canvasScaler == null)
                    canvasScaler = uiCanvas.gameObject.AddComponent<CanvasScaler>();
                
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                canvasScaler.scaleFactor = 1f;
            }
        }
        
        private void FindPlayerCamera()
        {
            // Try to find the main camera or VR camera
            if (Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }
            else
            {
                Camera[] cameras = FindObjectsOfType<Camera>();
                foreach (Camera cam in cameras)
                {
                    if (cam.CompareTag("MainCamera") || cam.name.Contains("Camera"))
                    {
                        playerCamera = cam.transform;
                        break;
                    }
                }
            }
        }
        
        private void SetupAudio()
        {
            if (uiAudioSource == null)
            {
                uiAudioSource = gameObject.AddComponent<AudioSource>();
                uiAudioSource.playOnAwake = false;
                uiAudioSource.spatialBlend = 1f; // 3D audio
                uiAudioSource.volume = 0.7f;
            }
        }
        
        /// <summary>
        /// Creates a new VR button with enhanced visual and interaction features
        /// </summary>
        public EnhancedVRButton CreateVRButton(string text, Vector3 position, System.Action onClick = null)
        {
            // Create button GameObject
            GameObject buttonObj = new GameObject($"VRButton_{text}");
            buttonObj.transform.SetParent(transform);
            buttonObj.transform.localPosition = position;
            
            // Add required components
            buttonObj.AddComponent<CanvasRenderer>();
            
            // Add RectTransform
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = defaultButtonSize;
            
            // Add Button component
            Button button = buttonObj.AddComponent<Button>();
            
            // Add Enhanced VR Button component
            EnhancedVRButton vrButton = buttonObj.AddComponent<EnhancedVRButton>();
            
            // Create background image
            GameObject backgroundObj = new GameObject("Background");
            backgroundObj.transform.SetParent(buttonObj.transform);
            backgroundObj.AddComponent<CanvasRenderer>();
            
            RectTransform bgRect = backgroundObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            Image backgroundImage = backgroundObj.AddComponent<Image>();
            backgroundImage.sprite = CreateRoundedRectSprite();
            backgroundImage.type = Image.Type.Sliced;
            
            // Create text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            textObj.AddComponent<CanvasRenderer>();
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = text;
            buttonText.fontSize = currentTheme != null ? currentTheme.bodyFontSize : 18f;
            buttonText.font = currentTheme?.primaryFont;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = currentTheme != null ? currentTheme.textColor : Color.white;
            
            // Create outline
            GameObject outlineObj = new GameObject("Outline");
            outlineObj.transform.SetParent(buttonObj.transform);
            outlineObj.AddComponent<CanvasRenderer>();
            
            RectTransform outlineRect = outlineObj.AddComponent<RectTransform>();
            outlineRect.anchorMin = Vector2.zero;
            outlineRect.anchorMax = Vector2.one;
            outlineRect.sizeDelta = Vector2.one * 4f; // Slightly larger for outline effect
            outlineRect.anchoredPosition = Vector2.zero;
            
            Image outlineImage = outlineObj.AddComponent<Image>();
            outlineImage.sprite = CreateRoundedRectSprite();
            outlineImage.type = Image.Type.Sliced;
            outlineImage.color = new Color(1f, 1f, 1f, 0.3f);
            
            // Move outline behind background
            outlineObj.transform.SetSiblingIndex(0);
            
            // Apply theme
            ApplyThemeToButton(vrButton, backgroundImage, buttonText, outlineImage);
            
            // Setup button click
            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }
            
            // Add to managed list
            managedButtons.Add(vrButton);
            
            return vrButton;
        }
        
        /// <summary>
        /// Creates enhanced VR text with improved readability
        /// </summary>
        public EnhancedVRText CreateVRText(string text, Vector3 position, float fontSize = 18f)
        {
            // Create text GameObject
            GameObject textObj = new GameObject($"VRText_{text.Substring(0, Mathf.Min(10, text.Length))}");
            textObj.transform.SetParent(transform);
            textObj.transform.localPosition = position;
            
            // Add RectTransform
            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400f, 100f);
            
            // Add TextMeshPro
            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.font = currentTheme?.primaryFont;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.color = currentTheme != null ? currentTheme.textColor : Color.white;
            
            // Add Enhanced VR Text component
            EnhancedVRText vrText = textObj.AddComponent<EnhancedVRText>();
            
            // Add to managed list
            managedTexts.Add(vrText);
            
            return vrText;
        }
        
        /// <summary>
        /// Creates a VR panel with automatic layout for buttons
        /// </summary>
        public GameObject CreateVRPanel(string title, Vector3 position, params string[] buttonTexts)
        {
            // Create panel GameObject
            GameObject panelObj = new GameObject($"VRPanel_{title}");
            panelObj.transform.SetParent(transform);
            panelObj.transform.localPosition = position;
            
            // Add RectTransform
            RectTransform panelRect = panelObj.AddComponent<RectTransform>();
            
            // Calculate panel size based on content
            float panelHeight = panelPadding * 2 + 60f; // Title height
            panelHeight += buttonTexts.Length * (defaultButtonSize.y + buttonSpacing);
            float panelWidth = defaultButtonSize.x + panelPadding * 2;
            
            panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
            
            // Add background
            panelObj.AddComponent<CanvasRenderer>();
            Image panelBackground = panelObj.AddComponent<Image>();
            panelBackground.sprite = CreateRoundedRectSprite();
            panelBackground.type = Image.Type.Sliced;
            panelBackground.color = currentTheme != null ? currentTheme.backgroundColor : new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            // Create title
            if (!string.IsNullOrEmpty(title))
            {
                Vector3 titlePos = new Vector3(0f, panelHeight * 0.5f - 60f, 0f);
                EnhancedVRText titleText = CreateVRText(title, titlePos, currentTheme?.headingFontSize ?? 24f);
                titleText.transform.SetParent(panelObj.transform);
            }
            
            // Create buttons
            float startY = panelHeight * 0.5f - 120f; // Account for title
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                Vector3 buttonPos = new Vector3(0f, startY - i * (defaultButtonSize.y + buttonSpacing), 0f);
                EnhancedVRButton button = CreateVRButton(buttonTexts[i], buttonPos);
                button.transform.SetParent(panelObj.transform);
            }
            
            // Add panel animation
            if (enablePanelAnimations)
            {
                StartCoroutine(AnimatePanel(panelObj, true));
            }
            
            return panelObj;
        }
        
        private void ApplyThemeToButton(EnhancedVRButton vrButton, Image background, TextMeshProUGUI text, Image outline)
        {
            if (currentTheme == null) return;
            
            // Apply colors
            vrButton.SetColors(
                currentTheme.secondaryColor,
                currentTheme.primaryColor,
                currentTheme.accentColor
            );
            
            background.color = currentTheme.secondaryColor;
            text.color = currentTheme.textColor;
            text.font = currentTheme.primaryFont;
            text.fontSize = currentTheme.bodyFontSize;
        }
        
        private void ApplyThemeToExistingElements()
        {
            // Apply theme to all managed buttons and texts
            foreach (var button in managedButtons)
            {
                if (button != null && currentTheme != null)
                {
                    button.SetColors(currentTheme.secondaryColor, currentTheme.primaryColor, currentTheme.accentColor);
                }
            }
            
            foreach (var text in managedTexts)
            {
                if (text != null && currentTheme != null)
                {
                    text.SetColor(currentTheme.textColor);
                    text.SetFontSize(currentTheme.bodyFontSize);
                }
            }
        }
        
        private IEnumerator AnimatePanel(GameObject panel, bool fadeIn)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panel.AddComponent<CanvasGroup>();
            
            Vector3 originalScale = panel.transform.localScale;
            Vector3 targetScale = fadeIn ? originalScale : Vector3.zero;
            Vector3 startScale = fadeIn ? Vector3.zero : originalScale;
            
            float targetAlpha = fadeIn ? 1f : 0f;
            float startAlpha = fadeIn ? 0f : 1f;
            
            canvasGroup.alpha = startAlpha;
            panel.transform.localScale = startScale;
            
            // Play sound
            if (uiAudioSource != null)
            {
                AudioClip soundToPlay = fadeIn ? panelOpenSound : panelCloseSound;
                if (soundToPlay != null)
                {
                    uiAudioSource.clip = soundToPlay;
                    uiAudioSource.Play();
                }
            }
            
            float elapsedTime = 0f;
            while (elapsedTime < panelFadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / panelFadeInDuration;
                float curveValue = panelAnimationCurve.Evaluate(t);
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
                panel.transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
            panel.transform.localScale = targetScale;
            
            if (!fadeIn)
            {
                panel.SetActive(false);
            }
        }
        
        private Sprite CreateRoundedRectSprite()
        {
            // Create a simple rounded rectangle sprite
            // This is a simplified implementation - you might want to use actual sprite assets
            Texture2D texture = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(8, 8, 8, 8));
        }
        
        // Public utility methods
        public void SetTheme(VRTheme newTheme)
        {
            currentTheme = newTheme;
            ApplyThemeToExistingElements();
        }
        
        public void ShowPanel(GameObject panel, bool animate = true)
        {
            if (panel != null)
            {
                panel.SetActive(true);
                if (animate && enablePanelAnimations)
                {
                    StartCoroutine(AnimatePanel(panel, true));
                }
            }
        }
        
        public void HidePanel(GameObject panel, bool animate = true)
        {
            if (panel != null)
            {
                if (animate && enablePanelAnimations)
                {
                    StartCoroutine(AnimatePanel(panel, false));
                }
                else
                {
                    panel.SetActive(false);
                }
            }
        }
        
        public void UpdateButtonSpacing(float newSpacing)
        {
            buttonSpacing = newSpacing;
        }
        
        public void UpdateDefaultButtonSize(Vector2 newSize)
        {
            defaultButtonSize = newSize;
        }
        
        private void Update()
        {
            // Optional: Make UI always face the player
            if (playerCamera != null)
            {
                Vector3 directionToCamera = playerCamera.position - transform.position;
                directionToCamera.y = 0; // Keep UI upright
                
                if (directionToCamera != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(directionToCamera);
                }
            }
        }
    }
}