// Sprites-Default (built-in pipeline) with a hue-based outfit recolor for the player.
// Shirt: near-white, low-saturation pixels. Jeans: blue-hued pixels. Shading is preserved
// through the source brightness, so every animation frame recolors consistently.
Shader "StudentSurvival/OutfitSwap"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ShirtColor ("Shirt color", Color) = (1,1,1,1)
        _JeansColor ("Jeans color", Color) = (0.25,0.4,0.7,1)
        _SwapShirt ("Swap shirt", Float) = 0
        _SwapJeans ("Swap jeans", Float) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment OutfitFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            fixed4 _ShirtColor;
            fixed4 _JeansColor;
            float _SwapShirt;
            float _SwapJeans;

            float3 RgbToHsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            fixed4 OutfitFrag(v2f IN) : SV_Target
            {
                fixed4 tex = SampleSpriteTexture(IN.texcoord);
                float3 hsv = RgbToHsv(tex.rgb);
                if (_SwapShirt > 0.5 && hsv.y < 0.14 && hsv.z > 0.55)
                    tex.rgb = _ShirtColor.rgb * saturate(hsv.z / 0.92);
                else if (_SwapJeans > 0.5 && hsv.x > 0.52 && hsv.x < 0.72 && hsv.y > 0.2 && hsv.z > 0.15)
                    tex.rgb = _JeansColor.rgb * saturate(hsv.z / 0.7);
                fixed4 c = tex * IN.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}
