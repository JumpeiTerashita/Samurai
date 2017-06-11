Shader "Custom/MonoTone" {
	Properties{
		_MainTex("MainTex", 2D) = "" {}
	_StopTime("StopTime", Float) = 0
	}

		SubShader{
				Pass{
		CGPROGRAM

	#include "UnityCG.cginc"

	#pragma vertex vert_img
	#pragma fragment frag

		sampler2D _MainTex;
		float _StopTime;
		fixed4 frag(v2f_img i) :COLOR
		{
		fixed4 c = tex2D(_MainTex,i.uv);
		float gray = c.r*0.3  + c.g*0.6 + c.b*0.1;
		c.rgb = fixed3(c.r*(1.1f-_StopTime) +gray*_StopTime, c.g * (1.1f - _StopTime) +gray*_StopTime,c.b* (1.1f - _StopTime) +gray*_StopTime);

		return c;
		}

			ENDCG
			}
	}
}
