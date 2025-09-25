Shader "FungiGame/VR Text Enhanced"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        [MainColor] _Color ("Color", Color) = (1,1,1,1)
        
        [Header(Outline)]
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 1)) = 0.2
        
        [Header(Glow)]
        _GlowColor ("Glow Color", Color) = (1,1,1,0.5)
        _GlowPower ("Glow Power", Range(0, 5)) = 1
        
        [Header(Shadow)]
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffset ("Shadow Offset", Vector) = (2, -2, 0, 0)
        
        [Header(VR Optimization)]
        _Brightness ("Brightness", Range(0.5, 2)) = 1
        _Contrast ("Contrast", Range(0.5, 2)) = 1
        
        // UI Material properties
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        // Stencil for UI clipping
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Pass
        {
            Name "VR_TEXT_ENHANCED"
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _OutlineColor;
            fixed _OutlineWidth;
            fixed4 _GlowColor;
            fixed _GlowPower;
            fixed4 _ShadowColor;
            float4 _ShadowOffset;
            fixed _Brightness;
            fixed _Contrast;
            
            float4 _ClipRect;
            float4 _MainTex_ST;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // Sample the main texture
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Apply VR optimizations
                color.rgb = ((color.rgb - 0.5) * _Contrast + 0.5) * _Brightness;
                
                // Sample for outline effect
                float2 texelSize = 1.0 / _ScreenParams.xy;
                half4 outline = 0;
                
                // Simple 4-sample outline
                if (_OutlineWidth > 0)
                {
                    outline += tex2D(_MainTex, IN.texcoord + float2(-_OutlineWidth, 0) * texelSize);
                    outline += tex2D(_MainTex, IN.texcoord + float2(_OutlineWidth, 0) * texelSize);
                    outline += tex2D(_MainTex, IN.texcoord + float2(0, -_OutlineWidth) * texelSize);
                    outline += tex2D(_MainTex, IN.texcoord + float2(0, _OutlineWidth) * texelSize);
                    outline /= 4;
                    
                    // Apply outline color
                    outline *= _OutlineColor;
                    
                    // Combine with main color
                    color.rgb = lerp(outline.rgb, color.rgb, color.a);
                    color.a = max(color.a, outline.a * _OutlineWidth);
                }
                
                // Apply glow effect
                if (_GlowPower > 0)
                {
                    half glow = color.a * _GlowPower;
                    color.rgb += _GlowColor.rgb * glow * _GlowColor.a;
                }
                
                // UI Clipping
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
                
                // Alpha clipping for UI
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif
                
                return color;
            }
            ENDCG
        }
    }
    
    FallBack "UI/Default"
}