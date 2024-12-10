using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class StartController : GameModeController<StartController> {
	[Header("Display")]
	public GameObject loadingGO;
	public GameObject readyGO;
	public GameObject startGO;
	public float startActiveDelay;
	public GameObject continueGO;

	private bool mIsProceed;

	public void Continue() {
		if(mIsProceed) return;


		StartCoroutine(DoProceed(true));
	}

	public void NewGame() {
		if(mIsProceed) return;

		StartCoroutine(DoProceed(false));
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		if(loadingGO) loadingGO.SetActive(true);
		if(readyGO) readyGO.SetActive(false);
		if(startGO) startGO.SetActive(false);
	}

	protected override IEnumerator Start() {
		yield return base.Start();

		GameData.instance.MusicPlay(true);

		yield return new WaitForSeconds(0.3f);

		if(loadingGO) loadingGO.SetActive(false);

		if(readyGO) readyGO.SetActive(true);

		yield return new WaitForSeconds(startActiveDelay);

		if(startGO) startGO.SetActive(true);

		var lolMgr = LoLManager.instance;

		if(continueGO) continueGO.SetActive(lolMgr.curProgress > 0 || GameData.instance.savedLevelIndex > 0);
	}

	IEnumerator DoProceed(bool isContinue) {
		mIsProceed = true;

		yield return null;

		if(isContinue)
			GameData.instance.ProgressContinue();
		else {
			GameData.instance.ProgressReset();

			GameData.instance.ProgressContinue();
		}
	}
}
