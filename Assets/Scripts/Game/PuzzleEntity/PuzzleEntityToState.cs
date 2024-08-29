using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change to a new state after current state is finish
/// </summary>
public class PuzzleEntityToState : MonoBehaviour {
    [System.Serializable]
    public struct Data {
        public PuzzleEntityState fromState;
		public PuzzleEntityState toState;
	}

    public PuzzleEntitySolid target;
	public Data[] changeStates;

	void OnDestroy() {
		if(target)
			target.onStateEnd -= OnStateEnd;
	}

	void Awake() {
		if(!target)
			target = GetComponent<PuzzleEntitySolid>();

		if(target)
			target.onStateEnd += OnStateEnd;
	}

	void OnStateEnd(PuzzleEntityState state) {
		for(int i = 0; i < changeStates.Length; i++) {
			var dat = changeStates[i];
			if(dat.fromState == state) {
				target.state = dat.toState;
				break;
			}
		}
	}
}
