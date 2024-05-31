using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalOnValueActive : MonoBehaviour {
	public GoalController target;

	public M8.RangeFloat valueRange;

	public void OnValue(float val) {
		if(!target) return;

		if(valueRange.InRange(val))
			target.state = GoalState.Active;
		else
			target.state = GoalState.Inactive;
	}

	public void OnValue(int val) {
		OnValue((float)val);
	}

	void Awake() {
		if(!target)
			target = GetComponent<GoalController>();
	}
}
