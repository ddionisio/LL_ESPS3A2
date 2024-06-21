using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalApplyPowerOnValue : GoalApplyPower {
	
	public M8.RangeFloat valueRange;

	public void OnValue(float val) {
		if(!target) return;

		isApply = valueRange.InRange(val);
	}

	public void OnValue(int val) {
		OnValue((float)val);
	}
}
