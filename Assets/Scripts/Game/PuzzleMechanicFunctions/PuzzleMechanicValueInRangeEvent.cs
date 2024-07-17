using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleMechanicValueInRangeEvent : PuzzleMechanicValueInRangeBase {
	public UnityEvent<bool> onValueInRange;
	public UnityEvent onValueInRangeTrue;
	public UnityEvent onValueInRangeFalse;

	protected override void ApplyInRange(bool isInRange) {
		onValueInRange?.Invoke(isInRange);

		if(isInRange)
			onValueInRangeTrue?.Invoke();
		else
			onValueInRangeFalse?.Invoke();
	}
}
