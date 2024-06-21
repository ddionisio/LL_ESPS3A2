using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalApplyPower : MonoBehaviour {
	public GoalController target;

	public float powerRate;

	public bool isApply { get; set; }

	void Awake() {
		if(!target)
			target = GetComponent<GoalController>();
	}

	void Update() {
		if(isApply)
			target.power += powerRate * Time.deltaTime;
	}
}
