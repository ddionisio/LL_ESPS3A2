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
	public ModalDialogFlowIncremental dlgVictory;

	protected override IEnumerator Start() {
		yield return base.Start();

		landLightOn.Set();

		yield return new WaitForSeconds(2f);

		sheepAries.PerformAction(SheepController.Action.Wake);

		yield return new WaitForSeconds(2f);

		sheepAriesTransform.Set();

		yield return new WaitForSeconds(2f);
				
		yield return dlgVictory.Play();

		landHappy.Set();

		sheepAries.MoveOffscreen(SheepController.Side.Right);

		while(sheepAries.isBusy)
			yield return null;

		GameData.instance.ProgressNext();
	}
}
