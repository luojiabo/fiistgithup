using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki;

namespace Ubtrobot
{
	/// <summary>
	/// 环境区域
	/// </summary>
	public class EnvironmentZone : Environment
	{
		/// <summary>
		/// Tick
		/// 可以在这里控制环境变化曲线
		/// </summary>
		/// <param name="deltaTime"></param>
		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
		}
	}
}
