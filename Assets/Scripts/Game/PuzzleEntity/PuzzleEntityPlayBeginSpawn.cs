using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntityPlayBeginSpawn : MonoBehaviour {
	public PuzzleEntity target;
	public PuzzleEntityState toState = PuzzleEntityState.Idle;

	public Transform spawnPointRoot;

	void OnDestroy() {
		if(GameData.isInstantiated)
			GameData.instance.signalPlayBegin.callback -= OnPlayBegin;
	}

	void Awake() {
		GameData.instance.signalPlayBegin.callback += OnPlayBegin;
	}

	void OnPlayBegin() {
		StartCoroutine(DoSpawn());
	}

	IEnumerator DoSpawn() {
		target.position = spawnPointRoot.position;

		target.state = PuzzleEntityState.Spawn;

		while(!target.stateIsComplete)
			yield return null;

		target.state = toState;
	}
}
