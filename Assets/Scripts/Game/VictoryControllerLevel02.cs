using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class VictoryControllerLevel02 : GameModeController<VictoryControllerLevel02> {
	[Header("Complete")]
	public M8.AnimatorTargetParamTrigger landLightOn;

	public SheepController sheepAries;

	public SheepController[] sheepOthers;

	protected override IEnumerator Start() {
		yield return base.Start();

		landLightOn.Set();

		yield return new WaitForSeconds(2f);

		sheepAries.MoveOffscreen(SheepController.Side.Right);

		//wait for other sheeps to all be offscreen
		int offCount = 0;
		while(offCount < sheepOthers.Length) {
			yield return null;

			offCount = 0;
			for(int i = 0; i < sheepOthers.Length; i++) {
				if(sheepOthers[i].isOffscreen)
					offCount++;
			}
		}
	}
}
