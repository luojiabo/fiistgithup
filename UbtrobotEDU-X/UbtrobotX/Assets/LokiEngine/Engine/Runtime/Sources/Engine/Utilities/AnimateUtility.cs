using UnityEngine;

namespace Loki
{
	public static class AnimateUtility
	{
		public static void StandardizeLampAnimationClip(AnimationClip clip)
		{
#if UNITY_EDITOR
			var bindings = UnityEditor.AnimationUtility.GetCurveBindings(clip);
			for (int i = 0; i < bindings.Length; i++)
			{
				var binding = bindings[i];
				var curve = UnityEditor.AnimationUtility.GetEditorCurve(clip, binding);
				var length = curve.length;
				for (int j = 0; j < length; j++)
				{
					UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, j, UnityEditor.AnimationUtility.TangentMode.Constant);
					UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, j, UnityEditor.AnimationUtility.TangentMode.Constant);
				}
				UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
			}
#endif
		}

		public static bool Sample(Animation anim, AnimationState state, ref float startTime, ref float endTime, float timeInterval, float length)
		{
			var result = false;
			endTime += Time.deltaTime;
			if (endTime >= length)
			{
				endTime = length + 0.0001f;
				result = true;
			}
			var time = startTime;
			for (float i = startTime; i < endTime; i += timeInterval)
			{
				time = i;
				Sample(anim, state, i);
			}
			startTime = time;
			return result;
		}

		public static void Sample(Animation anim, AnimationState state, float time)
		{
			anim.Play();
			state.enabled = true;
			state.weight = 1;
			state.time = time;
			anim.Sample();
			state.enabled = false;
		}
	}
}
