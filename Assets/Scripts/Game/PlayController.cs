using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

/// <summary>
/// General level game controller, use this as a basis for other levels with dialogs and learning stuff
/// </summary>
public class PlayController : GameModeController<PlayController> {

	public bool isPuzzleComplete { get; private set; }

	protected override void OnInstanceDeinit() {
		if(GameData.isInstantiated) {
			var gameDat = GameData.instance;

			gameDat.signalPuzzleComplete.callback -= OnSignalPuzzleComplete;
		}

		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		var gameDat = GameData.instance;

		//setup signals
		gameDat.signalPuzzleComplete.callback += OnSignalPuzzleComplete;
	}

	protected override IEnumerator Start() {
		yield return base.Start();

		var gameDat = GameData.instance;

		//intro stuff

		//signal for puzzle to be playable
		gameDat.signalPlayBegin.Invoke();

		gameDat.signalPuzzleInteractable.Invoke(true);

		//wait for all goals to be finished
		while(!isPuzzleComplete)
			yield return null;

		gameDat.signalPuzzleInteractable.Invoke(false);

		//jingle and pop-off

		gameDat.signalPlayEnd.Invoke();

		//move to marching band
	}

	void OnSignalPuzzleComplete() {
		isPuzzleComplete = true;
	}
}
