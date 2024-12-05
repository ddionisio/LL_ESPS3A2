using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotateStaggeredMotionMulti : MonoBehaviour {
	[System.Serializable]
	public class Data {
		public Transform target;
		public M8.RangeFloat rotateRange;

		private float mStart;
		private float mEnd;

		public float rotate {
			get { return target.localEulerAngles.z; }
			set {
				var rots = target.localEulerAngles;
				if(rots.z != value) {
					rots.z = value;
					target.localEulerAngles = rots;
				}
			}
		}

		public float start { get { return mStart; } }
		public float end { get { return mEnd; } }

		public void Init(bool isEnd) {
			if(isEnd)
				rotate = rotateRange.max;
			else
				rotate = rotateRange.min;
		}

		public void SetupFull(bool isEnd) {
			mStart = M8.MathUtil.AngleAbs(rotate);
			mEnd = M8.MathUtil.AngleAbs(isEnd ? rotateRange.min : rotateRange.max);
		}

		public void SetupStagger(bool isEnd, float t) {
			mStart = M8.MathUtil.AngleAbs(rotate);
			mEnd = M8.MathUtil.AngleAbs(isEnd ? Mathf.Lerp(rotateRange.min, rotateRange.Lerp(0.5f), t) : Mathf.Lerp(rotateRange.Lerp(0.5f), rotateRange.max, t));
		}

		public void Lerp(float t) {
			rotate = Mathf.Lerp(mStart, mEnd, t);
		}
	}

	[Header("Full Rotate")]
	public Data[] rotates;
	public M8.RangeInt rotateCountRange;
	public M8.RangeFloat rotateDelayRange;

	[Header("Stagger Rotate")]
	public M8.RangeFloat staggerRotateDelayRange;
	public M8.RangeInt staggerRotateCountRange;

	[Header("Wait")]
	public M8.RangeFloat waitDelayRange;
		
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
		if(waitDelayRange.length > 0f)
			mWait = new M8.WaitForSecondsRandom(waitDelayRange.min, waitDelayRange.max);
	}

	IEnumerator DoRotates() {
		float curTime, delay, t;

		//initial
		bool isEnd = Random.Range(0, 2) == 1;

		for(int i = 0; i < rotates.Length; i++)
			rotates[i].Init(isEnd);

		while(true) {
			//full
			delay = rotateDelayRange.random;

			var rotCount = rotateCountRange.random;
			for(int i = 0; i < rotCount; i++) {
				for(int j = 0; j < rotates.Length; j++)
					rotates[j].SetupFull(isEnd);

				curTime = 0f;
				while(curTime < delay) {
					yield return null;

					curTime += Time.deltaTime;

					t = -(Mathf.Cos(Mathf.PI * (curTime / delay)) - 1f) * 0.5f;

					for(int j = 0; j < rotates.Length; j++)
						rotates[j].Lerp(t);
				}

				isEnd = !isEnd;
			}

			//stagger
			var staggerCount = staggerRotateCountRange.random;
			for(int i = 0; i < staggerCount; i++) {
				t = Random.value;

				for(int j = 0; j < rotates.Length; j++)
					rotates[j].SetupStagger(isEnd, t);

				curTime = 0f;
				delay = staggerRotateDelayRange.random;
				while(curTime < delay) {
					yield return null;

					curTime += Time.deltaTime;

					t = -(Mathf.Cos(Mathf.PI * (curTime / delay)) - 1f) * 0.5f;

					for(int j = 0; j < rotates.Length; j++)
						rotates[j].Lerp(t);
				}

				isEnd = !isEnd;

				yield return mWait;
			}
		}
	}
}
