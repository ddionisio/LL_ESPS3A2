using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerRandomInterval : MonoBehaviour {	
    public M8.AnimatorParamTrigger trigger;
	public M8.RangeFloat intervalDelayRange;

	private float mLastTriggerTime;
	private float mCurDelay;

	private Animator mAnim;

	void OnEnable() {
		if(mAnim)
			trigger.Reset(mAnim);

		mLastTriggerTime = Time.time;
		mCurDelay = intervalDelayRange.random;
	}

	void Awake() {
		mAnim = GetComponent<Animator>();
	}

	void Update() {
		if(mAnim) {
			var t = Time.time;
			if(t - mLastTriggerTime >= mCurDelay) {
				trigger.Set(mAnim);
				mLastTriggerTime = t;
				mCurDelay = intervalDelayRange.random;
			}
		}
    }
}
