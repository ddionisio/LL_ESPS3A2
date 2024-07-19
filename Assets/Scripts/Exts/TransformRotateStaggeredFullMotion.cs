using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotateStaggeredFullMotion : MonoBehaviour {
    public Transform target;

    [Header("Full Rotate")]
	public bool fullRotateClockwise;
	public M8.RangeFloat fullRotateDelayRange;
	public M8.RangeInt fullRotateCountRange;
	public DG.Tweening.Ease fullRotateEase;

    [Header("Stagger Rotate")]
    public M8.RangeFloat staggerRotateRange;
	public M8.RangeFloat staggerRotateDelayRange;
	public M8.RangeInt staggerRotateCountRange;
	public DG.Tweening.Ease staggerRotateEase;

	[Header("Wait")]
	public M8.RangeFloat waitDelayRange;
				
	private float rotate {
		get { return target.localEulerAngles.z; }
		set {
			var rots = target.localEulerAngles;
			if(rots.z != value) {
				rots.z = value;
				target.localEulerAngles = rots;
			}
		}
	}

	private Coroutine mRout;

	private DG.Tweening.EaseFunction mFullRotateEaseFunc;
	private DG.Tweening.EaseFunction mStaggerRotateEaseFunc;

	private M8.WaitForSecondsRandom mWait;

	void OnDisable() {
		if(mRout != null) {
			StopCoroutine(mRout);
			mRout = null;
		}
	}

	void OnEnable() {
		mRout = StartCoroutine(DoRotates());
	}

	void Awake() {
		if(!target) target = transform;

		mFullRotateEaseFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(fullRotateEase);
		mStaggerRotateEaseFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(staggerRotateEase);

		if(waitDelayRange.length > 0f)
			mWait = new M8.WaitForSecondsRandom(waitDelayRange.min, waitDelayRange.max);
	}

	IEnumerator DoRotates() {
		while(true) {			
			float curTime, delay;
			float startRotate, endRotate;

			//full
			startRotate = M8.MathUtil.AngleAbs(rotate);
			var fullRotate = 360f * fullRotateCountRange.random;
			endRotate = fullRotateClockwise ? rotate - fullRotate : rotate + fullRotate;

			curTime = 0f;
			delay = fullRotateDelayRange.random;
			while(curTime < delay) {
				yield return null;

				curTime += Time.deltaTime;

				rotate = Mathf.Lerp(startRotate, endRotate, mFullRotateEaseFunc(curTime, delay, 0f, 0f));
			}

			//stagger
			var originRotate = M8.MathUtil.AngleAbs(rotate);
			var isClockwise = Random.Range(0, 2) == 0;

			var staggerCount = staggerRotateCountRange.random;
			for(int i = 0; i < staggerCount; i++) {
				startRotate = M8.MathUtil.AngleAbs(rotate);
				endRotate = M8.MathUtil.AngleAbs(originRotate + (isClockwise ? -staggerRotateRange.random : staggerRotateRange.random));

				curTime = 0f;
				delay = staggerRotateDelayRange.random;
				while(curTime < delay) {
					yield return null;

					curTime += Time.deltaTime;

					rotate = Mathf.LerpAngle(startRotate, endRotate, mStaggerRotateEaseFunc(curTime, delay, 0f, 0f));
				}

				yield return mWait;

				isClockwise = !isClockwise;
			}
		}
	}
}
