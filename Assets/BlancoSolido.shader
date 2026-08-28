Shader "Custom/BlancoSolidoIntermitente"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0.0, 1.0)) = 1.0 // 0 = Sprite Normal, 1 = Blanco Sólido
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        Lighting Off 
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t 
            { 
                float4 vertex : POSITION; 
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f 
            { 
                float4 vertex : SV_POSITION; 
                float2 texcoord : TEXCOORD0; 
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _FlashAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Obtener el color original del sprite
                fixed4 texColor = tex2D(_MainTex, i.texcoord) * i.color;
                
                // 2. Definir el color blanco puro manteniendo la transparencia original
                fixed4 whiteColor = fixed4(1.0, 1.0, 1.0, texColor.a);
                
                // 3. Mezclar el color original con el blanco según el deslizador _FlashAmount
                fixed4 finalColor = lerp(texColor, whiteColor, _FlashAmount);
                
                return finalColor;
            }
            ENDCG
        }
    }
}
