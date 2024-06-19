using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalOnValuePower : MonoBehaviour {
	public GoalController target;

	public float power;

	public M8.RangeFloat valueRange;

	private bool mIsPower;

	public void OnValue(float val) {
		if(!target) return;

		var isPower = valueRange.InRange(val);
		if(mIsPower != isPower) {
			mIsPower = isPower;
			target.AddPower(mIsPower ? power : -power);
		}
	}

	public void OnValue(int val) {
		OnValue((float)val);
	}

	void Awake() {
		if(!target)
			target = GetComponent<GoalController>();
	}
}
