using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleEntitySpiritFire : MonoBehaviour {

	[System.Serializable]
	public struct PowerDisplayData {
		public GameObject gameObject;
		public bool exclusive;

		public void SetActive(bool active) {
			if(gameObject)
				gameObject.SetActive(active);
		}
	}

	[Header("Power")]
	public float powerCapacity = 100f;
	public float powerFuelMinimum = 25f; //if there is fuel to burn
	public float powerDelay = 0.3f;


	[Header("Fuel")]
	public float fuelAmount; //per solids
	public M8.RangeFloat fuelBurnRateRange; //determined by wind scalar value [0, 1]

	[Header("Wind Source")]
	public float windCapacity;
	public float windDelay = 2f;
	public float windDecayWaitDelay = 8f;
	public float windDecayDelay = 5f;

	[Header("Display")]
	public PowerDisplayData[] powerDisplays;

	public ParticleSystem windFX;
	public M8.RangeFloat windFXEmissionRange;

	[Header("Events")]
	public UnityEvent<float> powerChangedScale;

	public float power { get; private set; }

	public float powerScale { get { return power / powerCapacity; } }

	public float wind { 
		get { return mWindAccum; }

		set {
			var val = Mathf.Clamp(value, 0f, windCapacity);
			if(mWindAccum != val)
				mWindAccum = val;
						
			mWindIsAccum = true;

			ApplyWindDisplay();
		}
	}

	public float windScale {
		get { return mWind / windCapacity; }
	}

	public float fuel {
		get { return mFuel; }
	}

	private const int solidFuelCapacity = 8;
	private M8.CacheList<PuzzleEntitySolid> mSolidFuels = new M8.CacheList<PuzzleEntitySolid>(solidFuelCapacity);

	private Animator mAnim;

	private float mPowerAccum; //power target value
	private float mPowerVel;

	private bool mWindIsAccum;

	private float mWind;
	private float mWindAccum;
	private float mWindVel;

	private float mWindLastTime;

	private float mFuel;

	public void WindAccumulate(float amt) {
		wind += amt;
	}

	void OnTriggerEnter2D(Collider2D collision) {
		var solid = collision.GetComponent<PuzzleEntitySolid>();
		if(solid) {
			if(mSolidFuels.IsFull)
				solid.Respawn(); //fail-safe, don't add any more fuel if full
			else {
				solid.SetSpecialActive(true);

				mSolidFuels.Add(solid);

				if(mSolidFuels.Count == 1)
					mFuel = fuelAmount;
			}
		}
	}

	void OnEnable() {
		mWind = 0f;
		mFuel = 0f;

		mPowerVel = 0f;

		RefreshPower();
	}

	void OnDisable() {
		//clear out solid fuels
		for(int i = 0; i < mSolidFuels.Count; i++) {
			var solid = mSolidFuels[i];
			if(solid && solid.state == PuzzleEntityState.Action)
				solid.Respawn();
		}

		mSolidFuels.Clear();
	}

	void Awake() {
		mAnim = GetComponent<Animator>();
	}

	private const float checkApprox = 0.001f;

	void Update() {
		if(mWindIsAccum) {
			mWind = Mathf.SmoothDamp(mWind, mWindAccum, ref mWindVel, windDelay);

			ApplyWindEmissionDisplay();

			if(M8.MathUtil.Approx(mWind, mWindAccum, checkApprox)) {
				mWind = mWindAccum;
				mWindLastTime = Time.time;
				mWindVel = 0f;
				mWindIsAccum = false;
			}
		}
		else if(Time.time - mWindLastTime >= windDecayWaitDelay) {
			if(!M8.MathUtil.Approx(mWind, 0f, checkApprox)) {
				mWindAccum = mWind = Mathf.SmoothDamp(mWind, 0f, ref mWindVel, windDelay);

				ApplyWindEmissionDisplay();

				if(M8.MathUtil.Approx(mWind, 0f, checkApprox)) {
					mWindAccum = mWind = 0f;

					ApplyWindDisplay();
				}
			}
		}

		if(mFuel > 0f) {
			mFuel -= fuelBurnRateRange.Lerp(windScale) * Time.deltaTime;

			if(mFuel <= 0f && mSolidFuels.Count > 0) {
				var solid = mSolidFuels[0];
				solid.Respawn();
				mSolidFuels.RemoveAt(0);

				if(mSolidFuels.Count > 0)
					mFuel = fuelAmount;
			}

			mPowerAccum = powerFuelMinimum;

			if(mWind > 0f)
				mPowerAccum += (powerCapacity - powerFuelMinimum) * windScale;
		}
		else
			mPowerAccum = 0f;

		if(!M8.MathUtil.Approx(power, mPowerAccum, checkApprox)) {
			power = Mathf.SmoothDamp(power, mPowerAccum, ref mPowerVel, powerDelay);

			if(M8.MathUtil.Approx(power, mPowerAccum, checkApprox)) {
				power = mPowerAccum;
				mPowerVel = 0f;
			}	

			RefreshPower();
		}
	}

	private void RefreshPower() {
		//update visual
		var ind = Mathf.RoundToInt(powerDisplays.Length * powerScale);

		for(int i = 0; i < powerDisplays.Length; i++) {
			var display = powerDisplays[i];
			if(i == ind) {
				display.SetActive(true);
			}
			else
				display.SetActive(!display.exclusive && i < ind);
		}

		//event
		powerChangedScale.Invoke(powerScale);
	}

	private void ApplyWindDisplay() {
		bool isWindActive = mWindIsAccum || mWind > 0f;

		if(windFX) {
			var windFXDat = windFX.main;			

			if(isWindActive) {
				if(!windFX.isPlaying)
					windFX.Play();

				windFXDat.loop = true;
			}
			else {
				windFXDat.loop = false;
			}
		}
	}

	private void ApplyWindEmissionDisplay() {
		if(windFX) {
			var windFXEmission = windFX.emission;

			windFXEmission.rateOverTime = windFXEmissionRange.Lerp(windScale);
		}
	}
}
