using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleGameplayEntityRespawn : MonoBehaviour {
    public PuzzleEntity target;

    public Transform spawnPointRoot;

    public PuzzleEntityState toState = PuzzleEntityState.Idle;

    public bool autoEnabled;
    public PuzzleEntityState autoFromState = PuzzleEntityState.Action;
    public float autoDelay;

	public bool isBusy { get { return mRout != null; } }

    private bool mIsAutoWait;
    private Coroutine mRout;

    public void Respawn() {
        if(isBusy) return;

        if(!(target.state == PuzzleEntityState.None || target.state == PuzzleEntityState.Spawn || target.state == PuzzleEntityState.Despawn || target.state == PuzzleEntityState.Victory)) {
            StopRout();

			mRout = StartCoroutine(DoRespawn(false));
        }
    }

	void OnDisable() {
        StopRout();
	}

	void OnDestroy() {
		if(autoEnabled) {
            if(target)
			    target.onStateEnd -= OnEntityStateEnd;
		}
	}

	void Awake() {
		if(autoEnabled) {
            target.onStateEnd += OnEntityStateEnd;
        }
	}

	private void StopRout() {
        mIsAutoWait = false;

        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }

	void OnEntityStateEnd(PuzzleEntityState state) {
		if(state == autoFromState) {
			StopRout();

			mRout = StartCoroutine(DoRespawn(true));
		}
		else if(mIsAutoWait || !(target.state == PuzzleEntityState.None || target.state == PuzzleEntityState.Spawn || target.state == PuzzleEntityState.Despawn))
			StopRout();
	}

    IEnumerator DoRespawn(bool isAutoWait) {
        if(isAutoWait) {
            mIsAutoWait = true;

            var curTime = 0f;
            while(curTime < autoDelay) {
                yield return null;
                curTime += Time.deltaTime;
            }

            mIsAutoWait = false;
        }

        target.state = PuzzleEntityState.Despawn;

        while(!target.stateIsComplete)
            yield return null;

        target.position = spawnPointRoot.position;
        target.rotation = 0f;

		target.state = PuzzleEntityState.Spawn;

		while(!target.stateIsComplete)
			yield return null;

        target.state = toState;

		mRout = null;
	}
}
