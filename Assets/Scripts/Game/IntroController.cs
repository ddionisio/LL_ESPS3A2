using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class IntroController : GameModeController<IntroController> {
	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;
	public ModalDialogFlowIncremental dlgIntroPost;

	[Header("Animation")]
	public M8.AnimatorTargetParamTrigger introPlay;

	protected override IEnumerator Start() {
		yield return base.Start();

		GameData.instance.MusicPlay(true);

		yield return new WaitForSeconds(1f);

		yield return dlgIntro.Play();

		if(introPlay.target) {
			introPlay.Set();

			yield return new WaitForSeconds(1.5f);
		}

		if(!string.IsNullOrEmpty(dlgIntroPost.prefix))
			yield return dlgIntroPost.Play();

		GameData.instance.ProgressNext();
	}
}
