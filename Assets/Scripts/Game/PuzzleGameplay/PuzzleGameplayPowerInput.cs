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

	private PuzzleDropOff mDropOff;

	private PuzzleGameplayPowerConnect mPowerConnect;
		
	void OnEnable() {
		RefreshPowerDisplay();
	}

	void OnDisable() {
		ClearPowerInput();

		mIsPowerActive = false;
	}

	void OnDestroy() {
		if(mDropOff) {
			mDropOff.onDropOffPickupChanged.RemoveListener(DropOff);
			mDropOff = null;
		}
	}

	void Awake() {
		mDropOff = GetComponent<PuzzleDropOff>();
		if(mDropOff)
			mDropOff.onDropOffPickupChanged.AddListener(DropOff);
	}

	void OnPowerConnectActive(bool active) {
		RefreshPower();
	}

	private void DropOff(PuzzleMechanicPickUp pickup) {
		var powerOutput = pickup ? pickup.GetComponent<PuzzleGameplayPowerConnect>() : null;

		if(mPowerConnect != powerOutput) {
			ClearPowerInput();

			mPowerConnect = powerOutput;

			if(mPowerConnect)
				mPowerConnect.powerActiveEvent.AddListener(OnPowerConnectActive);
		}

		RefreshPower();
	}

	private void ClearPowerInput() {
		if(mPowerConnect) {
			mPowerConnect.powerActiveEvent.RemoveListener(OnPowerConnectActive);
			mPowerConnect = null;
		}
	}

	private void RefreshPower() {
		var inputPowerActive = mPowerConnect ? mPowerConnect.isPowerActive : false;

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
