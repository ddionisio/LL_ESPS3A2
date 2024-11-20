using UnityEngine;
using UnityEngine.U2D;

public class PuzzleGameplayPowerFillDisplay : MonoBehaviour {
	public AnimationCurve scaleCurve;

    public SpriteShapeRenderer fillRender;
    public M8.RangeFloat fillAlphaRange;

    public ParticleSystem fx;
    public M8.RangeFloat fxEmissionRateRange;

	public GameObject powerActiveGO;

	public M8.AnimatorTargetParamTrigger powerActiveAnimTrigger;

	private float mScale = 0f;

	//[0, 1]
	public void ApplyScale(float t) {
		if(mScale != t) {
			var playActive = mScale <= 0f && t > 0f;

			mScale = t;
			RefreshScale(playActive);
		}
	}

	void OnEnable() {
		RefreshScale(false);
	}

	private void RefreshScale(bool playActive) {
		var t = scaleCurve.Evaluate(mScale);

		if(t > 0f) {
			if(fillRender) {
				fillRender.gameObject.SetActive(true);

				var clr = fillRender.color;
				clr.a = fillAlphaRange.Lerp(t);

				fillRender.color = clr;
			}

			if(fx) {
				var fxMain = fx.main;

				fxMain.loop = true;

				var fxEmission = fx.emission;

				fxEmission.rateOverTime = fxEmissionRateRange.Lerp(t);

				if(!fx.isPlaying)
					fx.Play();
			}

			if(powerActiveGO)
				powerActiveGO.SetActive(true);

			if(playActive)
				powerActiveAnimTrigger.Set();
		}
		else {
			if(fillRender)
				fillRender.gameObject.SetActive(false);

			if(fx) {
				var fxMain = fx.main;

				fxMain.loop = false;

				var fxEmission = fx.emission;

				fxEmission.rateOverTime = fxEmissionRateRange.Lerp(0f);
			}

			if(powerActiveGO)
				powerActiveGO.SetActive(false);
		}
	}
}
