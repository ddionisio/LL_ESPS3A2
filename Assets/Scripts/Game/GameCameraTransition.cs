using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraTransition : M8.SingletonBehaviour<GameCameraTransition> {
	public M8.AnimatorTargetParamTrigger animatorFadeOutTrigger;
	public M8.AnimatorTargetParamTrigger animatorFadeInTrigger;

	public Transform startRoot;

	public Vector2 position { 
		get { return transform.position; }
		private set {
			var pos = transform.position;
			if(pos.x != value.x || pos.y != value.y) {
				pos.x = value.x;
				pos.y = value.y;
				transform.position = pos;
			}
		}
	}

	public bool isBusy { get { return mRout != null; } }

	private Coroutine mRout;

	private Transform mCurrentRoot;
	
	public void Transition(Transform toRoot) {
		if(mRout != null)
			StopCoroutine(mRout);

		mRout = StartCoroutine(DoTransition(toRoot));
	}

	protected override void OnInstanceInit() {
		mCurrentRoot = startRoot;
		if(mCurrentRoot) {
			mCurrentRoot.gameObject.SetActive(true);
			position = mCurrentRoot.position;
		}
	}

	private IEnumerator DoTransition(Transform toRoot) {
		animatorFadeOutTrigger.Set();

		while(true) {
			yield return null;

			var state = animatorFadeOutTrigger.target.GetCurrentAnimatorStateInfo(0);
			if(state.normalizedTime >= 1f)
				break;
		}

		if(mCurrentRoot)
			mCurrentRoot.gameObject.SetActive(false);

		mCurrentRoot = toRoot;
		mCurrentRoot.gameObject.SetActive(true);
		position = mCurrentRoot.position;

		animatorFadeInTrigger.Set();

		while(true) {
			yield return null;

			var state = animatorFadeInTrigger.target.GetCurrentAnimatorStateInfo(0);
			if(state.normalizedTime >= 1f)
				break;
		}

		mRout = null;
	}
}
