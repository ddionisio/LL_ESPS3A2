using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleEntitySpiritFire : MonoBehaviour {

	[Header("Power")]
	public float powerCapacity;

	[Header("Fuel")]
	public float fuelAmount; //per solids
	public M8.RangeFloat fuelBurnRateRange; //determined by wind scalar value [0, 1]

	[Header("Wind Source")]
	public float windCapacity;
	public float windRate; //when accumulating wind
	public float windDecayWaitDelay;
	public float windDecayRate;

	[Header("Display")]
	public GameObject[] powerDisplayGOs;

	[Header("Animation")]
	public M8.AnimatorParamTrigger animEnterTrigger;
	public M8.AnimatorParamTrigger animWindTrigger;

	[Header("Events")]
	public UnityEvent<float> powerChangedScale;

	public float power { get; private set; }

	public float powerScale { get { return power / powerCapacity; } }

	public float wind { 
		get { return mWind; }
		set {
			var val = Mathf.Clamp(value, 0f, windCapacity);
			if(mWind != val) {
				mWind = val;

				RefreshPower();
			}
		}
	}

	public float windScale {
		get { return mWind / windCapacity; }
	}

	public float fuel { 
		get { return mFuel; }
		set {
			var val = Mathf.Clamp(value, 0f, fuelAmount);
			if(mFuel != val) {
				mFuel = val;

				RefreshPower();
			}
		}
	}

	private const int solidFuelCapacity = 8;
	private M8.CacheList<PuzzleEntitySolid> mSolidFuels = new M8.CacheList<PuzzleEntitySolid>(solidFuelCapacity);

	private Animator mAnim;

	private float mWind;
	private float mFuel;

	private Coroutine mWindRout;
	private Coroutine mFuelRout;
		
	public void WindBlow() {
		if(mWindRout != null)
			StopCoroutine(mWindRout);

		mWindRout = StartCoroutine(DoWind());
	}

	void OnTriggerEnter2D(Collider2D collision) {
		var solid = collision.GetComponent<PuzzleEntitySolid>();
		if(solid) {
			mSolidFuels.Add(solid);

			if(mFuelRout == null)
				mFuelRout = StartCoroutine(DoFuel());
		}
	}

	void OnEnable() {
		mWind = 0f;
		mFuel = 0f;

		RefreshPower();

		if(mAnim)
			animEnterTrigger.Set(mAnim);
	}

	void OnDisable() {
		if(mWindRout != null) {
			StopCoroutine(mWindRout);
			mWindRout = null;
		}

		if(mFuelRout != null) {
			StopCoroutine(mFuelRout);
			mFuelRout = null;
		}

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

	IEnumerator DoWind() {
		if(mAnim)
			animWindTrigger.Set(mAnim);

		while(wind < windCapacity) {
			yield return null;

			wind += windRate * Time.deltaTime;
		}

		//wait a bit
		var curTime = 0f;
		while(curTime < windDecayWaitDelay) {
			yield return null;
			curTime += Time.deltaTime;
		}

		//decay
		while(wind > 0f) {
			yield return null;

			wind -= windDecayRate * Time.deltaTime;
		}

		mWindRout = null;
	}

	IEnumerator DoFuel() {
		while(mSolidFuels.Count > 0) {
			fuel = fuelAmount;

			while(fuel > 0f) {
				yield return null;

				fuel -= fuelBurnRateRange.Lerp(windScale) * Time.deltaTime;

				//TODO: change to the solid?
			}

			var solid = mSolidFuels[0];

			solid.Respawn();

			mSolidFuels.RemoveAt(0);
		}

		mFuelRout = null;
	}

	private void RefreshPower() {
		power = fuel > 0f ? powerCapacity * windScale : 0f; //update power

		//update visual
		var ind = Mathf.FloorToInt(powerDisplayGOs.Length * powerScale);

		for(int i = 0; i < powerDisplayGOs.Length; i++) {
			var go = powerDisplayGOs[i];
			if(go)
				go.SetActive(i <= ind);
		}

		//event
		powerChangedScale.Invoke(powerScale);
	}
}
