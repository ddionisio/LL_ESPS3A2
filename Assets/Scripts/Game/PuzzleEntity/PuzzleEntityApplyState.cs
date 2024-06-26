using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntityApplyState : MonoBehaviour {
	public PuzzleEntity target;

	public PuzzleEntityState state;

	public void Apply() {
		target.state = state;
	}
}
