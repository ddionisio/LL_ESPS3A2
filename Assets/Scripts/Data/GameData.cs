using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

[CreateAssetMenu(fileName = "gameData", menuName = "Game/GameData")]
public class GameData : M8.SingletonScriptableObject<GameData> {
	[Header("Signals")]
	public M8.Signal signalPlayBegin;
	public M8.Signal signalPlayEnd;

	public M8.SignalBoolean signalPuzzleInteractable;
	public M8.Signal signalPuzzleComplete;
		
	[Header("Scenes")]
    public M8.SceneAssetPath[] levelScenes;
    public M8.SceneAssetPath endScene;

	public bool isProceed { get; private set; }

	public void ProgressReset() {
		//LoLManager.instance.userData.Delete();

		LoLManager.instance.ApplyProgress(0, 0);

		isProceed = false;
	}

	public void ProgressContinue() {
		isProceed = true;

		var curProgress = LoLManager.instance.curProgress;

		levelScenes[curProgress].Load();
	}

	public void ProgressNext() {
		int curProgress;

		if(!isProceed) { //generate proper progress (when testing)
			var curScene = M8.SceneManager.instance.curScene;

			if(curScene == endScene)
				curProgress = levelScenes.Length - 1;
			else {
				int levelInd = -1;
				for(int i = 0; i < levelScenes.Length; i++) {
					if(levelScenes[i] == curScene) {
						levelInd = i;
						break;
					}
				}

				curProgress = levelInd == -1 ? 0 : levelInd;
			}

			isProceed = true;
		}
		else
			curProgress = LoLManager.instance.curProgress;

		var nextProgress = curProgress + 1;

		LoLManager.instance.ApplyProgress(nextProgress);

		if(nextProgress < levelScenes.Length)
			levelScenes[nextProgress].Load();
		else
			endScene.Load();
	}

	protected override void OnInstanceInit() {
		isProceed = false;

		//compute max progress
		if(LoLManager.isInstantiated)
			LoLManager.instance.progressMax = levelScenes.Length;
	}
}
