Shader "Test/SimpleShader"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
	}

		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _Color;
			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR0;
			};

            v2f vert (float4 v: POSITION)
            {
				v2f o;
				float4 originPos = UnityObjectToClipPos(v);
				o.pos = UnityObjectToClipPos(v);
				//fixed4 offset = 1 - o.pos - originPos;
				o.color = _Color;//fixed4(offset.x, offset.y, offset.z, 1.0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}