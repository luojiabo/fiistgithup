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
	public class LineRenderer
	{
		struct LineInfo
		{
			public Color color;
			/// <summary>
			/// The Lines
			/// </summary>
			public Vector3[] positions;

			public static int Comparison(LineInfo x, LineInfo y)
			{
				int hash1 = x.color.GetHashCode();
				int hash2 = y.color.GetHashCode();

				if (hash1 == hash2)
				{
					return x.color == y.color ? 0 : -1;
				}
				if (hash1 < hash2)
				{
					return -1;
				}
				return 1;
			}
		}

		private static readonly MemoryPool<Vector3[]> msPositionsPool;// = new MemoryPool<Vector3[]>();
		private readonly List<LineInfo> mLineInfos = new List<LineInfo>();

		public Material lineMaterial { get; set; }

		static LineRenderer()
		{
			msPositionsPool = new MemoryPool<Vector3[]>();
			msPositionsPool.SetNew(positions =>
			{
				if (positions == null)
					return new Vector3[24];
				return positions;
			});
		}

		public void Drawcall()
		{
			if (mLineInfos.Count <= 0)
			{
				return;
			}

			//if (mLineInfos.Count > 2)
			//{
			//	mLineInfos.Sort(LineInfo.Comparison);
			//}
			if (lineMaterial != null)
			{
				lineMaterial.SetPass(1);
			}

			GL.PushMatrix();
			GL.Begin(GL.LINES);
			//GL.Color(Color.blue);
			foreach (var line in mLineInfos)
			{
				GL.Color(line.color);
				var positions = line.positions;
				for (int i = 1; i < positions.Length; i += 2)
				{
					GL.Vertex(positions[i - 1]);
					GL.Vertex(positions[i]);
				}
			}
			GL.End();
			GL.PopMatrix();
		}

		public void Clear()
		{
			foreach (var line in mLineInfos)
			{
				msPositionsPool.Push(line.positions);
			}
			mLineInfos.Clear();
		}

		public void DrawBox(Bounds bounds, Color color)
		{
			Vector3 v3Center = bounds.center;
			Vector3 v3Extents = bounds.extents;

			var v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);
			var v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);
			var v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);
			var v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);
			var v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
			var v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);
			var v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);
			var v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);

			var positions = msPositionsPool.Pop();
			positions[0] = (v3FrontTopLeft);
			positions[1] = (v3FrontTopRight);
			positions[2] = (v3FrontTopRight);
			positions[3] = (v3FrontBottomRight);
			positions[4] = (v3FrontBottomRight);
			positions[5] = (v3FrontBottomLeft);
			positions[6] = (v3FrontBottomLeft);
			positions[7] = (v3FrontTopLeft);
			positions[8] = (v3BackTopLeft);
			positions[9] = (v3BackTopRight);
			positions[10] = (v3BackTopRight);
			positions[11] = (v3BackBottomRight);
			positions[12] = (v3BackBottomRight);
			positions[13] = (v3BackBottomLeft);
			positions[14] = (v3BackBottomLeft);
			positions[15] = (v3BackTopLeft);
			positions[16] = (v3FrontTopLeft);
			positions[17] = (v3BackTopLeft);
			positions[18] = (v3FrontTopRight);
			positions[19] = (v3BackTopRight);
			positions[20] = (v3FrontBottomRight);
			positions[21] = (v3BackBottomRight);
			positions[22] = (v3FrontBottomLeft);
			positions[23] = (v3BackBottomLeft);

			mLineInfos.Add(new LineInfo() { color = color, positions = positions });
		}


	}
}
