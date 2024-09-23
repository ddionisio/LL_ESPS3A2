using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntitySpiritFire : MonoBehaviour {

	[Header("Power")]
	public M8.RangeFloat powerRange;

	[Header("Fuel")]
	public float fuelAmount; //per solids
	public M8.RangeFloat fuelBurnRateRange; //determined by wind scalar value [0, 1]

	[Header("Wind Source")]
	public float windCapacity;
	public float windRate; //when accumulating wind
	public float windDecayWaitDelay;
	public float windDecayRate;

	public float power { get; private set; }

	public float wind { get; private set; }

	public float fuel { get; private set; }

	private const int solidFuelCapacity = 8;
	private M8.CacheList<PuzzleEntitySolid> mSolidFuels = new M8.CacheList<PuzzleEntitySolid>(solidFuelCapacity);

	private Coroutine mWindRout;
	private Coroutine mFuelRout;

	public void WindBlow() {
		if(mWindRout != null)
			StopCoroutine(mWindRout);

		mWindRout = StartCoroutine(DoWind());
	}

	void OnTriggerEnter2D(Collider2D collision) {
		
	}

	void OnEnable() {
		power = powerRange.min;
		wind = 0f;
		fuel = 0f;
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

	IEnumerator DoWind() {
		yield return null;

		mWindRout = null;
	}

	IEnumerator DoFuel() {
		yield return null;

		mFuelRout = null;
	}
}
