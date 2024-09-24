using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Add this component on dropoff
public class PuzzleGameplayPowerInput : MonoBehaviour {
	public GameObject[] activeGOs;

	[Header("Events")]
	public UnityEvent<bool> powerActiveEvent;

	public bool isPowerActive { get { return mIsPowerActive; } }

	private bool mIsPowerActive;

	private PuzzleGameplayPowerInput mPowerInput;

	/// <summary>
	/// Attach to PuzzleDropOff.onDropOff
	/// </summary>
	public void DropOff(PuzzleMechanicPickUp pickup) {
		PuzzleGameplayPowerInput powerInput = pickup ? pickup.GetComponent<PuzzleGameplayPowerInput>() : null;

		if(mPowerInput != powerInput) {
			ClearPowerInput();

			mPowerInput = powerInput;

			if(mPowerInput)
				mPowerInput.powerActiveEvent.AddListener(OnPowerInputActive);
		}

		RefreshPowerInput();
	}

	void OnEnable() {
		RefreshPowerDisplay();
	}

	void OnDisable() {
		ClearPowerInput();

		mIsPowerActive = false;
	}

	void OnPowerInputActive(bool active) {
		RefreshPowerInput();
	}

	private void ClearPowerInput() {
		if(mPowerInput) {
			mPowerInput.powerActiveEvent.RemoveListener(OnPowerInputActive);
			mPowerInput = null;
		}
	}

	private void RefreshPowerInput() {
		var inputPowerActive = mPowerInput ? mPowerInput.isPowerActive : false;

		if(mIsPowerActive != inputPowerActive) {
			mIsPowerActive = inputPowerActive;

			RefreshPowerDisplay();

			powerActiveEvent.Invoke(mIsPowerActive);
		}
	}

	private void RefreshPowerDisplay() {
		for(int i = 0; i < activeGOs.Length; i++) {
			var go = activeGOs[i];
			if(go)
				go.SetActive(mIsPowerActive);
		}
	}
}
