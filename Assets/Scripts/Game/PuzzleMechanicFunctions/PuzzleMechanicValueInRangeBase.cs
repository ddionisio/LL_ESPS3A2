using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleMechanicValueInRangeBase : MonoBehaviour {
	public PuzzleMechanicValueBase mechanicTarget;
	public M8.RangeFloat valueRange;
	public bool applyOnRelease;

	private bool mIsInRange;

	protected abstract void ApplyInRange(bool isInRange);

	void OnDestroy() {
		if(mechanicTarget) {
			if(applyOnRelease)
				mechanicTarget.onInputDown.RemoveListener(OnMechanicInputDown);
			else
				mechanicTarget.onValueChanged.RemoveListener(OnMechanicValueChanged);
		}
	}

	void Awake() {
		if(mechanicTarget) {
			if(applyOnRelease)
				mechanicTarget.onInputDown.AddListener(OnMechanicInputDown);
			else
				mechanicTarget.onValueChanged.AddListener(OnMechanicValueChanged);

			mIsInRange = valueRange.InRange(mechanicTarget.value);
		}
	}

	void OnMechanicValueChanged(float val) {
		var _inRange = valueRange.InRange(val);
		if(mIsInRange != _inRange) {
			mIsInRange = _inRange;
			ApplyInRange(mIsInRange);
		}
	}

	void OnMechanicInputDown(bool isDown) {
		if(!isDown)
			OnMechanicValueChanged(mechanicTarget.value);
	}
}
