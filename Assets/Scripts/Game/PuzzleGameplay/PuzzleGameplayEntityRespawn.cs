using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleGameplayEntityRespawn : MonoBehaviour {
    public PuzzleEntity target;

    public Transform spawnPointRoot;

    public PuzzleEntityState toState = PuzzleEntityState.Idle;

	public UnityEvent<bool> onBusyChanged;

	public bool isBusy { get { return mRout != null; } }

    private Coroutine mRout;

    public void Respawn() {
        if(isBusy) return;

        if(!(target.state == PuzzleEntityState.None || target.state == PuzzleEntityState.Spawn || target.state == PuzzleEntityState.Despawn || target.state == PuzzleEntityState.Victory))
            mRout = StartCoroutine(DoRespawn());
    }

    IEnumerator DoRespawn() {
        onBusyChanged?.Invoke(true);

        target.state = PuzzleEntityState.Despawn;

        while(!target.stateIsComplete)
            yield return null;

        target.position = spawnPointRoot.position;

		target.state = PuzzleEntityState.Spawn;

		while(!target.stateIsComplete)
			yield return null;

        target.state = toState;

		mRout = null;

		onBusyChanged?.Invoke(false);
	}
}
