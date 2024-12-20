using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntitySolidThrower : MonoBehaviour {
	
	public GameObject solidRootGO;

	[Header("Animation")]
	public M8.AnimatorParamTrigger animEnterTrigger;
	public float animEnterDelay;
	public M8.AnimatorParamTrigger animActionTrigger;
	public float animActionDelay;

	public bool isBusy { get { return mRout != null; } }

	private M8.CacheList<PuzzleEntitySolid> mSolidActives;
	private M8.CacheList<PuzzleEntitySolid> mSolidIdles;

	private Animator mAnim;

	private Coroutine mRout;

	public void Enter() {
		StopRout();

		mRout = StartCoroutine(DoEnter());
	}

	public void Action() {
		if(isBusy)
			return;

		mRout = StartCoroutine(DoAction());
	}

	void OnEnable() {
		Enter();
	}

	void OnDisable() {
		StopRout();
	}

	void OnDestroy() {
		
	}

	void Awake() {
		var solids = solidRootGO.GetComponentsInChildren<PuzzleEntitySolid>(true);

		mSolidActives = new M8.CacheList<PuzzleEntitySolid>(solids.Length);
		mSolidIdles = new M8.CacheList<PuzzleEntitySolid>(solids.Length);

		for(int i = 0; i < solids.Length; i++) {
			var solid = solids[i];

			solid.onSpawnComplete += OnSolidSpawned;
			solid.active = false;

			mSolidIdles.Add(solid);
		}

		mAnim = GetComponent<Animator>();
	}

	void OnSolidSpawned(PuzzleEntitySolid solid) {
		//move from actives to idles
		for(int i = 0; i < mSolidActives.Count; i++) {
			if(solid == mSolidActives[i]) {
				mSolidActives.RemoveAt(i);
				mSolidIdles.Add(solid);
				break;
			}
		}
	}

	IEnumerator DoEnter() {
		//animation enter
		if(mAnim)
			animEnterTrigger.Set(mAnim);

		yield return new WaitForSeconds(animEnterDelay);

		//show solids
		for(int i = 0; i < mSolidIdles.Count; i++) {
			var solid = mSolidIdles[i];
			if(solid)
				solid.active = true;
		}

		mRout = null;
	}

	IEnumerator DoAction() {
		//animation act
		if(mAnim)
			animActionTrigger.Set(mAnim);

		yield return new WaitForSeconds(animActionDelay);

		//launch an idle solid
		if(mSolidIdles.Count > 0) {
			var solid = mSolidIdles.Remove();

			solid.Action(1f);

			mSolidActives.Add(solid);
		}

		mRout = null;
	}

	private void StopRout() {
		if(mRout != null) {
			StopCoroutine(mRout);
			mRout = null;
		}
	}
}
