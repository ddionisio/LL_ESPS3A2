using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

/// <summary>
/// General level game controller, use this as a basis for other levels with dialogs and learning stuff
/// </summary>
public abstract class PlayControllerBase : GameModeController<PlayControllerBase> {

	public bool isPuzzleComplete { get; protected set; }

	protected virtual IEnumerator Intro() { yield return null; }

	protected virtual IEnumerator GameBegin() { yield return null; }

	protected virtual void GameUpdate() { }

	protected virtual IEnumerator GameEnd() { yield return null; }

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
		yield return Intro();

		//signal for puzzle to be playable
		gameDat.signalPlayBegin.Invoke();

		gameDat.signalPuzzleInteractable.Invoke(true);

		//game start
		yield return GameBegin();

		//wait for all goals to be finished
		while(!isPuzzleComplete) {
			GameUpdate();
			yield return null;
		}

		gameDat.signalPuzzleInteractable.Invoke(false);

		gameDat.signalPlayEnd.Invoke();

		//end
		yield return GameEnd();

		//enter next level
		gameDat.ProgressNext();
	}

	void OnSignalPuzzleComplete() {
		isPuzzleComplete = true;

		//Debug.Log("Complete");
	}
}
