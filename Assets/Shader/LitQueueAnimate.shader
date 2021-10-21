Shader "Lit/UVQueueAnimation"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//动画播放速度
		_Speed("Speed",float) = 1
		// 序列图横向几个
		_HorAmount("Horizontal Amount",float) = 1
		// 序列图纵向几个
		_VerAmount("Vertical Amount",float) = 1
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Speed;
			float _HorAmount;
			float _VerAmount;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// 向下求整，保证每次一帧一帧变化（而不是滚动动画形式）
				float time = floor(_Time.y * _Speed);
				float row = floor(time / _HorAmount);
				float column = time - row * _HorAmount;

				// 因为 UV 的原点在左下角，而序列图是从上往下，从左到右（相当与原点在左上角），所以 y 要颠倒调整
				// _VerAmount -1 是因为编程中 0 开始，例如竖直方向 3 个，即为 0，1，2，所以 -1
				half2 uv = i.uv + half2(column ,  (_VerAmount - 1) - row);

				// 实现序列图集中只显示其中一张
				uv.x /= _HorAmount;
				uv.y /= _VerAmount;

				// sample the texture
				fixed4 col = tex2D(_MainTex, uv);

				return col;
			}
			ENDCG
		}
	}
}