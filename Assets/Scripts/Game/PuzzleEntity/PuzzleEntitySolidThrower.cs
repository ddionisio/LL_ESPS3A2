using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntitySolidThrower : MonoBehaviour {
	
	public GameObject solidRootGO;

	public bool isBusy { get { return mRout != null; } }

	private M8.CacheList<PuzzleEntitySolid> mSolidActives;
	private M8.CacheList<PuzzleEntitySolid> mSolidIdles;

	private Coroutine mRout;

	public void Enter() {
		
	}

	public void Exit() {

	}

	public void Action() {

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

		//show each solid one by one

		//done
		yield return null;

		mRout = null;
	}

	IEnumerator DoExit() {
		//respawn all solids

		//animation exit

		//done
		yield return null;

		mRout = null;
	}

	IEnumerator DoAction() {
		yield return null;

		mRout = null;
	}
}
