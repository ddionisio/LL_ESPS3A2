using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotateStaggeredFullMotion : MonoBehaviour {
    public Transform target;

    [Header("Full Rotate")]
	public bool fullRotateClockwise;
	public M8.RangeFloat fullRotateDelayRange;
	public M8.RangeInt fullRotateCountRange;

    [Header("Stagger Rotate")]
    public M8.RangeFloat staggerRotateRange;
	public M8.RangeFloat staggerRotateDelayRange;
	public M8.RangeInt staggerRotateCountRange;

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

		if(waitDelayRange.length > 0f)
			mWait = new M8.WaitForSecondsRandom(waitDelayRange.min, waitDelayRange.max);
	}

	IEnumerator DoRotates() {
		float curTime, delay;
		float startRotate, endRotate;

		while(true) {
			//full
			startRotate = M8.MathUtil.AngleAbs(rotate);
			var fullRotate = 360f * fullRotateCountRange.random;
			endRotate = fullRotateClockwise ? rotate - fullRotate : rotate + fullRotate;

			curTime = 0f;
			delay = fullRotateDelayRange.random;
			while(curTime < delay) {
				yield return null;

				curTime += Time.deltaTime;

				rotate = Mathf.Lerp(startRotate, endRotate, -(Mathf.Cos(Mathf.PI * (curTime / delay)) - 1f) * 0.5f);
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

					rotate = Mathf.LerpAngle(startRotate, endRotate, -(Mathf.Cos(Mathf.PI * (curTime / delay)) - 1f) * 0.5f);
				}

				yield return mWait;

				isClockwise = !isClockwise;
			}
		}
	}
}
