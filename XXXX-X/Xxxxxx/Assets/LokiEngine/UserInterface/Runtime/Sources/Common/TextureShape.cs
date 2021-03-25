/// Credit CiaccoDavide
/// Sourced from - http://ciaccodavi.de/unity/uipolygon

using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Loki.UI
{
	[AddComponentMenu("UI/Primitives/Texture Shape")]
	public class TextureShape : MaskableGraphic
	{
		private float mSize = 0;

		[SerializeField]
		private Texture mTexture;

		public bool fill = true;

		public float thickness = 5;

		[Range(3, 360)]
		public int sides = 3;

		[Range(0, 360)]
		public float angle = 0;

		[Range(0, 1)]
		public float[] verticesDistances = new float[3];

		public override Texture mainTexture
		{
			get
			{
				return mTexture == null ? s_WhiteTexture : mTexture;
			}
		}

		public Texture texture
		{
			get
			{
				return mTexture;
			}
			set
			{
				if (mTexture == value) return;
				mTexture = value;
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}

		public void DrawShape(int sides)
		{
			this.sides = sides;
			this.verticesDistances = new float[sides + 1];
			for (int i = 0; i < sides; i++)
				verticesDistances[i] = 1;
			this.angle = 0;
		}

		public void DrawShape(int sides, float[] verticesDistances)
		{
			this.sides = sides;
			this.verticesDistances = verticesDistances;
			this.angle = 0;
		}

		public void DrawShape(int sides, float[] verticesDistances, float angle)
		{
			this.sides = sides;
			this.verticesDistances = verticesDistances;
			this.angle = angle;
		}

		private void Update()
		{
			mSize = rectTransform.rect.width;
			if (rectTransform.rect.width > rectTransform.rect.height)
				mSize = rectTransform.rect.height;
			else
				mSize = rectTransform.rect.width;
			thickness = (float)Mathf.Clamp(thickness, 0, mSize / 2);
		}

		protected UIVertex[] SetVBO(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] vbo = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				var vert = UIVertex.simpleVert;
				vert.color = color;
				vert.position = vertices[i];
				vert.uv0 = uvs[i];
				vbo[i] = vert;
			}
			return vbo;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Vector2 prevX = Vector2.zero;
			Vector2 prevY = Vector2.zero;
			float degrees = 360f / sides;
			int vertices = sides + 1;
			if (verticesDistances.Length != vertices)
			{
				verticesDistances = new float[vertices];
				for (int i = 0; i < vertices - 1; i++) 
					verticesDistances[i] = 1;
			}
			// last vertex is also the first!
			verticesDistances[vertices - 1] = verticesDistances[0];
			for (int i = 0; i < vertices; i++)
			{
				float outer = -rectTransform.pivot.x * mSize * verticesDistances[i];
				float inner = -rectTransform.pivot.x * mSize * verticesDistances[i] + thickness;
				float rad = Mathf.Deg2Rad * (i * degrees + angle);
				float c = Mathf.Cos(rad);
				float s = Mathf.Sin(rad);
				var uv0 = new Vector2(0, 1);
				var uv1 = new Vector2(1, 1);
				var uv2 = new Vector2(1, 0);
				var uv3 = new Vector2(0, 0);
				var pos0 = prevX;
				var pos1 = new Vector2(outer * c, outer * s);
				Vector2 pos2;
				Vector2 pos3;
				if (fill)
				{
					pos2 = Vector2.zero;
					pos3 = Vector2.zero;
				}
				else
				{
					pos2 = new Vector2(inner * c, inner * s);
					pos3 = prevY;
				}
				prevX = pos1;
				prevY = pos2;
				vh.AddUIVertexQuad(SetVBO(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
			}
		}
	}
}
