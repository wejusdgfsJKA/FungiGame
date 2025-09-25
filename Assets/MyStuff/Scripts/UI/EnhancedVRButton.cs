using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections;

namespace FungiGame.UI
{
    /// <summary>
    /// Enhanced VR Button with improved visual feedback, animations, and better text rendering
    /// Provides a professional look for VR interaction buttons with smooth animations
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class EnhancedVRButton : MonoBehaviour
    {
        [Header("Visual Components")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        [SerializeField] private Color hoverColor = new Color(0.3f, 0.5f, 0.8f, 0.9f);
        [SerializeField] private Color pressedColor = new Color(0.1f, 0.3f, 0.6f, 1f);
        [SerializeField] private Color textNormalColor = Color.white;
        [SerializeField] private Color textHoverColor = Color.white;
        [SerializeField] private Color outlineColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float pressedScale = 0.95f;
        [SerializeField] private float animationDuration = 0.2f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Glow Effect")]
        [SerializeField] private bool enableGlow = true;
        [SerializeField] private Image glowImage;
        [SerializeField] private Color glowColor = new Color(0.3f, 0.5f, 1f, 0.3f);
        [SerializeField] private float glowIntensity = 1.5f;
        [SerializeField] private float glowPulseSpeed = 2f;
        
        [Header("Audio Feedback")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
        
        [Header("Haptic Feedback")]
        [SerializeField] private bool enableHapticFeedback = true;
        [SerializeField] [Range(0f, 1f)] private float hoverHapticIntensity = 0.1f;
        [SerializeField] [Range(0f, 1f)] private float clickHapticIntensity = 0.3f;
        
        // Private variables
        private Vector3 originalScale;
        private Coroutine currentAnimation;
        private Coroutine glowAnimation;
        private bool isHovering = false;
        private bool isPressed = false;
        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor currentInteractor;
        
        private void Awake()
        {
            // Auto-assign components if not set
            if (button == null)
                button = GetComponent<Button>();
            if (buttonText == null)
                buttonText = GetComponentInChildren<TextMeshProUGUI>();
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            // Store original scale
            originalScale = transform.localScale;
            
            // Setup initial colors
            SetupInitialColors();
            
            // Setup audio source if not assigned
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // 3D sound
                audioSource.volume = volume;
            }
        }
        
        private void Start()
        {
            // Setup XR interaction events
            SetupXRInteractionEvents();
            
            // Start glow animation if enabled
            if (enableGlow && glowImage != null)
            {
                StartGlowAnimation();
            }
        }
        
        private void SetupInitialColors()
        {
            if (backgroundImage != null)
                backgroundImage.color = normalColor;
                
            if (buttonText != null)
                buttonText.color = textNormalColor;
                
            if (outlineImage != null)
                outlineImage.color = outlineColor;
        }
        
        private void SetupXRInteractionEvents()
        {
            // Find XR Interactable component
            var xrInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (xrInteractable != null)
            {
                xrInteractable.hoverEntered.AddListener(OnHoverEntered);
                xrInteractable.hoverExited.AddListener(OnHoverExited);
                xrInteractable.selectEntered.AddListener(OnSelectEntered);
                xrInteractable.selectExited.AddListener(OnSelectExited);
            }
            
            // Fallback to traditional UI events
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
        }
        
        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor)
            {
                currentInteractor = interactor;
            }
            
            isHovering = true;
            AnimateToHoverState();
            PlayHoverEffects();
        }
        
