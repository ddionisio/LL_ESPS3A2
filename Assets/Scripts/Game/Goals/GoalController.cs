using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoalController : MonoBehaviour {
	public float powerMax = 100f;
	public float powerPerSecond = 100f;

	public UnityEvent<float> powerChanged;
	public UnityEvent<float> powerChangedNormal;
	public UnityEvent<bool> powerFullyCharged;

	public float power { get; private set; }

	public float powerNormal { get { return Mathf.Clamp01(power / powerMax); } }

	public bool isPowerFull { get { return power >= powerMax; } }
		
	private float mToPower;

	public void AddPower(float amt) {
		mToPower += amt;
	}

	void OnEnable() {
		powerChanged?.Invoke(power);
		powerChangedNormal?.Invoke(powerNormal);
	}

	void Update() {
		if(power != mToPower) {
			var lastPowerFull = isPowerFull;

			if(power < mToPower) {
				power += powerPerSecond * Time.deltaTime;
				if(power > mToPower)
					power = mToPower;
			}
			else {
				power -= powerPerSecond * Time.deltaTime;
				if(power < mToPower)
					power = mToPower;
			}

			powerChanged?.Invoke(power);
			powerChangedNormal?.Invoke(powerNormal);

			if(lastPowerFull != isPowerFull)
				powerFullyCharged?.Invoke(isPowerFull);
		}
	}
}
