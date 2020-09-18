Shader "Unlit/FisheyeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rate ("Distortion Rate", Range(0.55,0.7)) = 0.58
        _Center ("Distortion Center", Range(0.3,0.7)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
//      Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ IS_SQUARE
            #pragma multi_compile _ USE_MASK

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float rate : RATE;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Rate;
            float _Center;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #ifndef IS_SQUARE
                    o.rate = 1;
                #else
                    o.rate = _ScreenParams.x/_ScreenParams.y;
                #endif
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 coord = float2(i.uv.x-_Center, i.uv.y-0.5);
                float dist = (distance(coord, 0));
                #ifdef USE_MASK
                #ifndef IS_SQUARE
                if(dist>0.5){ return fixed4(0,0,0,0);}
                #else
                float2 ckCoord = (0.5 - i.uv)*float2(i.rate,1);
                float ckDist = (distance(ckCoord, 0));
                if(ckDist>0.5){ return fixed4(0,0,0,0);}
                #endif
                #endif

                float ang = atan2(coord.y,coord.x);
                float r = tan(dist * UNITY_PI/2) / tan(_Rate * UNITY_PI);
                coord.x = cos(ang)*r*2;
                coord.y = sin(ang)*r*2;
                coord = max(min(0.5-coord,1),0);
                if(coord.x<=0) { return fixed4(0,0,0,0); }
                if(coord.x>=1) { return fixed4(0,0,0,0); }
                if(coord.y<=0) { return fixed4(0,0,0,0); }
                if(coord.y>=1) { return fixed4(0,0,0,0); }
                fixed4 col = tex2D(_MainTex, coord);
                return col;
            }
            ENDCG
        }
    }
}