        private void OnHoverExited(HoverExitEventArgs args)
        {
            currentInteractor = null;
            isHovering = false;
            
            if (!isPressed)
            {
                AnimateToNormalState();
            }
        }
        
        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            isPressed = true;
            AnimateToPressedState();
            PlayClickEffects();
        }
        
        private void OnSelectExited(SelectExitEventArgs args)
        {
            isPressed = false;
            
            if (isHovering)
            {
                AnimateToHoverState();
            }
            else
            {
                AnimateToNormalState();
            }
        }
        
        private void OnButtonClick()
        {
            if (!isPressed) // Prevent double-triggering from XR and UI events
            {
                PlayClickEffects();
            }
        }
        
        private void AnimateToNormalState()
        {
            StopCurrentAnimation();
            currentAnimation = StartCoroutine(AnimateButton(originalScale, normalColor, textNormalColor));
        }
        
        private void AnimateToHoverState()
        {
            StopCurrentAnimation();
            Vector3 targetScale = originalScale * hoverScale;
            currentAnimation = StartCoroutine(AnimateButton(targetScale, hoverColor, textHoverColor));
        }
        
        private void AnimateToPressedState()
        {
            StopCurrentAnimation();
            Vector3 targetScale = originalScale * pressedScale;
            currentAnimation = StartCoroutine(AnimateButton(targetScale, pressedColor, textHoverColor));
        }
        
        private void StopCurrentAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
        }
        
        private IEnumerator AnimateButton(Vector3 targetScale, Color targetBackgroundColor, Color targetTextColor)
        {
            Vector3 startScale = transform.localScale;
            Color startBackgroundColor = backgroundImage != null ? backgroundImage.color : Color.white;
            Color startTextColor = buttonText != null ? buttonText.color : Color.white;
            
            float elapsedTime = 0f;
            
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / animationDuration;
                float curveValue = animationCurve.Evaluate(t);
                
                // Animate scale
                transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                
                // Animate background color
                if (backgroundImage != null)
                {
                    backgroundImage.color = Color.Lerp(startBackgroundColor, targetBackgroundColor, curveValue);
                }
                
                // Animate text color
                if (buttonText != null)
                {
                    buttonText.color = Color.Lerp(startTextColor, targetTextColor, curveValue);
                }
                
                yield return null;
            }
            
            // Ensure final values
            transform.localScale = targetScale;
            if (backgroundImage != null)
                backgroundImage.color = targetBackgroundColor;
            if (buttonText != null)
                buttonText.color = targetTextColor;
        }
        
        private void StartGlowAnimation()
        {
            if (glowAnimation != null)
                StopCoroutine(glowAnimation);
                
            glowAnimation = StartCoroutine(AnimateGlow());
        }
        
        private IEnumerator AnimateGlow()
        {
            if (glowImage == null)
                yield break;
                
            Color baseGlowColor = glowColor;
            
            while (true)
            {
                float pulse = (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) * 0.5f;
                Color currentGlowColor = baseGlowColor;
                currentGlowColor.a = baseGlowColor.a * pulse * glowIntensity;
                
                glowImage.color = currentGlowColor;
                
                yield return null;
            }
        }
        
        private void PlayHoverEffects()
        {
            // Play hover sound
            if (audioSource != null && hoverSound != null)
            {
                audioSource.clip = hoverSound;
                audioSource.Play();
            }
            
            // Play haptic feedback
            if (enableHapticFeedback && currentInteractor != null)
            {
                // Note: Haptic feedback would need XR-specific implementation
                // This is a placeholder for haptic feedback
                SendHapticFeedback(hoverHapticIntensity);
            }
        }
        
        private void PlayClickEffects()
        {
            // Play click sound
            if (audioSource != null && clickSound != null)
            {
                audioSource.clip = clickSound;
                audioSource.Play();
            }
            
            // Play haptic feedback
            if (enableHapticFeedback && currentInteractor != null)
            {
                SendHapticFeedback(clickHapticIntensity);
            }
        }
        
        private void SendHapticFeedback(float intensity)
        {
            // Placeholder for XR haptic feedback implementation
            // You would need to implement this based on your XR system
            if (currentInteractor != null)
            {
                // Example: currentInteractor.SendHapticImpulse(intensity, 0.1f);
                Debug.Log($"Haptic feedback: {intensity}");
            }
        }
        
        // Public methods for external control
        public void SetButtonText(string text)
        {
            if (buttonText != null)
                buttonText.text = text;
        }
        
        public void SetInteractable(bool interactable)
        {
            if (button != null)
                button.interactable = interactable;
                
            if (canvasGroup != null)
                canvasGroup.alpha = interactable ? 1f : 0.5f;
        }
        
        public void SetColors(Color normal, Color hover, Color pressed)
        {
            normalColor = normal;
            hoverColor = hover;
            pressedColor = pressed;
            
            if (!isHovering && !isPressed)
            {
                AnimateToNormalState();
            }
        }
        
        private void OnDestroy()
        {
            // Clean up XR interaction events
            var xrInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (xrInteractable != null)
            {
                xrInteractable.hoverEntered.RemoveListener(OnHoverEntered);
                xrInteractable.hoverExited.RemoveListener(OnHoverExited);
                xrInteractable.selectEntered.RemoveListener(OnSelectEntered);
                xrInteractable.selectExited.RemoveListener(OnSelectExited);
            }
        }
    }
}