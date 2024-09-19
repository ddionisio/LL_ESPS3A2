using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalApplyPower : MonoBehaviour {
	[SerializeField]
	GoalController _target;

	public float powerRate;

	public GoalController target {
		get {
			if(!_target)
				_target = GetComponent<GoalController>();

			return _target;
		}
	}

	public bool isApply { get; set; }

	void Update() {
		if(isApply) {
			target.power += powerRate * Time.deltaTime;
			target.DecayReset();
		}
	}
}
