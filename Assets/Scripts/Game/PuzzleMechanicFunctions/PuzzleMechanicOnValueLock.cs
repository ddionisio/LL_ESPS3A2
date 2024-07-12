using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMechanicOnValueLock : MonoBehaviour {
	[SerializeField]
	PuzzleMechanicBase _target;

	public M8.RangeFloat valueRange;

	public SignalMechanic signalListenMechanicValueChanged;

	public PuzzleMechanicBase target {
		get {
			if(!_target)
				_target = GetComponent<PuzzleMechanicBase>();

			return _target;
		}
	}

	public void OnValue(float val) {
		if(!target) return;

		if(valueRange.InRange(val))
			target.locked = true;
		else
			target.locked = false;
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
