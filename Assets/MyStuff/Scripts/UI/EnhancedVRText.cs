using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FungiGame.UI
{
    /// <summary>
    /// Enhanced VR Text component that provides better readability and visual effects for VR environments
    /// Handles dynamic text scaling, outline effects, and optimal VR text rendering
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class EnhancedVRText : MonoBehaviour
    {
        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private TextMeshProUGUI shadowText;
        
        [Header("VR Optimization")]
        [SerializeField] private bool autoScaleForVR = true;
        [SerializeField] private float vrScaleFactor = 1.5f;
        [SerializeField] private bool enhanceReadability = true;
        [SerializeField] private float readabilityDistance = 5f;
        
        [Header("Visual Effects")]
        [SerializeField] private bool enableOutline = true;
        [SerializeField] private Color outlineColor = Color.black;
        [SerializeField] private float outlineWidth = 0.2f;
        
        [SerializeField] private bool enableShadow = true;
        [SerializeField] private Vector2 shadowOffset = new Vector2(2f, -2f);
        [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.5f);
        
        [SerializeField] private bool enableGlow = false;
        [SerializeField] private Color glowColor = Color.white;
        [SerializeField] private float glowStrength = 0.5f;
        
        [Header("Animation")]
        [SerializeField] private bool enableTypewriterEffect = false;
        [SerializeField] private float typewriterSpeed = 50f; // Characters per second
        [SerializeField] private bool enableFadeIn = true;
        [SerializeField] private float fadeInDuration = 0.5f;
        
        [Header("Interactive Features")]
        [SerializeField] private bool enableLookAtCamera = false;
        [SerializeField] private bool smoothLookAt = true;
        [SerializeField] private float lookAtSpeed = 2f;
        
        // Private variables
        private string originalText;
        private float originalFontSize;
        private Transform cameraTransform;
        private CanvasGroup canvasGroup;
        private Coroutine typewriterCoroutine;
        private Coroutine fadeCoroutine;
        
        // Material property IDs for performance
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
        private static readonly int GlowPower = Shader.PropertyToID("_GlowPower");
        
        private void Awake()
        {
            // Auto-assign components
            if (mainText == null)
                mainText = GetComponent<TextMeshProUGUI>();
                
            // Create canvas group for fading
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
            // Store original values
            if (mainText != null)
            {
                originalText = mainText.text;
                originalFontSize = mainText.fontSize;
            }
            
            // Find camera
            if (enableLookAtCamera)
            {
                cameraTransform = Camera.main?.transform;
                if (cameraTransform == null)
                {
                    cameraTransform = FindObjectOfType<Camera>()?.transform;
                }
            }
        }
        
        private void Start()
        {
            SetupTextForVR();
            SetupVisualEffects();
            
            if (enableFadeIn)
            {
                StartFadeIn();
            }
            
            if (enableTypewriterEffect && !string.IsNullOrEmpty(originalText))
            {
                StartTypewriterEffect();
            }
        }
        
        private void Update()
        {
            if (enableLookAtCamera && cameraTransform != null)
            {
                HandleLookAtCamera();
            }
        }
        
        private void SetupTextForVR()
        {
            if (mainText == null) return;
            
            // Optimize text settings for VR
            if (autoScaleForVR)
            {
                mainText.fontSize = originalFontSize * vrScaleFactor;
            }
            
            if (enhanceReadability)
            {
                // Use more readable font rendering for VR
                mainText.fontSharedMaterial = GetOptimizedTextMaterial();
                
                // Adjust text properties for better VR readability
                mainText.lineSpacing = 1.2f;
                mainText.characterSpacing = 1f;
                mainText.wordSpacing = 2f;
            }
        }
        
        private Material GetOptimizedTextMaterial()
        {
            // Create or get optimized material for VR text rendering
            if (mainText.fontSharedMaterial != null)
            {
                Material optimizedMaterial = new Material(mainText.fontSharedMaterial);
                
                // Apply VR-optimized settings
                if (enableOutline)
                {
                    optimizedMaterial.EnableKeyword("OUTLINE_ON");
                    optimizedMaterial.SetColor(OutlineColor, outlineColor);
                    optimizedMaterial.SetFloat(OutlineWidth, outlineWidth);
                }
                
                if (enableGlow)
                {
                    optimizedMaterial.EnableKeyword("GLOW_ON");
                    optimizedMaterial.SetColor(GlowColor, glowColor);
                    optimizedMaterial.SetFloat(GlowPower, glowStrength);
                }
                
                return optimizedMaterial;
            }
            
            return null;
        }
        
        private void SetupVisualEffects()
        {
            if (enableShadow && shadowText == null)
            {
                CreateShadowText();
            }
        }
        
        private void CreateShadowText()
        {
            // Create shadow text GameObject
            GameObject shadowObj = new GameObject("ShadowText");
            shadowObj.transform.SetParent(transform);
            
            // Copy and setup shadow text component
            shadowText = shadowObj.AddComponent<TextMeshProUGUI>();
            shadowText.text = mainText.text;
            shadowText.font = mainText.font;
            shadowText.fontSize = mainText.fontSize;
            shadowText.fontStyle = mainText.fontStyle;
            shadowText.color = shadowColor;
            shadowText.alignment = mainText.alignment;
            
            // Position shadow
            RectTransform shadowRect = shadowText.rectTransform;
            RectTransform mainRect = mainText.rectTransform;
            
            shadowRect.anchorMin = mainRect.anchorMin;
            shadowRect.anchorMax = mainRect.anchorMax;
            shadowRect.anchoredPosition = mainRect.anchoredPosition + shadowOffset;
            shadowRect.sizeDelta = mainRect.sizeDelta;
            
            // Put shadow behind main text
            shadowRect.SetSiblingIndex(0);
            
            // Disable raycast target for shadow
            shadowText.raycastTarget = false;
        }
        
        private void HandleLookAtCamera()
        {
            if (cameraTransform == null) return;
            
            Vector3 targetDirection = cameraTransform.position - transform.position;
            targetDirection.y = 0; // Keep text upright
            
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                
                if (smoothLookAt)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookAtSpeed * Time.deltaTime);
                }
                else
                {
                    transform.rotation = targetRotation;
                }
            }
        }
        
        private void StartFadeIn()
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
                
            fadeCoroutine = StartCoroutine(FadeIn());
        }
        
        private IEnumerator FadeIn()
        {
            canvasGroup.alpha = 0f;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        private void StartTypewriterEffect()
        {
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
                
            typewriterCoroutine = StartCoroutine(TypewriterEffect());
        }
        
        private IEnumerator TypewriterEffect()
        {
            string fullText = originalText;
            mainText.text = "";
            
            if (shadowText != null)
                shadowText.text = "";
            
            float charactersPerSecond = typewriterSpeed;
            float timePerCharacter = 1f / charactersPerSecond;
            
            for (int i = 0; i <= fullText.Length; i++)
            {
                string displayText = fullText.Substring(0, i);
                mainText.text = displayText;
                
                if (shadowText != null)
                    shadowText.text = displayText;
                
                yield return new WaitForSeconds(timePerCharacter);
            }
        }
        
        // Public methods for external control
        public void SetText(string newText, bool animate = false)
        {
            originalText = newText;
            
            if (animate && enableTypewriterEffect)
            {
                StartTypewriterEffect();
            }
            else
            {
                mainText.text = newText;
                if (shadowText != null)
                    shadowText.text = newText;
            }
        }
        
        public void FadeOut(float duration = 0.5f)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
                
            fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
        }
        
        private IEnumerator FadeOutCoroutine(float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
        }
        
        public void SetVisibility(bool visible, bool animate = true)
        {
            if (animate)
            {
                if (visible)
                {
                    gameObject.SetActive(true);
                    StartFadeIn();
                }
                else
                {
                    FadeOut(fadeInDuration);
                }
            }
            else
            {
                gameObject.SetActive(visible);
                canvasGroup.alpha = visible ? 1f : 0f;
            }
        }
        
        public void SetColor(Color color)
        {
            if (mainText != null)
                mainText.color = color;
        }
        
        public void SetOutlineColor(Color color)
        {
            outlineColor = color;
            if (mainText.fontSharedMaterial != null)
            {
                mainText.fontSharedMaterial.SetColor(OutlineColor, color);
            }
        }
        
        public void SetFontSize(float size)
        {
            if (mainText != null)
            {
                mainText.fontSize = size;
                originalFontSize = size;
                
                if (shadowText != null)
                    shadowText.fontSize = size;
            }
        }
        
        private void OnDestroy()
        {
            // Clean up coroutines
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
        }
    }
}