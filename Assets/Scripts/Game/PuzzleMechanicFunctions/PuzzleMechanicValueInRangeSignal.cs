using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMechanicValueInRangeSignal : PuzzleMechanicValueInRangeBase {
	public M8.SignalBoolean signalInvokeValueInRange;
	public M8.Signal signalInvokeValueInRangeTrue;
	public M8.Signal signalInvokeValueInRangeFalse;

	protected override void ApplyInRange(bool isInRange) {
		signalInvokeValueInRange?.Invoke(isInRange);

		if(isInRange)
			signalInvokeValueInRangeTrue?.Invoke();
		else
			signalInvokeValueInRangeFalse?.Invoke();
	}
}
