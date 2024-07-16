using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMechanicValueInRangeSignal : MonoBehaviour {
	public M8.RangeFloat valueRange;

	public SignalMechanic signalListenMechanicValueChanged;

	public M8.SignalBoolean signalInvokeValueInRange;
	public M8.Signal signalInvokeValueInRangeTrue;
	public M8.Signal signalInvokeValueInRangeFalse;

	public void OnValue(float val) {
		if(valueRange.InRange(val)) {
			signalInvokeValueInRange?.Invoke(true);
		}
		else {
			signalInvokeValueInRange?.Invoke(false);
		}
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
