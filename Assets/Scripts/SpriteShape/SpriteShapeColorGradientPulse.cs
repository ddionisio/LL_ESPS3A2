using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeColorGradientPulse : MonoBehaviour {
	public SpriteShapeRenderer target;

	public Gradient gradient;

	public float pulsePerSecond;
	public bool isRealTime;

	private float mCurPulseTime = 0;
	private bool mStarted = false;
	private float mLastTime;

	private Color mDefaultColor;

	void OnEnable() {
		if(mStarted) {
			mCurPulseTime = 0;
			mLastTime = isRealTime ? Time.realtimeSinceStartup : Time.time;
			target.color = gradient.Evaluate(0f);
		}
	}

	void OnDisable() {
		if(mStarted) {
			target.color = mDefaultColor;
		}
	}

	void Awake() {
		if(target == null)
			target = GetComponent<SpriteShapeRenderer>();

		mDefaultColor = target.color;
	}

	// Use this for initialization
	void Start() {
		mStarted = true;
		target.color = gradient.Evaluate(0f);
	}

	// Update is called once per frame
	void Update() {
		float time = isRealTime ? Time.realtimeSinceStartup : Time.time;
		float delta = time - mLastTime;
		mLastTime = time;

		mCurPulseTime += delta;

		float t = Mathf.Sin(Mathf.PI * mCurPulseTime * pulsePerSecond);
		t *= t;

		target.color = gradient.Evaluate(t);
	}
}
