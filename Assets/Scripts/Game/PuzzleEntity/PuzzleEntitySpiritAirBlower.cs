using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleEntitySpiritAirBlower : MonoBehaviour {
	[Header("Animation")]
	public M8.AnimatorParamTrigger animEnterTrigger;
	public M8.AnimatorParamTrigger animActionTrigger;

	public UnityEvent action;

	public bool isBusy { get { return mRout != null; } }

	private Coroutine mRout;

	private Animator mAnim;

	public void Action() {
		if(isBusy)
			return;

		mRout = StartCoroutine(DoAction());
	}

	void OnDisable() {
		if(mRout != null) {
			StopCoroutine(mRout);
			mRout = null;
		}
	}

	void Awake() {
		mAnim = GetComponent<Animator>();
	}

	IEnumerator DoAction() {
		action.Invoke();

		if(mAnim) {
			animActionTrigger.Set(mAnim);

			yield return M8.AnimatorUtil.WaitNextState(mAnim);
		}
		else
			yield return null;

		mRout = null;
	}
}
