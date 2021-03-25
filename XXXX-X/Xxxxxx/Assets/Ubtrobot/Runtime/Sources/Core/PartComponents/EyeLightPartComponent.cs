using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public partial class EyeLightPartComponent : AdvancedPartComponent
	{
		private static readonly HashSet<ECommand> ms_SupportedCommands = new HashSet<ECommand>
		{
			ECommand.EyeCommand,
		};

		public override DriversType driversType => DriversType.EyesLight;

		public override bool Verify(ICommand command)
		{
			if (command.id != id && command.id != 0)
				return false;

			if (!ms_SupportedCommands.Contains(command.commandID))
				return false;

			return true;
		}
		public override ICommandResponseAsync Execute(ICommand command)
		{
			IProtocol result = null;
			switch (command.commandID)
			{
				case ECommand.EyeCommand:
					{
						result = ExecuteCommand(command);
						break;
					}
			}

			if (result == null)
			{
				DebugUtility.LogError(LoggerTags.Project, "Failure to execute command : {0}", command);
			}
			else
			{
				DebugUtility.Log(LoggerTags.Project, "Success to execute command : {0}", command);
			}
			var cra = new CommandResponseAsync(result);
			cra.host = command.host;
			cra.context = command.context;
			return cra;
		}

		private IProtocol ExecuteCommand(ICommand command)
		{
			var result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			if (command is EyeCommands.DisableLightCommand)
			{
				LightOff();
				result.code = 0;
			}
			else if (command is EyeCommands.EnableLightCommand)
			{
				var cmd = (EyeCommands.EnableLightCommand)command;
				LightUp(cmd.color);
				result.code = 0;
			}
			else if (command is EyeCommands.CustomLightCommand)
			{
				var cmd = (EyeCommands.CustomLightCommand)command;
				ShowCustomLight(cmd.colors, cmd.duration);
				result.code = 0;
			}
			else if (command is EyeCommands.PredefineLightColorCommand)
			{
				var cmd = (EyeCommands.PredefineLightColorCommand)command;
				ShowPredefineLight(cmd.mode, cmd.count);
				result.code = 0;
			}
			else if (command is EyeCommands.ExpressionCommand)
			{
				var cmd = (EyeCommands.ExpressionCommand)command;
				ShowExpression(cmd.mode, cmd.color, cmd.count);
				result.code = 0;
			}

			return result;
		}

		public override void Stop()
		{
			LightOff();
		}
	}

	public partial class EyeLightPartComponent
	{
		private static readonly Dictionary<int, Color> ms_ColorsMap = new Dictionary<int, Color>() 
		{
			{ 1, Color.red},//红
			{ 2, new Color(1, 0.5490196f, 0)},//橙
			{ 3, Color.yellow},//黄
			{ 4, Color.green},//绿
			{ 5, Color.cyan},//青
			{ 6, new Color(0.3686275f, 0.5568628f, 0.9333334f)},//蓝
			{ 7, new Color(0.6117647f, 0.4156863f, 0.9333334f)},//紫
			{ 8, Color.white},//白
			{ 9, Color.gray},//关闭
		};
		private static readonly int ms_DynamicColor = -1;
		private static readonly int ms_DefaultColor = 9;
		private static readonly int ms_LightsCount = 8;
		[SerializeField] private Renderer m_Renderer;
		[SerializeField] private Animation m_Animation;
		/// <summary>
		/// 0：眨眼
		/// 1：害羞
		/// 2：热泪盈眶
		/// 3：泪光闪动
		/// 4：哭泣
		/// 5：晕 
		/// 6：开心
		/// 7：惊讶
		/// 8：呼吸
		/// 9：闪烁
		/// 10：风扇
		/// 11：雨刮
		/// </summary>
		[SerializeField] private List<AnimationClip> m_Expressions;
		/// <summary>
		/// 0：七彩跑马灯
		/// 1：Disco
		/// 2：三原色
		/// 3：颜色堆叠
		/// </summary>
		[SerializeField] private List<AnimationClip> m_PredefineLights;

		private Coroutine m_LightCoroutine;
		private MaterialPropertyBlock m_Block;
		[HideInInspector] public int color0 = ms_DefaultColor;
		[HideInInspector] public int color1 = ms_DefaultColor;
		[HideInInspector] public int color2 = ms_DefaultColor;
		[HideInInspector] public int color3 = ms_DefaultColor;
		[HideInInspector] public int color4 = ms_DefaultColor;
		[HideInInspector] public int color5 = ms_DefaultColor;
		[HideInInspector] public int color6 = ms_DefaultColor;
		[HideInInspector] public int color7 = ms_DefaultColor;

		protected override void Awake()
		{
			base.Awake();
			m_Block = new MaterialPropertyBlock();
			StandardizeLampAnimationClips();
		}

		[Conditional("UNITY_EDITOR")]
		private void StandardizeLampAnimationClips()
		{
			if (m_Expressions != null)
			{
				for (int i = 0; i < m_Expressions.Count; i++)
				{
					AnimateUtility.StandardizeLampAnimationClip(m_Expressions[i]);
				}
			}

			if (m_PredefineLights != null)
			{
				for (int i = 0; i < m_PredefineLights.Count; i++)
				{
					AnimateUtility.StandardizeLampAnimationClip(m_PredefineLights[i]);
				}
			}
		}

		private IEnumerator OnShowCustomLight(Color[] colors, float time)
		{
			for (int i = 0; i < ms_LightsCount; i++)
			{
				SetColor(colors[i], i);
			}
			yield return new WaitForSeconds(time);
			LightOff();
		}

		private IEnumerator OnShowPredefineLight(AnimationClip clip, int count)
		{
			var startTime = 0f;
			var endTime = 0f;
			var timeInterval = 1 / clip.frameRate;
			var length = clip.length;
			var clipName = clip.name;
			m_Animation.clip = clip;
			if (!m_Animation.GetClip(clip.name))
			{
				m_Animation.AddClip(clip, clipName);
			}
			var state = m_Animation[clipName];
			while (true)
			{
				if (AnimateUtility.Sample(m_Animation, state, ref startTime, ref endTime, timeInterval, length))
				{
					if (--count <= 0) break;
					startTime = 0;
					endTime = 0;
				}
				SetColor(color0, 0);
				SetColor(color1, 1);
				SetColor(color2, 2);
				SetColor(color3, 3);
				SetColor(color4, 4);
				SetColor(color5, 5);
				SetColor(color6, 6);
				SetColor(color7, 7);
				yield return null;
			}
			LightOff();
		}

		private IEnumerator OnShowExpression(AnimationClip clip, Color color, int count)
		{
			var startTime = 0f;
			var endTime = 0f;
			var timeInterval = 1 / clip.frameRate;
			var length = clip.length;
			var clipName = clip.name;
			m_Animation.clip = clip;
			if (!m_Animation.GetClip(clip.name))
			{
				m_Animation.AddClip(clip, clipName);
			}
			var state = m_Animation[clipName];
			while (true)
			{
				if (AnimateUtility.Sample(m_Animation, state, ref startTime, ref endTime, timeInterval, length))
				{
					if (--count <= 0) break;
					startTime = 0;
					endTime = 0;
				}

				if (color0 != ms_DynamicColor) SetColor(color0, 0);
				else SetColor(color, 0);

				if (color1 != ms_DynamicColor) SetColor(color1, 1);
				else SetColor(color, 1);

				if (color2 != ms_DynamicColor) SetColor(color2, 2);
				else SetColor(color, 2);

				if (color3 != ms_DynamicColor) SetColor(color3, 3);
				else SetColor(color, 3);

				if (color4 != ms_DynamicColor) SetColor(color4, 4);
				else SetColor(color, 4);

				if (color5 != ms_DynamicColor) SetColor(color5, 5);
				else SetColor(color, 5);

				if (color6 != ms_DynamicColor) SetColor(color6, 6);
				else SetColor(color, 6);

				if (color7 != ms_DynamicColor) SetColor(color7, 7);
				else SetColor(color, 7);

				yield return null;
			}
			LightOff();
		}

		private static Color[] GetColors(int[] colorIndexs)
		{
			var count = colorIndexs.Length;
			var colors = new Color[count];
			for (int i = 0; i < count; i++)
			{
				colors[i] = GetColor(colorIndexs[i]);
			}
			return colors;
		}

		private static Color GetColor(int colorIndex)
		{
			Color color;
			if (ms_ColorsMap.TryGetValue(colorIndex, out color)) return color;
			return ms_ColorsMap[ms_DefaultColor];
		}

		private void SetColor(int colorIndex, int index)
		{
			var color = GetColor(colorIndex);
			SetColor(color, index);
		}

		private void SetColor(Color color, int index)
		{
			m_Renderer.GetPropertyBlock(m_Block, index);
			m_Block.SetColor("_Diffuse", color);
			m_Block.SetColor("_Specular", color);
			m_Renderer.SetPropertyBlock(m_Block, index);
		}

		private void ResetColors()
		{
			color0 = ms_DefaultColor;
			color1 = ms_DefaultColor;
			color2 = ms_DefaultColor;
			color3 = ms_DefaultColor;
			color4 = ms_DefaultColor;
			color5 = ms_DefaultColor;
			color6 = ms_DefaultColor;
			color7 = ms_DefaultColor;
			var color = GetColor(ms_DefaultColor);
			for (int i = 0; i < ms_LightsCount; i++)
			{
				SetColor(color, i);
			}
		}

		[ContextMenu("LightOff")]
		public void LightOff()
		{
			ResetColors();
			m_Animation.Stop();
			m_Animation.clip = null;
			if (m_LightCoroutine == null) return;
			StopCoroutine(m_LightCoroutine);
			m_LightCoroutine = null;
		}

		public void LightUp(Color color)
		{
			LightOff();
			for (int i = 0; i < ms_LightsCount; i++)
			{
				SetColor(color, i);
			}
		}

		public void LightUp(Color[] colors)
		{
			if (colors == null || colors.Length != ms_LightsCount) return;
			LightOff();
			for (int i = 0; i < ms_LightsCount; i++)
			{
				SetColor(colors[i], i);
			}
		}

		public void ShowCustomLight(int[] colorIndexs, float time)
		{
			if (colorIndexs == null || colorIndexs.Length != ms_LightsCount) return;
			LightOff();
			var colors = GetColors(colorIndexs);
			m_LightCoroutine = StartCoroutine(OnShowCustomLight(colors, time));
		}

		public void ShowPredefineLight(int type, int count)
		{
			if (type < 0 || type >= m_PredefineLights.Count) return;
			var clip = m_PredefineLights[type];
			if (clip == null) return;
			LightOff();
			m_LightCoroutine = StartCoroutine(OnShowPredefineLight(clip, count));
		}

		public void ShowExpression(int type, Color color, int count)
		{
			if (type < 0 || type >= m_Expressions.Count) return;
			var clip = m_Expressions[type];
			if (clip == null) return;
			LightOff();
			m_LightCoroutine = StartCoroutine(OnShowExpression(clip, color, count));
		}
	}
}
