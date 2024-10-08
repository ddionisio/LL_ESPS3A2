using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameplaySectorRefresh : MonoBehaviour {
	//ensure this is the parent of all sectors

    public PuzzleGameplaySector rootFillSector; //starting flood iteration, will always be set to 'filled'

	public M8.Signal signalListenSwitch; //listen to any switch update, flood fill beginning with root sector

	private PuzzleGameplaySector[] mSectors; //use for clearing fills

	public void Refresh() {
		for(int i = 0; i < mSectors.Length; i++) {
			if(mSectors[i])
				mSectors[i].StopFilling();
		}

		if(rootFillSector)
			rootFillSector.ApplyFill(true, 1);
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
}
