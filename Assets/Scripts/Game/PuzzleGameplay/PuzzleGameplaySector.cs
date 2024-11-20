using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

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

	[Header("Events")]
	public UnityEvent<bool> fillChanged;

	public bool isFilled { get; private set; }

	public bool isFilling { get { return mFillRout != null; } }

	private Coroutine mFillRout;

	private SpriteShapeRenderer[] mShapeRenders;
	private float[] mShapeRenderAlphas;

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
		if(fillDisplayGO)
			fillDisplayGO.SetActive(isFilled);

		ApplyFill(isFilled ? 1f : 0f);
	}

	void OnDisable() {
		StopFilling();
	}

	void Awake() {
		if(fillDisplayGO)
			mShapeRenders = fillDisplayGO.GetComponentsInChildren<SpriteShapeRenderer>();
		else
			mShapeRenders = new SpriteShapeRenderer[0];

		mShapeRenderAlphas = new float[mShapeRenders.Length];

		for(int i = 0; i < mShapeRenderAlphas.Length; i++) {
			var render = mShapeRenders[i];
			if(render)
				mShapeRenderAlphas[i] = render.color.a;
		}
	}

	IEnumerator DoFill(float waitDelay) {
		if(isFilled) {
			if(fillDisplayGO)
				fillDisplayGO.SetActive(true);
		}

		if(waitDelay > 0f) {
			var curTime = 0f;
			while(curTime < waitDelay) {
				yield return null;

				curTime += Time.deltaTime;

				var t = Mathf.Clamp01(curTime / waitDelay);

				ApplyFill(isFilled ? t : 1f - t);
			}
		}
		else
			yield return null;

		if(!isFilled) {
			if(fillDisplayGO)
				fillDisplayGO.SetActive(false);
		}

		fillChanged.Invoke(isFilled);

		mFillRout = null;
	}

	IEnumerator DoFillWaitNext() {
		yield return null;

		mFillRout = null;
	}

	private void ApplyFill(float t) {
		for(int i = 0; i < mShapeRenders.Length; i++) {
			var render = mShapeRenders[i];
			if(render) {
				var clr = render.color;
				clr.a = Mathf.LerpUnclamped(0f, mShapeRenderAlphas[i], t);

				render.color = clr;
			}
		}
	}
}
