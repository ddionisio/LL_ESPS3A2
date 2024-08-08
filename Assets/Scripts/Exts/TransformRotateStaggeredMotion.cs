using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotateStaggeredMotion : MonoBehaviour {
	public Transform target;

	[Header("Full Rotate")]
	public M8.RangeFloat rotateRange;
	public M8.RangeInt rotateCountRange;
	public DG.Tweening.Ease rotateEase;
	public M8.RangeFloat rotateDelayRange;

	[Header("Stagger Rotate")]
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

		mFullRotateEaseFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(rotateEase);
		mStaggerRotateEaseFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(staggerRotateEase);

		if(waitDelayRange.length > 0f)
			mWait = new M8.WaitForSecondsRandom(waitDelayRange.min, waitDelayRange.max);
	}

	IEnumerator DoRotates() {
		float curTime, delay;
		float startRotate, endRotate;

		//initial
		bool isEnd = Random.Range(0, 2) == 1;
		if(isEnd)
			rotate = rotateRange.max;
		else
			rotate = rotateRange.min;

		while(true) {
			//full
			delay = rotateDelayRange.random;

			var rotCount = rotateCountRange.random;
			for(int i = 0; i < rotCount; i++) {
				startRotate = M8.MathUtil.AngleAbs(rotate);
				endRotate = M8.MathUtil.AngleAbs(isEnd ? rotateRange.min : rotateRange.max);

				curTime = 0f;				
				while(curTime < delay) {
					yield return null;

					curTime += Time.deltaTime;

					rotate = Mathf.Lerp(startRotate, endRotate, mFullRotateEaseFunc(curTime, delay, 0f, 0f));
				}

				isEnd = !isEnd;
			}

			//stagger
			var staggerCount = staggerRotateCountRange.random;
			for(int i = 0; i < staggerCount; i++) {
				startRotate = M8.MathUtil.AngleAbs(rotate);
				endRotate = M8.MathUtil.AngleAbs(isEnd ? Mathf.Lerp(rotateRange.min, rotateRange.Lerp(0.5f), Random.value) : Mathf.Lerp(rotateRange.Lerp(0.5f), rotateRange.max, Random.value));

				curTime = 0f;
				delay = staggerRotateDelayRange.random;
				while(curTime < delay) {
					yield return null;

					curTime += Time.deltaTime;

					rotate = Mathf.Lerp(startRotate, endRotate, mStaggerRotateEaseFunc(curTime, delay, 0f, 0f));
				}

				isEnd = !isEnd;

				yield return mWait;
			}
		}
	}
}
