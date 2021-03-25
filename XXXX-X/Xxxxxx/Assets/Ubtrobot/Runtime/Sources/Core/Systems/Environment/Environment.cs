using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[DynamicSceneDrawer(sceneTitle = "环境", tooltip = "模拟环境输入参数, 如果环境不在传感器的捕获范围，将使用默认环境")]
	public abstract class Environment : UComponent, IUpdatable
	{
		/// <summary>
		/// 光源
		/// </summary>
		[Tooltip("太阳")]
		public Light sun;

		/// <summary>
		/// 是否默认环境
		/// </summary>
		[Tooltip("是否默认环境")]
		public bool isDefault = false;

		/// <summary>
		/// 温度-摄氏度C
		/// </summary>
		[Tooltip("单位:摄氏度. -273.15摄氏度为绝对零度, 5500摄氏度为太阳表面温度")]
		[Range(-273.15f, 5500.0f)]
		public float temperature = 25.0f;

		/// <summary>
		/// 湿度
		/// 湿度100%指的是相对湿度.
		/// 绝对湿度=单位质量大气中含有多少水汽单位质量大气中不能含有无限多的水汽, 有一个上限, 达到这个限制, 大气就达到饱和了.
		/// 再增加水汽, 多出来的部分会凝结成液态.
		/// 这个上限由大气的温度决定, 温度越高大气包含水汽的能力越高.
		/// 相对湿度=实际水汽含量/饱和时的水汽含量
		/// <see cref=""/>
		/// </summary>
		[Tooltip("相对湿度, [0% ~ 100%]. 相对湿度=实际水汽含量/饱和时的水汽含量")]
		[Range(0.0f, 100.0f)]
		public float humidity = 50f;

		/// <summary>
		/// 流明
		/// </summary>
		[Tooltip("采用光源在单位时间内发射出的光量称为光源的发光通量（流明）来描述，这里仅是一个数值模拟.")]
		[Range(0.0f, 3000.0f)]
		public float luminance = 800f;

		/// <summary>
		/// 声音大小
		/// </summary>
		[Tooltip("音量大小")]
		[Range(0.0f, 100.0f)]
		public float soundVolume = 40.0f;

		[Tooltip("环境参数影响半径")]
		[Range(1.0f, 100.0f)]
		public float affectRadius = 5.0f;

		[Tooltip("环境参数影响数量")]
		[Range(3.0f, 100.0f)]
		public float affectCount = 3.0f;


#if UNITY_EDITOR
		protected string DynamicDrawerName(string title)
		{
			if (isDefault)
			{
				return string.Concat(title, "(默认)");
			}
			return title;
		}

		private void OnDrawGizmos()
		{
			EditorHelper.OnSceneGUIDrawTitle(this, Color.green);
			if (isDefault)
			{
				return;
			}
			using (new GizmosColorScope(Color.blue))
			{
				Gizmos.DrawWireSphere(transform.position, affectRadius);
			}
		}
#endif

		/// <summary>
		/// 注册
		/// </summary>
		protected virtual void OnEnable()
		{
			EnvironmentSystem.GetOrAlloc().Register(this);
		}

		/// <summary>
		/// 反注册
		/// </summary>
		protected virtual void OnDisable()
		{
			if (EnvironmentSystem.Get())
				EnvironmentSystem.Get().Unregister(this);
		}

		/// <summary>
		/// 判断这个位置以及其朝向的物体是否匹配此环境对象
		/// </summary>
		/// <param name="position">需要感知环境的位置</param>
		/// <param name="forward">需要感知环境的目标朝向</param>
		/// <returns></returns>
		public virtual bool IsMatch(Vector3 position, Vector3 forward)
		{
			if (isDefault)
			{
				return false;
			}

			if ((position - transform.position).sqrMagnitude <= affectRadius * affectRadius)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Tick
		/// 可以在这里控制环境变化曲线
		/// </summary>
		/// <param name="deltaTime"></param>
		public virtual void OnUpdate(float deltaTime)
		{
			if (sun != null)
			{
				sun.intensity = luminance * 0.01f;
			}
		}

	}
}
