using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameplaySectorRefresh : MonoBehaviour {
	//ensure this is the parent of all sectors

    public PuzzleGameplaySector rootFillSector; //starting flood iteration, will always be set to 'filled'

	public M8.Signal signalListenSwitch; //listen to any switch update, flood fill beginning with root sector

	[M8.SoundPlaylist]
	public string fillSFX;

	private PuzzleGameplaySector[] mSectors; //use for clearing fills

	private M8.AudioSourceProxy mFillAudio;

	private Coroutine mFillRout;

	public void Refresh() {
		for(int i = 0; i < mSectors.Length; i++) {
			if(mSectors[i])
				mSectors[i].StopFilling();
		}

		if(mFillRout != null)
			StopCoroutine(mFillRout);

		mFillRout = StartCoroutine(DoFill());
	}

	void OnDisable() {
		if(mFillAudio) {
			if(M8.SoundPlaylist.isInstantiated)
				M8.SoundPlaylist.instance.Stop(mFillAudio);
			else
				mFillAudio.Stop();

			mFillAudio = null;
		}

		if(mFillRout != null) {
			StopCoroutine(mFillRout);
			mFillRout = null;
		}
	}

	void OnDestroy() {
		if(signalListenSwitch)
			signalListenSwitch.callback -= Refresh;
	}

	void Awake() {
		if(signalListenSwitch)
			signalListenSwitch.callback += Refresh;

		mSectors = GetComponentsInChildren<PuzzleGameplaySector>(true);
	}

	IEnumerator DoFill() {
		if(rootFillSector)
			rootFillSector.ApplyFill(true, 1);

		if(!(mFillAudio || string.IsNullOrEmpty(fillSFX)))
			mFillAudio = M8.SoundPlaylist.instance.Play(fillSFX, true);

		while(true) {
			yield return null;

			var fillFinishCount = 0;
			for(int i = 0; i < mSectors.Length; i++) {
				var sector = mSectors[i];
				if(!sector.isFilling)
					fillFinishCount++;
			}

			if(fillFinishCount == mSectors.Length)
				break;
		}

		if(mFillAudio) {
			M8.SoundPlaylist.instance.Stop(mFillAudio);
			mFillAudio = null;
		}

		mFillRout = null;
	}
}
