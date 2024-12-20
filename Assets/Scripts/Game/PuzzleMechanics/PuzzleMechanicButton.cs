using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PuzzleMechanicButton : PuzzleMechanicBase {
	[Header("Button Config")]
	public bool isHold;
	public float holdDelay;

	public UnityEvent onClick;

	public UnityEvent<bool> onHold;
	public UnityEvent<float> onHoldProgress; //value = [0, 1]

	public bool isHolding { get { return mHoldRout != null; } }

	private Coroutine mHoldRout;

	protected virtual void Click() {
		onClick?.Invoke();
	}

	protected override void InputDown(bool isDown) {
		if(isHold && isDown && mHoldRout == null)
			mHoldRout = StartCoroutine(DoHold());
	}

	protected override void InputClick(PointerEventData eventData) {
		if(!isHold)
			Click();
	}

	protected override void OnDisable() {
		CancelHold();

		base.OnDisable();
	}

	private void CancelHold() {
		if(mHoldRout != null) {
			StopCoroutine(mHoldRout);
			mHoldRout = null;

			onHoldProgress?.Invoke(0f);
			onHold?.Invoke(false);
		}
	}

	IEnumerator DoHold() {
		onHold?.Invoke(true);

		onHoldProgress?.Invoke(0f);

		var isDone = false;
		var curTime = 0f;

		while(!isDone) {
			yield return null;

			if(isDown) {
				curTime += Time.deltaTime;
				isDone = curTime >= holdDelay;
			}
			else {
				curTime -= Time.deltaTime;
				isDone = curTime <= 0f;
			}

			onHoldProgress?.Invoke(Mathf.Clamp01(curTime / holdDelay));
		}

		mHoldRout = null;
				
		onHoldProgress?.Invoke(0f);
		onHold?.Invoke(false);

		if(isDown)
			Click();
	}
}
