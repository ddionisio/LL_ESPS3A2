using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalApplyPower : MonoBehaviour {
	public GoalController target;

	public float power;

	private bool mIsApplied;

	public void Apply(bool isApply) {
		if(mIsApplied != isApply) {
			mIsApplied = isApply;

			if(mIsApplied)
				target.AddPower(power);
			else
				target.AddPower(-power);
		}
	}

	void Awake() {
		if(!target)
			target = GetComponent<GoalController>();
	}
}
