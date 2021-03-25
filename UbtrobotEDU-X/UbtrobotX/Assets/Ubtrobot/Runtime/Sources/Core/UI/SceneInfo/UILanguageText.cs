using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Ubtrobot
{
	public static class UILanguageText
	{
		#region UI
		public static string unFoldText => "展开";
		public static string foldTitleText => "环境信息";

		public static string followModel => "跟随模式";
		public static string overallModel => "鸟瞰模式";

		public static string detailsPanelRobotTitle => "的环境参数";

		public static string backTitle => "返回";
		public static string currModleTitle => "当前模型";
		public static string envNameText => "环境信息";
		public static string playStartText => "播放";
		public static string playStopText => "停止";
		public static string showIDTitle => "高亮";
		public static string resetText => "复位";
		public static string magnifyText => "放大";
		public static string shrinkText => "缩小";
		public static string fullScreenText => "全屏";
		#endregion

		#region Scene
		public static string temperature => "温度：{0}℃";
		public static string humidty => "湿度：{0}%";
		public static string luminance => "亮度：{0}Lux";
		public static string soundVolume => "声音：{0}";
		public static string wallCount => "围墙边上数：{0}";
		public static string affectRadius => "距离：{0}cm";

		public static string hvac => "环境温室-通风系统";
		public static string radar => "探测雷达";
		public static string robotticArm => "机械臂";
		public static string vechicle => "汽车控制";
		public static string windmill => "风扇";
		#endregion
	}
}
