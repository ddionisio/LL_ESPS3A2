using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class VictoryControllerLevel01 : GameModeController<VictoryControllerLevel01> {
	[Header("Complete")]
	public SheepController sheepAries;

	public M8.AnimatorTargetParamTrigger sheepAriesTransform;

	public M8.AnimatorTargetParamTrigger landLightOn;

	public M8.AnimatorTargetParamTrigger landHappy;

	[Header("Dialogs")]
	//public ModalDialogFlowIncremental dlgVictory;
	public ModalDialogFlowIncremental dlgVictoryAwake;
	public ModalDialogFlowIncremental dlgVictoryNext;

	protected override IEnumerator Start() {
		yield return base.Start();

		GameData.instance.MusicPlay(true);

		landLightOn.Set();

		yield return new WaitForSeconds(1.5f);

		sheepAries.PerformAction(SheepController.Action.Wake);

		yield return new WaitForSeconds(2f);

		landHappy.Set();
				
		yield return dlgVictoryAwake.Play();

		sheepAriesTransform.Set();

		yield return new WaitForSeconds(2f);

		yield return dlgVictoryNext.Play();
				
		sheepAries.MoveOffscreen(SheepController.Side.Right);

		while(sheepAries.isBusy)
			yield return null;

		GameData.instance.ProgressNext();
	}

	/*protected override IEnumerator Start() {
		yield return base.Start();

		GameData.instance.MusicPlay(true);

		landLightOn.Set();

		yield return new WaitForSeconds(2f);
				
		sheepAries.PerformAction(SheepController.Action.Wake);

		yield return dlgVictory.Play();
								
		sheepAriesTransform.Set();

		yield return new WaitForSeconds(2f);

		yield return dlgVictoryAwake.Play();

		landHappy.Set();

		sheepAries.MoveOffscreen(SheepController.Side.Right);

		yield return dlgVictoryNext.Play();

		while(sheepAries.isBusy)
			yield return null;

		GameData.instance.ProgressNext();
	}*/
}
