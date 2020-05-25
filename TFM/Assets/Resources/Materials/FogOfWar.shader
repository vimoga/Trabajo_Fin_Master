// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// FogOfWar shader
// Copyright (C) 2013 Sergey Taraban <http://staraban.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

Shader "Custom/FogOfWar" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}	
	_FogRadius1 ("FogRadius1", Float) = 1.0
	_Radar1_Pos ("_Radar1_Pos", Vector) = (0,0,0,1)
	_FogRadius2 ("FogRadius2", Float) = 1.0
    _Radar2_Pos ("_Radar2_Pos", Vector) = (0,0,0,1)
	_FogRadius3 ("FogRadius3", Float) = 1.0
    _Radar3_Pos ("_Radar3_Pos", Vector) = (0,0,0,1)
	_FogRadius4 ("FogRadius4", Float) = 1.0
	_Radar4_Pos ("_Radar4_Pos", Vector) = (0,0,0,1)
	_FogRadius5 ("FogRadius5", Float) = 1.0
    _Radar5_Pos ("_Radar5_Pos", Vector) = (0,0,0,1)
	_FogRadius6 ("FogRadius6", Float) = 1.0
    _Radar6_Pos ("_Radar6_Pos", Vector) = (0,0,0,1)
	_FogRadius7 ("FogRadius7", Float) = 1.0
	_Radar7_Pos ("_Radar7_Pos", Vector) = (0,0,0,1)
	_FogRadius8 ("FogRadius8", Float) = 1.0
    _Radar8_Pos ("_Radar8_Pos", Vector) = (0,0,0,1)
	_FogRadius9 ("FogRadius9", Float) = 1.0
    _Radar9_Pos ("_Radar9_Pos", Vector) = (0,0,0,1)
	_FogRadius10 ("FogRadius10", Float) = 1.0
	_Radar10_Pos ("_Radar10_Pos", Vector) = (0,0,0,1)
	_FogRadius11 ("FogRadius11", Float) = 1.0
    _Radar11_Pos ("_Radar11_Pos", Vector) = (0,0,0,1)
	_FogRadius12 ("FogRadius12", Float) = 1.0
    _Radar12_Pos ("_Radar12_Pos", Vector) = (0,0,0,1)
	_FogRadius13 ("FogRadius13", Float) = 1.0
	_Radar13_Pos ("_Radar13_Pos", Vector) = (0,0,0,1)
	_FogRadius14 ("FogRadius14", Float) = 1.0
    _Radar14_Pos ("_Radar14_Pos", Vector) = (0,0,0,1)
	_FogRadius15("FogRadius15", Float) = 1.0
	_Radar15_Pos("_Radar15_Pos", Vector) = (0,0,0,1)
	_FogRadius16("FogRadius16", Float) = 1.0
	_Radar16_Pos("_Radar16_Pos", Vector) = (0,0,0,1)
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Off

    CGPROGRAM
    #pragma surface surf Lambert vertex:vert alpha:blend

    sampler2D _MainTex;
    fixed4     _Color;
	
	float      _FogRadius1;
	float4     _Radar1_Pos;
	float      _FogRadius2;
	float4     _Radar2_Pos;
	float      _FogRadius3;
	float4     _Radar3_Pos;
	float      _FogRadius4;
	float4     _Radar4_Pos;
	float      _FogRadius5;
	float4     _Radar5_Pos;
	float      _FogRadius6;
	float4     _Radar6_Pos;
	float      _FogRadius7;
	float4     _Radar7_Pos;
	float      _FogRadius8;
	float4     _Radar8_Pos;
	float      _FogRadius9;
	float4     _Radar9_Pos;
	float      _FogRadius10;
	float4     _Radar10_Pos;
	float      _FogRadius11;
	float4     _Radar11_Pos;
	float      _FogRadius12;
	float4     _Radar12_Pos;
	float      _FogRadius13;
	float4     _Radar13_Pos;
	float      _FogRadius14;
	float4     _Radar14_Pos;
	float      _FogRadius15;
	float4     _Radar15_Pos;
	float      _FogRadius16;
	float4     _Radar16_Pos;

    struct Input {
        float2 uv_MainTex;
        float2 location;
    };

    float powerForPos(float4 pos, float2 nearVertex, float fogRadius);

    void vert(inout appdata_full vertexData, out Input outData) {
        float4 pos = UnityObjectToClipPos(vertexData.vertex);
        float4 posWorld = mul(unity_ObjectToWorld, vertexData.vertex);
        outData.uv_MainTex = vertexData.texcoord;
        outData.location = posWorld.xz;
    }

    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		
		float radarsAplhas = powerForPos(_Radar1_Pos, IN.location, _FogRadius1);
		radarsAplhas += powerForPos(_Radar2_Pos, IN.location, _FogRadius2);		
		radarsAplhas += powerForPos(_Radar3_Pos, IN.location, _FogRadius3);
		radarsAplhas += powerForPos(_Radar4_Pos, IN.location, _FogRadius4);
		radarsAplhas += powerForPos(_Radar5_Pos, IN.location, _FogRadius5);
		radarsAplhas += powerForPos(_Radar6_Pos, IN.location, _FogRadius6);
		radarsAplhas += powerForPos(_Radar7_Pos, IN.location, _FogRadius7);
		radarsAplhas += powerForPos(_Radar8_Pos, IN.location, _FogRadius8);
		radarsAplhas += powerForPos(_Radar9_Pos, IN.location, _FogRadius9);
		radarsAplhas += powerForPos(_Radar10_Pos, IN.location, _FogRadius10);
		radarsAplhas += powerForPos(_Radar11_Pos, IN.location, _FogRadius11);
		radarsAplhas += powerForPos(_Radar12_Pos, IN.location, _FogRadius12);
		radarsAplhas += powerForPos(_Radar13_Pos, IN.location, _FogRadius13);
		radarsAplhas += powerForPos(_Radar14_Pos, IN.location, _FogRadius14);


		float alpha = (1 - (baseColor.a + radarsAplhas));
        o.Albedo = baseColor.rgb;
		if (alpha > 0.20) {
			o.Alpha = alpha;
		}        
    }

    float powerForPos(float4 pos, float2 nearVertex, float fogRadius) {

        float atten = clamp(fogRadius - length(pos.xz - nearVertex.xy), 0.0, fogRadius);
		if (atten > 0.0 && atten < fogRadius) {
			atten = 1;
		}
		else {
			atten = 0;
		}
		return atten;
    }

    ENDCG
}

Fallback "Transparent/VertexLit"
}