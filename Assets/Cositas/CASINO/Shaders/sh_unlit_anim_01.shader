Shader "Casino/URP_unlit_anim_01"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _numU ("numU", Float) = 4
        _numV ("numV", Float) = 8
        _Speed ("Speed", Float) = 0.3
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float _numU;
            float _numV;
            float _Speed;

            // Structs
            struct VertexInput
            {
                float4 position : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Vertex Shader
            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(v.position); // URP Function
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            // Helper functions
            float GetColOffset(float numCol, float time, float numRow)
            {
                float oneCol = 1.0 / numCol;
                float allSteps = 1.0 / (numCol * numRow);
                int currentCol = floor(time / allSteps);

                if (currentCol >= numCol)
                {
                    currentCol -= floor(currentCol / numCol) * numCol;
                }

                return oneCol * currentCol;
            }

            float GetRowOffset(float numRow, float time)
            {
                float oneRow = 1.0 / numRow;
                int currentRow = floor(time / oneRow);
                return (1.0 - (oneRow * currentRow)) - numRow;
            }

            // Fragment Shader
            half4 frag(VertexOutput i) : SV_Target
            {
                float time = _Time.y * _Speed;
                float colOffset = GetColOffset(_numU, frac(time), _numV);
                float rowOffset = GetRowOffset(_numV, frac(time));
                float2 uvOffset = float2(colOffset, rowOffset);

                float2 tiledUV = (float2(1.0 / _numU, 1.0 / _numV) * i.uv) + uvOffset;
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, tiledUV);
                return half4(texColor.rgb, 1.0); // Output color
            }
            ENDHLSL
        }
    }
    FallBack "Unlit"
}
