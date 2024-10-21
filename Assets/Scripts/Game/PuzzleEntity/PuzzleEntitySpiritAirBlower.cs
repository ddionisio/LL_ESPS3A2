using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleEntitySpiritAirBlower : MonoBehaviour {
	[Header("Data")]
	public float actionValue;
	public float actionDelay = 0.5f;

	[Header("Animation")]
	public M8.AnimatorParamTrigger animEnter;
	public M8.AnimatorParamTrigger animAction;

	[Header("Events")]
	public UnityEvent<float> actionEvent;

	private float mActionLastTime;
	private Animator mAnim;

	public void Action() {
		if(Time.time - mActionLastTime > actionDelay) {
			if(mAnim != null)
				animAction.Set(mAnim);

			actionEvent.Invoke(actionValue);

			mActionLastTime = Time.time;
		}
	}

	void OnEnable() {
		if(mAnim) {
			animEnter.Set(mAnim);
		}

		mActionLastTime = 0f;
	}

	void Awake() {
		mAnim = GetComponent<Animator>();
	}
}
