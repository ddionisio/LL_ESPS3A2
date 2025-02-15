using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class VictoryControllerLevel01Rev : GameModeController<VictoryControllerLevel01> {
	[Header("Complete")]
	public SheepController sheepAries;

	public M8.AnimatorTargetParamTrigger landLightOn;

	//[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgVictory;
	//public ModalDialogFlowIncremental dlgVictoryAwake;
	//public ModalDialogFlowIncremental dlgVictoryNext;

	protected override IEnumerator Start() {
		yield return base.Start();

		GameData.instance.MusicPlay(true);

		landLightOn.Set();

		yield return new WaitForSeconds(1f);
				
		sheepAries.PerformAction(SheepController.Action.Wake);

		yield return new WaitForSeconds(5f);

		yield return dlgVictory.Play();

		//yield return new WaitForSeconds(2f);

		//while(sheepAries.isBusy)
		//yield return null;

		GameData.instance.ProgressNext();
	}
}
