using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleMechanicValueInRangeEvent : MonoBehaviour {
	public M8.RangeFloat valueRange;

	public SignalMechanic signalListenMechanicValueChanged;

	public UnityEvent<bool> onValueInRange;
	public UnityEvent onValueInRangeTrue;
	public UnityEvent onValueInRangeFalse;

	public void OnValue(float val) {
		if(valueRange.InRange(val)) {
			onValueInRange?.Invoke(true);
			onValueInRangeTrue?.Invoke();
		}
		else {
			onValueInRange?.Invoke(false);
			onValueInRangeFalse?.Invoke();
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
