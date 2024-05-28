using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

/// <summary>
/// General level game controller, use this as a basis for other levels with dialogs and learning stuff
/// </summary>
public class PlayController : GameModeController<PlayController> {

	protected override void OnInstanceDeinit() {
		//

		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		//
	}

	protected override IEnumerator Start() {
		yield return base.Start();

		//signal for puzzle to be playable

		//wait for all goals to be finished

		//jingle and pop-off

		//move to marching band
	}
}
