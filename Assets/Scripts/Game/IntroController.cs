using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class IntroController : GameModeController<IntroController> {
	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;

	protected override IEnumerator Start() {
		yield return base.Start();

		GameData.instance.MusicPlay(true);

		yield return dlgIntro.Play();

		GameData.instance.ProgressNext();
	}
}
