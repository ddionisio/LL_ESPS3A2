using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalApplyPowerOnValue : GoalApplyPower {
	
	public M8.RangeFloat valueRange;

	public SignalMechanic signalListenMechanicValueChanged;

	public void OnValue(float val) {
		if(!target) return;

		isApply = valueRange.InRange(val);
	}

	public void OnValue(int val) {
		OnValue((float)val);
	}

	void OnMechanicValueChanged(PuzzleMechanicBase mechanic) {
		var mechanicalVal = mechanic as PuzzleMechanicValueBase;

		if(mechanicalVal)
			OnValue(mechanicalVal.value);
	}

	void OnDestroy() {
		if(signalListenMechanicValueChanged)
			signalListenMechanicValueChanged.callback -= OnMechanicValueChanged;
	}

	void Awake() {
		if(signalListenMechanicValueChanged)
			signalListenMechanicValueChanged.callback += OnMechanicValueChanged;
	}
}
