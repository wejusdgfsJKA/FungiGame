using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum HDRType
{
    ColorfulStudio,
    Microscopic
}

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; protected set; }
    [SerializeField] protected GameObject[] rooms;
    [SerializeField] protected Volume globalVolume;

    [Header("Skybox Materials - Pre-created for URP")]
    [SerializeField] protected Material microscopicSkyboxMaterial; // For Room 2
    [SerializeField] protected Material colorfulStudioSkyboxMaterial; // For Room 1 and 3

    [Header("Fallback Cubemaps (Legacy)")]
    [SerializeField] protected Cubemap microscopicHDR; // For Room 2
    [SerializeField] protected Cubemap colorfulStudioHDR; // For Room 1 and 3

    [Header("Debug Controls")]
    [SerializeField] protected bool enableDebugControls = true;
    [SerializeField] protected HDRType debugHDRSelection = HDRType.ColorfulStudio;
    [SerializeField] protected bool applyDebugHDR = false;

    private HDRType lastDebugSelection;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // Validate skybox materials for URP
        ValidateSkyboxMaterials();
    }

    private void ValidateSkyboxMaterials()
    {
        if (microscopicSkyboxMaterial == null)
        {
            Debug.LogWarning("Microscopic skybox material not assigned. Please create a material with URP/Skybox shader and assign the cubemap.");
        }

        if (colorfulStudioSkyboxMaterial == null)
        {
            Debug.LogWarning("Colorful Studio skybox material not assigned. Please create a material with URP/Skybox shader and assign the cubemap.");
        }

        // Check if materials use the correct shader
        if (microscopicSkyboxMaterial != null && !IsValidSkyboxMaterial(microscopicSkyboxMaterial))
        {
            Debug.LogWarning("Microscopic skybox material doesn't use a valid skybox shader.");
        }

        if (colorfulStudioSkyboxMaterial != null && !IsValidSkyboxMaterial(colorfulStudioSkyboxMaterial))
        {
            Debug.LogWarning("Colorful Studio skybox material doesn't use a valid skybox shader.");
        }
    }

    private bool IsValidSkyboxMaterial(Material material)
    {
        if (material == null || material.shader == null) return false;

        string shaderName = material.shader.name;
        return shaderName.Contains("Skybox") || shaderName.Contains("skybox");
    }

    private void Update()
    {
        // Debug controls - only works in editor
#if UNITY_EDITOR
        if (enableDebugControls)
        {
            // Check if the debug HDR selection changed
            if (debugHDRSelection != lastDebugSelection)
            {
                lastDebugSelection = debugHDRSelection;
                if (applyDebugHDR)
                {
                    ApplyDebugHDR();
                }
            }

            // Apply debug HDR when button is pressed
            if (applyDebugHDR)
            {
                ApplyDebugHDR();
                applyDebugHDR = false; // Reset the button
            }
        }
#endif
    }

#if UNITY_EDITOR
    private void ApplyDebugHDR()
    {
        switch (debugHDRSelection)
        {
            case HDRType.ColorfulStudio:
                SetSkyboxMaterial(colorfulStudioSkyboxMaterial, "Colorful Studio");
                Debug.Log("Debug: Applied Colorful Studio HDR");
                break;
            case HDRType.Microscopic:
                SetSkyboxMaterial(microscopicSkyboxMaterial, "Microscopic");
                Debug.Log("Debug: Applied Microscopic HDR");
                break;
        }
    }
#endif

    public void LoadRoom1()
    {
        rooms[1].SetActive(false);
        rooms[2].SetActive(false);
        rooms[0].SetActive(true);

        // Load colorful studio skybox for Room 1
        SetSkyboxMaterial(colorfulStudioSkyboxMaterial, "Colorful Studio");
    }

    public void LoadRoom2()
    {
        rooms[0].SetActive(false);
        rooms[2].SetActive(false);
        rooms[1].SetActive(true);

        // Load microscopic skybox for Room 2
        SetSkyboxMaterial(microscopicSkyboxMaterial, "Microscopic");
    }

    public void LoadRoom3()
    {
        rooms[0].SetActive(false);
        rooms[1].SetActive(false);
        rooms[2].SetActive(true);

        // Load colorful studio skybox for Room 3
        SetSkyboxMaterial(colorfulStudioSkyboxMaterial, "Colorful Studio");
    }

    private void SetSkyboxMaterial(Material skyboxMaterial, string skyboxName)
    {
        if (skyboxMaterial != null)
        {
            // Direct assignment is much more reliable on mobile devices
            RenderSettings.skybox = skyboxMaterial;

            // Force update the environment lighting
            DynamicGI.UpdateEnvironment();

            Debug.Log($"Applied {skyboxName} skybox material");
        }
        else
        {
            Debug.LogError($"{skyboxName} skybox material is null! Please assign a pre-created material in the inspector.");
        }
    }

#if UNITY_EDITOR
    // Debug methods that can be called from inspector buttons or other scripts
    [ContextMenu("Debug: Apply Colorful Studio HDR")]
    public void DebugApplyColorfulStudioHDR()
    {
        SetSkyboxMaterial(colorfulStudioSkyboxMaterial, "Colorful Studio");
        Debug.Log("Debug: Applied Colorful Studio HDR via context menu");
    }

    [ContextMenu("Debug: Apply Microscopic HDR")]
    public void DebugApplyMicroscopicHDR()
    {
        SetSkyboxMaterial(microscopicSkyboxMaterial, "Microscopic");
        Debug.Log("Debug: Applied Microscopic HDR via context menu");
    }

    [ContextMenu("Debug: Test Room 1 Loading")]
    public void DebugLoadRoom1()
    {
        LoadRoom1();
        Debug.Log("Debug: Loaded Room 1 with Colorful Studio HDR");
    }

    [ContextMenu("Debug: Test Room 2 Loading")]
    public void DebugLoadRoom2()
    {
        LoadRoom2();
        Debug.Log("Debug: Loaded Room 2 with Microscopic HDR");
    }

    [ContextMenu("Debug: Test Room 3 Loading")]
    public void DebugLoadRoom3()
    {
        LoadRoom3();
        Debug.Log("Debug: Loaded Room 3 with Colorful Studio HDR");
    }
#endif
}
