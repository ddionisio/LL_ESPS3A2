using UnityEngine;
using UnityEngine.Events;

public class GoalController : MonoBehaviour {
	public float powerCapacity = 100f;

	[Header("Decay Settings")]
	[SerializeField]
	bool isDecay;
	public float decayWaitDelay;
	public float decayRate;

	[Header("Audio")]
	public AudioSource audioSource;

	public UnityEvent<float> powerChanged;
	public UnityEvent<float> powerChangedNormal;
	public UnityEvent<bool> powerFullyCharged;

	public float power { 
		get { return mPower; }
		set {
			var val = Mathf.Clamp(value, 0f, powerCapacity);
			if(mPower != val) {
				var lastPowerFull = isPowerFull;

				//reset decay if increasing
				if(val > mPower)
					DecayReset();

				mPower = val;

				var _powerNorm = powerNormal;

				if(audioSource)
					audioSource.volume = M8.UserSettingAudio.instance.soundVolume * _powerNorm;

				powerChanged?.Invoke(mPower);
				powerChangedNormal?.Invoke(_powerNorm);

				if(lastPowerFull != isPowerFull)
					powerFullyCharged?.Invoke(isPowerFull);
			}
		}
	}

	public float powerNormal { get { return Mathf.Clamp01(mPower / powerCapacity); } }

	public bool isPowerFull { get { return mPower >= powerCapacity; } }
		
	private float mPower;

	private bool mIsDecayWait;
	private float mDecayCurTime;

	public void DecayReset() {
		mIsDecayWait = true; 
		mDecayCurTime = 0f;
	}

	public void ForceAudioPlay() {
		if(audioSource) {
			audioSource.Stop();
			audioSource.Play();
		}
	}

	public void ForceAudioStop() {
		if(audioSource) {
			audioSource.Stop();
		}
	}

	void OnEnable() {
		mIsDecayWait = mPower > 0f;
		mDecayCurTime = 0f;

		var _powerNorm = powerNormal;

		if(audioSource) {
			audioSource.volume = M8.UserSettingAudio.instance.soundVolume * _powerNorm;
			audioSource.Play();
		}

		powerChanged?.Invoke(mPower);
		powerChangedNormal?.Invoke(_powerNorm);
		powerFullyCharged?.Invoke(isPowerFull);
	}

	void OnDisable() {
		if(audioSource)
			audioSource.Stop();
	}

	void OnDestroy() {
		if(GameData.isInstantiated)
			GameData.instance.signalPuzzleComplete.callback -= OnPuzzleComplete;
	}

	void Awake() {
		GameData.instance.signalPuzzleComplete.callback += OnPuzzleComplete;
	}

	void Update() {
		if(isDecay && mPower > 0f) {
			if(mIsDecayWait) {
				mDecayCurTime += Time.deltaTime;
				if(mDecayCurTime >= decayWaitDelay) {
					mIsDecayWait = false;
					mDecayCurTime = 0f;
				}
			}
			else {
				power -= decayRate * Time.deltaTime;
			}
		}
	}

	void OnPuzzleComplete() {
		isDecay = false;
	}
}
