using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ubtrobot
{
	public static class CalculateLengthTools
	{
		public static float CalculateEquilateralLength(int wallsCount, float radius)
		{
			if (wallsCount == 0)
				return 0;
			
			float angle = 360f / (wallsCount * 2);
			return radius * Mathf.Tan((float)Math.PI * angle / 180.0f) * 2.0f;
		}
	}

	public class RadarSceneInfo : SceneInfo
	{
		private readonly List<Transform> wallsList = new List<Transform>();
		private Transform prefabWall;
		private int oldCount = 0;
		private float oldRadius = 0f;
		private Vector3 oldPos = Vector3.zero;
		private Vector3 tempVec = Vector3.one;

		private Environment lastEnv = null;

		protected override void OnInitialize()
		{
			base.OnInitialize();

			prefabWall = transform.GetChild(0);
			tempVec.y = prefabWall.localScale.y;
			tempVec.z = prefabWall.localScale.z;
			UpdateEnv(activeEnv);
		}

		private void UpdateEnv(Environment env)
		{
			if (env != lastEnv)
			{
				lastEnv = env;

				if (env != null)
				{
					oldCount = (int)env.affectCount;
					oldRadius = env.affectRadius;
				}
				else
				{
					oldCount = 3;
					oldRadius = 100.0f;
				}

				if (modelHandle != null)
				{
					oldPos = new Vector3(modelHandle.forward.x, 0, modelHandle.forward.z).normalized * oldRadius + modelHandle.position;
				}
				else
				{
					oldPos = Vector3.forward * oldRadius;
				}

				prefabWall.position = new Vector3(oldPos.x, prefabWall.position.y, oldPos.z);
				oldPos = prefabWall.position;
				LookAtModelHandle(prefabWall);
				prefabWall.gameObject.SetActive(false);
				UpdateWalls(oldCount);
			}
		}

		private void Update()
		{
			UpdateEnv(activeEnv);

			if (oldCount != (int)activeEnv.affectCount
				|| !Loki.Misc.Nearly(oldRadius, activeEnv.affectRadius, 0.001f))
			{
				UpdateWalls((int)activeEnv.affectCount);
			}
		}

		private void UpdateWalls(int runtimeWallsCount)
		{
			List<Transform> activeList = wallsList.FindAll((tempTrans) => tempTrans.gameObject.activeSelf);

			int residue = activeList.Count - runtimeWallsCount;
			if (residue < 0)
			{
				for (int i = 0; i < -residue; i++)
				{
					Transform tempTransform = wallsList.Find((temp) => !temp.gameObject.activeSelf);

					if (tempTransform == null)
					{
						GameObject tempObj = Instantiate(prefabWall.gameObject, transform);
						tempObj.gameObject.SetActive(true);
						wallsList.Add(tempObj.transform);
					}
					else
					{
						tempTransform.gameObject.SetActive(true);
					}
				}
			}
			else if (residue > 0)
			{
				for (int i = 0; i < residue; i++)
				{
					activeList[i].gameObject.SetActive(false);
				}
			}

			LayoutWalls();

			oldRadius = activeEnv.affectRadius;
			oldCount = (int)activeEnv.affectCount;
		}

		private void LayoutWalls()
		{
			if (wallsList.Count <= 0)
			{
				return;
			}

			List<Transform> activeList = wallsList.FindAll((tempTrans) => tempTrans.gameObject.activeSelf == true);

			oldPos.z = activeEnv.affectRadius;

			tempVec.x = CalculateLengthTools.CalculateEquilateralLength((int)activeEnv.affectCount, activeEnv.affectRadius);

			float angle = 360f / activeList.Count;
			for (int i = 0; i < activeList.Count; i++)
			{
				activeList[i].position = oldPos;
				activeList[i].RotateAround(GetModelHandlePosition(activeList[i].position.y), Vector3.up, angle * i);
				LookAtModelHandle(activeList[i]);
				activeList[i].localScale = tempVec;
			}
		}
	}
}
