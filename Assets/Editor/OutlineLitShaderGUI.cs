using UnityEngine;
using UnityEditor;

public class OutlineLitShaderGUI : ShaderGUI
{
    // Material properties
    MaterialProperty outlineColor = null;
    MaterialProperty outlineThickness = null;

    // Standard lit properties
    MaterialProperty baseMap = null;
    MaterialProperty baseColor = null;
    MaterialProperty metallic = null;
    MaterialProperty smoothness = null;
    MaterialProperty normalMap = null;
    MaterialProperty emission = null;

    bool showOutlineOptions = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // Find all properties
        FindProperties(properties);

        Material material = materialEditor.target as Material;

        EditorGUI.BeginChangeCheck();

        // Draw standard properties
        DrawStandardProperties(materialEditor);

        // Add space before outline section
        EditorGUILayout.Space();

        // Draw outline options
        DrawOutlineSection(materialEditor);

        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in materialEditor.targets)
                MaterialChanged((Material)obj);
        }
    }

    void FindProperties(MaterialProperty[] props)
    {
        // Outline properties
        outlineColor = FindProperty("_OutlineColor", props, false);
        outlineThickness = FindProperty("_OutlineThickness", props, false);

        // Standard lit properties
        baseMap = FindProperty("_BaseMap", props, false);
        baseColor = FindProperty("_BaseColor", props, false);
        metallic = FindProperty("_Metallic", props, false);
        smoothness = FindProperty("_Smoothness", props, false);
        normalMap = FindProperty("_BumpMap", props, false);
        emission = FindProperty("_EmissionColor", props, false);
    }

    void DrawStandardProperties(MaterialEditor materialEditor)
    {
        // Surface properties
        GUILayout.Label("Surface", EditorStyles.boldLabel);

        if (baseMap != null && baseColor != null)
        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), baseMap, baseColor);
        }

        if (metallic != null)
        {
            materialEditor.RangeProperty(metallic, "Metallic");
        }

        if (smoothness != null)
        {
            materialEditor.RangeProperty(smoothness, "Smoothness");
        }

        if (normalMap != null)
        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), normalMap);
        }

        if (emission != null)
        {
            materialEditor.ColorProperty(emission, "Emission");
        }
    }

    void DrawOutlineSection(MaterialEditor materialEditor)
    {
        // Create foldout for outline options
        showOutlineOptions = EditorGUILayout.Foldout(showOutlineOptions, "Outline Options", true);

        if (showOutlineOptions)
        {
            EditorGUI.indentLevel++;

            if (outlineColor != null)
            {
                materialEditor.ColorProperty(outlineColor, "Outline Color");
            }

            if (outlineThickness != null)
            {
                materialEditor.RangeProperty(outlineThickness, "Outline Thickness");
            }

            EditorGUI.indentLevel--;
        }
    }

    static void MaterialChanged(Material material)
    {
        // You can add material validation logic here if needed
    }
}