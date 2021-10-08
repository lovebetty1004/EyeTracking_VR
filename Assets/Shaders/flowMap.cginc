#ifndef flowMap_INCLUDED
#define flowMap_INCLUDED

void flowMap_float(Texture2D _MainTex,
                    float4 _FlowDir, float2 texcoord, SamplerState SS,
                    float flowTime, float _FlowSpeed,
                    out float3 outColor){
    
    float4 flowDir = _FlowDir * 2.0f - 1.0f;
    flowDir *= _FlowSpeed;
    float phase0 = frac(flowTime * 0.5f + 0.5f);
    float phase1 = frac(flowTime * 0.5f + 1.0f);

    // half3 tex0 = tex2D(_MainTex, texcoord + flowDir.xy * phase0);
    // half3 tex1 = tex2D(_MainTex, texcoord + flowDir.xy * phase1);

    half3 tex0 = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord + flowDir.xy * phase0).rgb;
    half3 tex1 = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord + flowDir.xy * phase1).rgb;
    

    float flowLerp = abs((0.5f - phase0) / 0.5f);
    half3 finalColor = lerp(tex0, tex1, flowLerp);

    outColor = finalColor;
}

void flowMapAlpha_float(Texture2D _MainTex,
                    float2 texcoord, SamplerState SS,
                    float flowTime, float _FlowSpeed,
                    float depth, out float4 outColor){
    
    float4 oriCol = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord);
    
    // float4 col = float4(oriCol.r * 0.299, oriCol.g * 0.587, oriCol.b * 0.114, oriCol.a);
    // col = oriCol;

    float4 col = oriCol;

    float4 flowDir = col * 2.0f - 1.0f;
    flowDir *= _FlowSpeed * depth;
    float phase0 = frac(flowTime * 0.5f + 0.5f);
    float phase1 = frac(flowTime * 0.5f + 1.0f);

    // half3 tex0 = tex2D(_MainTex, texcoord + flowDir.xy * phase0);
    // half3 tex1 = tex2D(_MainTex, texcoord + flowDir.xy * phase1);

    half4 tex0 = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord + flowDir.xy * phase0);
    half4 tex1 = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord + flowDir.xy * phase1);

    float flowLerp = abs((0.5f - phase0) / 0.5f);
    half4 finalColor = lerp(tex0, tex1, flowLerp);

    outColor = finalColor;
}

void flowMapAlphaDir_float(Texture2D _MainTex,
                    float2 texcoord, SamplerState SS,
                    float2 dirRot,
                    float flowTime, float _FlowSpeed,
                    float depth, out float4 outColor){
    
    float4 oriCol = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord);
    
    // float4 col = float4(oriCol.r * 0.299, oriCol.g * 0.587, oriCol.b * 0.114, oriCol.a);
    // col = oriCol;

    float4 col = oriCol;

    float4 flowDir = normalize((col * 2.0f - 1.0f) * float4(dirRot.xy, 1, 1));
    flowDir *= _FlowSpeed * depth;
    float phase0 = frac(flowTime * 0.5f + 0.5f);
    float phase1 = frac(flowTime * 0.5f + 1.0f);

    // half3 tex0 = tex2D(_MainTex, texcoord + flowDir.xy * phase0);
    // half3 tex1 = tex2D(_MainTex, texcoord + flowDir.xy * phase1);

    half4 tex0 = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord + flowDir.xy * phase0);
    half4 tex1 = SAMPLE_TEXTURE2D(_MainTex, SS, texcoord + flowDir.xy * phase1);

    float flowLerp = abs((0.5f - phase0) / 0.5f);
    half4 finalColor = lerp(tex0, tex1, flowLerp);

    outColor = finalColor;
}

void flowMapAlphaTexArray_float(Texture2DArray _MainTex,
                    int _TexId, float2 texcoord, SamplerState SS,
                    float flowTime, float _FlowSpeed,
                    float depth, out float4 outColor){

    float4 oriCol = SAMPLE_TEXTURE2D_ARRAY(_MainTex, SS, texcoord, _TexId);
    
    // float4 col = float4(oriCol.r * 0.299, oriCol.g * 0.587, oriCol.b * 0.114, oriCol.a);
    // col = oriCol;

    float4 col = oriCol;

    float4 flowDir = col * 2.0f - 1.0f;
    flowDir *= _FlowSpeed * depth;
    float phase0 = frac(flowTime * 0.5f + 0.5f);
    float phase1 = frac(flowTime * 0.5f + 1.0f);

    // half3 tex0 = tex2D(_MainTex, texcoord + flowDir.xy * phase0);
    // half3 tex1 = tex2D(_MainTex, texcoord + flowDir.xy * phase1);

    half4 tex0 = SAMPLE_TEXTURE2D_ARRAY(_MainTex, SS, texcoord + flowDir.xy * phase0, _TexId);
    half4 tex1 = SAMPLE_TEXTURE2D_ARRAY(_MainTex, SS, texcoord + flowDir.xy * phase1, _TexId);

    float flowLerp = abs((0.5f - phase0) / 0.5f);
    half4 finalColor = lerp(tex0, tex1, flowLerp);

    outColor = finalColor;
}

#endif