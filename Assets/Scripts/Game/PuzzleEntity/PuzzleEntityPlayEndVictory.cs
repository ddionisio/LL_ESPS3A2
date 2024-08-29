using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntityPlayEndVictory : MonoBehaviour {
	public PuzzleEntitySolid target;

	void OnDestroy() {
		if(GameData.isInstantiated)
			GameData.instance.signalPlayEnd.callback -= OnPlayEnd;
	}

	void Awake() {
		GameData.instance.signalPlayEnd.callback += OnPlayEnd;
	}

	void OnPlayEnd() {
		target.state = PuzzleEntityState.Victory;
	}
}
