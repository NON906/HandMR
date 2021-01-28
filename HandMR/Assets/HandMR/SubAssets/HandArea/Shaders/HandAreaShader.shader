Shader "Unlit/HandAreaShader"
{
    Properties
    {
        _ColorInner ("Color Inner", Color) = (0.5,0.5,0.5,0.5)
        _ThicknessInnerX ("Thickness Inner X", Float) = 0.15
        _ThicknessInnerY ("Thickness Inner Y", Float) = 0.15
        _Color ("Color Outer", Color) = (1,1,1,0.5)
        _ThicknessX ("Thickness Outer X", Float) = 0.1
        _ThicknessY ("Thickness Outer Y", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _ThicknessX, _ThicknessY;
            float4 _ColorInner;
            float _ThicknessInnerX, _ThicknessInnerY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if(i.uv.x<=_ThicknessX) { return _Color; }
                if(i.uv.x>=1 - _ThicknessX) { return _Color; }
                if(i.uv.y<=_ThicknessY) { return _Color; }
                if(i.uv.y>=1 - _ThicknessY) { return _Color; }

                if(i.uv.x<=_ThicknessInnerX) { return _ColorInner; }
                if(i.uv.x>=1 - _ThicknessInnerX) { return _ColorInner; }
                if(i.uv.y<=_ThicknessInnerY) { return _ColorInner; }
                if(i.uv.y>=1 - _ThicknessInnerY) { return _ColorInner; }

                return float4(0,0,0,0);
            }
            ENDCG
        }
    }
}
