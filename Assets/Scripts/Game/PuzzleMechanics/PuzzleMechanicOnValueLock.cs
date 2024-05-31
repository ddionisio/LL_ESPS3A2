using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMechanicOnValueLock : MonoBehaviour {
	public PuzzleMechanicBase target;

	public M8.RangeFloat valueRange;

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

	void Awake() {
		if(!target)
			target = GetComponent<PuzzleMechanicBase>();
	}
}
