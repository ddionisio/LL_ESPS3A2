using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControllerLevel04 : PlayControllerBase {

	[Header("Signals")]
	public M8.Signal signalInvokeFillRefresh;

	protected override IEnumerator Intro() {
		yield return null;
	}

	protected override IEnumerator GameBegin() {
		yield return null;

		signalInvokeFillRefresh?.Invoke();
	}

	//protected override void GameUpdate() { }

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
