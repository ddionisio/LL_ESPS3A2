using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleGameplayPlunger : MonoBehaviour {
    public PuzzleMechanicSlider slider;

    public float sliderPullMoveDelay;
    public float sliderResetMoveDelay;

    public UnityEvent<float> onAction;

	private Coroutine mRout;

	void OnDisable() {
		if(mRout != null) {
			StopCoroutine(mRout);
			mRout = null;
		}
	}

	void OnDestroy() {
		if(slider) {
			slider.onInputUp.RemoveListener(OnSliderInputUp);
		}
	}

	void Awake() {
        slider.handleMoveDelay = sliderPullMoveDelay;

		slider.onInputUp.AddListener(OnSliderInputUp);
	}

    void OnSliderInputUp() {
		if(mRout != null)
			StopCoroutine(mRout);

		mRout = StartCoroutine(DoPlunge());
    }

	IEnumerator DoPlunge() {
		var val = slider.valueFromHandlePosition;

		slider.handleMoveDelay = sliderResetMoveDelay;
		slider.locked = true;

		slider.valueScalar = 0f;

		var curTime = 0f;
		while(curTime < sliderResetMoveDelay) {
			yield return null;
			curTime += Time.deltaTime;
		}

		slider.handleMoveDelay = sliderPullMoveDelay;
		slider.locked = false;

		mRout = null;

		//Debug.Log("Plunge value: " + val);

		onAction?.Invoke(val);
	}
}
