using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleEntitySpiritFire : MonoBehaviour {

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
	public GameObject[] powerDisplayGOs;

	[Header("Animation")]
	public M8.AnimatorParamTrigger animEnter;
	public M8.AnimatorParamBool animWind;

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

			if(mAnim)
				animWind.Set(mAnim, mWindIsAccum);
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

		if(mAnim)
			animEnter.Set(mAnim);
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

			if(M8.MathUtil.Approx(mWind, mWindAccum, checkApprox)) {
				mWind = mWindAccum;
				mWindLastTime = Time.time;
				mWindVel = 0f;
				mWindIsAccum = false;

				if(mAnim)
					animWind.Set(mAnim, mWindIsAccum);
			}
		}
		else if(Time.time - mWindLastTime >= windDecayWaitDelay) {
			if(!M8.MathUtil.Approx(mWind, 0f, checkApprox)) {
				mWindAccum = mWind = Mathf.SmoothDamp(mWind, 0f, ref mWindVel, windDelay);

				if(M8.MathUtil.Approx(mWind, 0f, checkApprox))
					mWindAccum = mWind = 0f;
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
		var ind = Mathf.RoundToInt(powerDisplayGOs.Length * powerScale);

		for(int i = 0; i < powerDisplayGOs.Length; i++) {
			var go = powerDisplayGOs[i];
			if(go)
				go.SetActive(i <= ind);
		}

		//event
		powerChangedScale.Invoke(powerScale);
	}
}
