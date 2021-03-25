#define ENABLE_DOTWEEN

#if ENABLE_DOTWEEN
using DG.Tweening;
using Tween = DG.Tweening.Tween;
#else
namespace Loki
{
	public class Tween
	{

	}
}
#endif

namespace Loki
{
	public struct TweenProperty
	{
		private Tween mTween;

		public static implicit operator Tween(TweenProperty property)
		{
			return property.mTween;
		}

		public static implicit operator TweenProperty(Tween tween)
		{
			return new TweenProperty(tween);
		}

		public TweenProperty(Tween tween)
		{
			this.mTween = tween;
		}

		public void SetTween(Tween tween, bool forceCompletedPrev = false)
		{
			Stop(forceCompletedPrev);
			this.mTween = tween;
		}

		public void Stop(bool forceCompleted = false)
		{
			if (mTween != null)
			{
#if ENABLE_DOTWEEN
				mTween.Kill(forceCompleted);
#endif
				mTween = null;
			}
		}
	}
}
