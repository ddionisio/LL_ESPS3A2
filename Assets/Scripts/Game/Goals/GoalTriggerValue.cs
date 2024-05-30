using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GoalController))]
public class GoalTriggerValue : MonoBehaviour {
    public M8.RangeFloat valueRange;

    private GoalController mGoalCtrl;

	public void CheckValue(float val) {
		if(valueRange.InRange(val))
			mGoalCtrl.state = GoalState.Active;
		else
			mGoalCtrl.state = GoalState.Inactive;
	}

	public void CheckValue(int val) {
		CheckValue((float)val);
	}

	void Awake() {
		mGoalCtrl = GetComponent<GoalController>();
	}
}
