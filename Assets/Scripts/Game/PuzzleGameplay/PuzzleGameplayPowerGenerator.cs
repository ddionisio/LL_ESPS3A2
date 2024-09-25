using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameplayPowerGenerator : MonoBehaviour {
    [System.Serializable]
    public struct RotateData {
        public Transform root;
		public M8.RangeFloat rotateRateRange;

        public void Update(float rangeT, float deltaTime) {
            var rot = root.localEulerAngles;

            rot.z += rotateRateRange.Lerp(rangeT) * deltaTime;

            root.localEulerAngles = rot;
        }
	}

    [System.Serializable]
    public struct PowerData {
        public PuzzleGameplayPowerConnect connector;
        public GameObject[] activeGOs;

        public void ApplyActive(bool active) {
            if(connector)
				connector.isPowerActive = active;

            for(int i = 0; i < activeGOs.Length; i++) {
                var go = activeGOs[i];
                if(go)
                    go.SetActive(active);
            }
		}
	}

    public PowerData[] powers;

    public RotateData[] rotators;

    public float powerScale { get; private set; }

    //[0, 1]
    public void ApplyPowerScale(float t) {
        if(powerScale != t) {
			powerScale = t;
            RefreshPower();
		}
    }

	void OnEnable() {
		RefreshPower();
	}

	void Update() {
        if(powerScale > 0f) {
            var dt = Time.deltaTime;

            for(int i = 0; i < rotators.Length; i++) {
                var rot = rotators[i];
                rot.Update(powerScale, dt);
            }
        }
	}

	private void RefreshPower() {
        int activeInd = Mathf.Clamp(Mathf.RoundToInt(powerScale * powers.Length), 0, powers.Length - 1);

        for(int i = 0; i < powers.Length; i++) {
            var power = powers[i];
            power.ApplyActive(i <= activeInd);
        }
    }
}
