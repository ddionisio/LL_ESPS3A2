using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameplaySector : MonoBehaviour {
	[System.Serializable]
	public class Link {
		public string label;
		public PuzzleMechanicSwitch switchRef; //determines if this link is open if the label matches
		public PuzzleGameplaySector[] sectors; //sectors affected by this link

	}
		
	public Link[] links;

	[Header("Display")]
	public GameObject fillDisplayGO;

	[Header("Animations")]
	public float fillDelay = 0.3f;
	public M8.AnimatorParamBool fillAnimParam;

	public bool isFilled { get; private set; }

	public bool isFilling { get { return mFillRout != null; } }

	private Animator mAnim;
	private Coroutine mFillRout;

	public void ApplyFill(bool fill, int iteration) {
		int nextIter;

		if(isFilled != fill) {
			isFilled = fill;
						
			mFillRout = StartCoroutine(DoFill(fillDelay * iteration));

			nextIter = iteration + 1;
		}
		else {
			mFillRout = StartCoroutine(DoFillWaitNext()); //this is just to 'tag' this as iterated so it can be ignored during recursion

			nextIter = iteration;
		}

		//go through open sectors
		for(int i = 0; i < links.Length; i++) {
			var link = links[i];

			if(!link.switchRef) //fail-safe
				continue;

			if(link.switchRef.indexLabel == link.label) {
				for(int j = 0; j < link.sectors.Length; j++) {
					var sector = link.sectors[j];

					if(!sector) //fail-safe
						continue;

					if(!sector.isFilling)
						sector.ApplyFill(isFilled, nextIter);
				}
			}
		}

		//go through closed sectors
		for(int i = 0; i < links.Length; i++) {
			var link = links[i];

			if(!link.switchRef) //fail-safe
				continue;

			if(link.switchRef.indexLabel != link.label) {
				for(int j = 0; j < link.sectors.Length; j++) {
					var sector = link.sectors[j];

					if(!sector) //fail-safe
						continue;

					if(!sector.isFilling)
						sector.ApplyFill(false, nextIter);
				}
			}
		}
	}

	public void StopFilling() {
		if(mFillRout != null) {
			StopCoroutine(mFillRout);
			mFillRout = null;
		}
	}

	void OnEnable() {
		if(mAnim)
			fillAnimParam.Set(mAnim, isFilled);
	}

	void OnDisable() {
		StopFilling();
	}

	void Awake() {
		mAnim = GetComponent<Animator>();
	}

	IEnumerator DoFill(float waitDelay) {
		if(isFilled) {
			if(fillDisplayGO)
				fillDisplayGO.SetActive(true);
		}

		if(mAnim)
			fillAnimParam.Set(mAnim, isFilled);

		if(waitDelay > 0f)
			yield return new WaitForSeconds(waitDelay);
		else
			yield return null;

		if(!isFilled) {
			if(fillDisplayGO)
				fillDisplayGO.SetActive(false);
		}

		mFillRout = null;
	}

	IEnumerator DoFillWaitNext() {
		yield return null;

		mFillRout = null;
	}
}
