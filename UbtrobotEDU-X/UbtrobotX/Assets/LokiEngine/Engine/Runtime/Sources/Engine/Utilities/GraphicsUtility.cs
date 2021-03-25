using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Loki
{
	public static class GraphicsUtility
	{
		private static Mesh msQuadMesh;
		private static Mesh msCubeMesh;
		private static Vector3[] msSolidArcPoints = new Vector3[60];

		public static Mesh quadMesh
		{
			get
			{
				if (msQuadMesh != null)
					return msQuadMesh;

				msQuadMesh = new Mesh();
				msQuadMesh.vertices = new Vector3[] {
					new Vector3(-1,-1,0.5f),
					new Vector3(-1,1,0.5f),
					new Vector3(1,1,0.5f),
					new Vector3(1,-1,0.5f)
				};
				msQuadMesh.uv = new Vector2[] {
					new Vector2(0,1),
					new Vector2(0,0),
					new Vector2(1,0),
					new Vector2(1,1)
				};

				msQuadMesh.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
				return msQuadMesh;
			}
		}

		public static Mesh cubeMesh
		{
			get
			{
				if (msCubeMesh == null)
				{
					msCubeMesh = new Mesh();
					msCubeMesh.vertices = new Vector3[] {
						new Vector3(0, 0, 0),
						new Vector3(1, 0, 0),
						new Vector3(1, 1, 0),
						new Vector3(0, 1, 0),
						new Vector3(0, 1, 1),
						new Vector3(1, 1, 1),
						new Vector3(1, 0, 1),
						new Vector3(0, 0, 1),
					};
					msCubeMesh.triangles = new int[]{
						0, 2, 1, //face front
						0, 3, 2,
						2, 3, 4, //face top
						2, 4, 5,
						1, 2, 5, //face right
						1, 5, 6,
						0, 7, 4, //face left
						0, 4, 3,
						5, 4, 7, //face back
						5, 7, 6,
						0, 6, 7, //face bottom
						0, 1, 6
					};
					msCubeMesh.Optimize();
					msCubeMesh.RecalculateBounds();
					msCubeMesh.RecalculateNormals();
				}
				return msCubeMesh;
			}
		}

		public static Shader GetWireframeShader(EWireframeType type = EWireframeType.GSSolid)
		{
			Shader target = null;
			var shaders = EngineSettings.GetOrLoad().shaderSetings;
			switch (type)
			{
				case EWireframeType.GSSolid:
					{
						target = shaders.wireframeSolid;
						break;
					}
				case EWireframeType.GSTransparent:
					{
						target = shaders.wireframeTransparent;
						break;
					}
				case EWireframeType.GSSolidTransparentCulled:
					{
						target = shaders.wireframeTransparentCulled;
						break;
					}
			}
			if (target != null && target.isSupported)
			{
				return target;
			}
			return target;
		}

		public static Material GetWireframeMaterial(EWireframeType type = EWireframeType.GSSolid)
		{
			Material target = null;
			var shaders = EngineSettings.GetOrLoad().shaderSetings;
			switch (type)
			{
				case EWireframeType.GSSolid:
					{
						target = shaders.wireframeSolidMat;
						break;
					}
				case EWireframeType.GSTransparent:
					{
						target = shaders.wireframeTransparentMat;
						break;
					}
				case EWireframeType.GSSolidTransparentCulled:
					{
						target = shaders.wireframeTransparentCulledMat;
						break;
					}
			}
			if (target != null && target.shader != null && target.shader.isSupported)
			{
				return target;
			}
			return target;
		}

		public static void BlitMRT(this CommandBuffer buffer, RenderTargetIdentifier[] colorIdentifier, RenderTargetIdentifier depthIdentifier, Material mat, int pass)
		{
			buffer.SetRenderTarget(colorIdentifier, depthIdentifier);
			buffer.DrawMesh(quadMesh, Matrix4x4.identity, mat, 0, pass);
		}

		public static void BlitSRT(this CommandBuffer buffer, RenderTargetIdentifier destination, Material mat, int pass)
		{
			buffer.SetRenderTarget(destination);
			buffer.DrawMesh(quadMesh, Matrix4x4.identity, mat, 0, pass);
		}

		public static void BlitMRT(this CommandBuffer buffer, Texture source, RenderTargetIdentifier[] colorIdentifier, RenderTargetIdentifier depthIdentifier, Material mat, int pass)
		{
			buffer.SetRenderTarget(colorIdentifier, depthIdentifier);
			buffer.DrawMesh(quadMesh, Matrix4x4.identity, mat, 0, pass);
		}

		public static void BlitSRT(this CommandBuffer buffer, Texture source, RenderTargetIdentifier destination, Material mat, int pass)
		{
			buffer.SetGlobalTexture(ShaderIDs.MainTex, source);
			buffer.SetRenderTarget(destination);
			buffer.DrawMesh(quadMesh, Matrix4x4.identity, mat, 0, pass);
		}

		public static void BlitSRT(this CommandBuffer buffer, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material mat, int pass)
		{
			buffer.SetGlobalTexture(ShaderIDs.MainTex, source);
			buffer.SetRenderTarget(destination);
			buffer.DrawMesh(quadMesh, Matrix4x4.identity, mat, 0, pass);
		}

		public static void BlitStencil(this CommandBuffer buffer, RenderTargetIdentifier colorSrc, RenderTargetIdentifier colorBuffer, RenderTargetIdentifier depthStencilBuffer, Material mat, int pass)
		{
			buffer.SetGlobalTexture(ShaderIDs.MainTex, colorSrc);
			buffer.SetRenderTarget(colorBuffer, depthStencilBuffer);
			buffer.DrawMesh(quadMesh, Matrix4x4.identity, mat, 0, pass);
		}

		public static void BlitStencil(this CommandBuffer buffer, RenderTargetIdentifier colorBuffer, RenderTargetIdentifier depthStencilBuffer, Material mat, int pass)
		{
			buffer.SetRenderTarget(colorBuffer, depthStencilBuffer);
			buffer.DrawMesh(quadMesh, Matrix4x4.identity, mat, 0, pass);
		}

		public static void SetDiscSectionPoints(Vector3[] points, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
		{
			if (points == null)
				return;

			from = Vector3.Normalize(from) * radius;

			int count = points.Length;
			if (count <= 0)
				return;

			points[0] = center + from;

			if (count == 1)
				return;

			float offsetAngle = angle / (count - 1);
			for (int i = 1; i < count; ++i)
			{
				points[i] = center + Quaternion.AngleAxis(i * offsetAngle, normal) * from;
			}
		}

		public static void DrawSolidArc(Matrix4x4 localToWorld, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
		{
			Vector3[] points = msSolidArcPoints;
			var count = points.Length;
			if (count <= 0)
				return;

			SetDiscSectionPoints(points, center, normal, from, angle, radius);
			GL.PushMatrix();
			GL.MultMatrix(localToWorld);
			GL.Begin(GL.TRIANGLES);
			for (int i = 1; i < count; ++i)
			{
				GL.Color(color);
				GL.Vertex(center);
				GL.Vertex(points[i - 1]);
				GL.Vertex(points[i]);

				GL.Vertex(center);
				GL.Vertex(points[i]);
				GL.Vertex(points[i - 1]);
			}
			GL.End();
			GL.PopMatrix();
		}

		public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
		{
			Vector3[] points = msSolidArcPoints;
			var count = points.Length;
			if (count <= 0)
				return;

			SetDiscSectionPoints(points, center, normal, from, angle, radius);
			GL.PushMatrix();
			GL.Begin(GL.TRIANGLES);
			for (int i = 1; i < count; ++i)
			{
				GL.Color(color);
				GL.Vertex(center);
				GL.Vertex(points[i - 1]);
				GL.Vertex(points[i]);

				GL.Vertex(center);
				GL.Vertex(points[i]);
				GL.Vertex(points[i - 1]);
			}
			GL.End();
			GL.PopMatrix();
		}
	}
}
