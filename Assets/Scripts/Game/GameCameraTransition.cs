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

	public void SetCurrentRoot(Transform root) {
		//currently transitioning?
		if(mRout != null) {
			Transition(root);
			return;
		}

		if(mCurrentRoot)
			mCurrentRoot.gameObject.SetActive(false);

		mCurrentRoot = root;

		if(mCurrentRoot) {
			position = mCurrentRoot.position;

			mCurrentRoot.gameObject.SetActive(true);
		}
	}
	
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

		yield return M8.AnimatorUtil.WaitNextState(animatorFadeOutTrigger.target);

		if(mCurrentRoot)
			mCurrentRoot.gameObject.SetActive(false);

		mCurrentRoot = toRoot;
		mCurrentRoot.gameObject.SetActive(true);
		position = mCurrentRoot.position;

		animatorFadeInTrigger.Set();

		yield return M8.AnimatorUtil.WaitNextState(animatorFadeInTrigger.target);

		mRout = null;
	}
}
