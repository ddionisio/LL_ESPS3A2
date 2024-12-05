using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalApplyPowerOnTrigger : GoalApplyPower {

	public float exitDelay = 1f;

	private bool mTriggerIsExit;
	private float mLastTriggerExitTime;

	void OnTriggerEnter2D(Collider2D collision) {
		isApply = true;
		mTriggerIsExit = false;
	}

	void OnTriggerExit2D(Collider2D collision) {
		mTriggerIsExit = true;
		mLastTriggerExitTime = Time.time;
	}

	protected override void Update() {
		if(mTriggerIsExit && (Time.time - mLastTriggerExitTime) >= exitDelay) {
			isApply = false;
			mTriggerIsExit = false;
		}

		base.Update();
	}
}
