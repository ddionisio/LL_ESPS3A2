using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalApplyPowerOnTrigger : GoalApplyPower {

	void OnTriggerEnter2D(Collider2D collision) {
		isApply = true;
	}

	void OnTriggerExit2D(Collider2D collision) {
		isApply = false;
	}
}
