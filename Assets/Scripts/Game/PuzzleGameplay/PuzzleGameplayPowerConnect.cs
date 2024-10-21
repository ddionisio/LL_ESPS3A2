using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Add this component on pickup
public class PuzzleGameplayPowerConnect : MonoBehaviour {
    public GameObject[] activeGOs;

	[Header("Events")]
	public UnityEvent<bool> powerActiveEvent;

	public bool isPowerActive {
        get { return mIsPowerActive; }
        set {
            if(mIsPowerActive != value) {
                mIsPowerActive = value;

                ApplyPowerActive();

				powerActiveEvent.Invoke(mIsPowerActive);
			}
        }
    }

    private bool mIsPowerActive;

	void OnEnable() {
		ApplyPowerActive();
	}

	private void ApplyPowerActive() {
        for(int i = 0; i < activeGOs.Length; i++) {
            var go = activeGOs[i];
            if(go)
                go.SetActive(mIsPowerActive);
		}
    }
}
