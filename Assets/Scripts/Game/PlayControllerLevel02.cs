using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControllerLevel02 : PlayControllerBase {
	[Header("Scene")]
	public Transform landRoot;
	public Transform skyRoot;

	[Header("Complete")]
	public SheepController sheepAries;

	public SheepController[] sheepOthers;

	protected override IEnumerator Intro() {
		yield return null;
	}

	protected override IEnumerator GameBegin() {
		yield return null;
	}

	protected override void GameUpdate() {
	}

	protected override IEnumerator GameEnd() {
		yield return null;
	}

	protected override void OnInstanceDeinit() {
		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();
	}
}