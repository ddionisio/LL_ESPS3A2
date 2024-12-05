using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

[CreateAssetMenu(fileName = "gameData", menuName = "Game/Data/Main")]
public class GameData : M8.SingletonScriptableObject<GameData> {
	public const string levelIndKey = "lvlInd";

	[System.Serializable]
	public struct LevelData {
		public M8.SceneAssetPath scene;
		public int progressMax;
	}

	[Header("Layers")]
	public LayerMask layerDropOff;

	[Header("Puzzle Mechanic General")]
	public float mechanicRayCastUpdateDelay = 0.3f;
	public float goalIntervalDelay = 2f;
	[M8.TagSelector]
	public string sheepMainTag;

	[M8.MusicPlaylist]
	public string musicMain;

	[Header("Signals")]
	public M8.Signal signalPlayBegin;
	public M8.Signal signalPlayEnd;

	public M8.SignalBoolean signalPuzzleInteractable;
	public M8.Signal signalPuzzleComplete;
		
	[Header("Scenes")]
    public LevelData[] levels;
    public M8.SceneAssetPath endScene;

	public bool isProceed { get; private set; }

	public int progressMax {
		get {
			int max = 0;

			for(int i = 0; i < levels.Length; i++)
				max += levels[i].progressMax;

			return max;
		}
	}

	public int currentLevelIndex {
		get {
			var curScene = M8.SceneManager.instance.curScene;

			var lvlInd = -1;

			for(int i = 0; i < levels.Length; i++) {
				if(levels[i].scene == curScene) {
					lvlInd = i;
					break;
				}
			}

			return lvlInd;
		}
	}

	public void MusicPlay(bool play) {
		if(play) {
			if(!M8.MusicPlaylist.instance.isPlaying)
				M8.MusicPlaylist.instance.Play(musicMain, true, false);
		}
		else
			M8.MusicPlaylist.instance.Stop(false);
	}

	public int GetProgressMax(int levelIndex) {
		int max = 0;

		for(int i = 0; i <= levelIndex; i++)
			max += levels[i].progressMax;

		return max;
	}

	public void ProgressReset() {
		LoLManager.instance.userData.Delete();

		LoLManager.instance.ApplyProgress(0, 0);

		isProceed = false;
	}

	public void ProgressContinue() {
		isProceed = true;

		int lvlInd = LoLManager.instance.userData.GetInt(levelIndKey);

		levels[lvlInd].scene.Load();
	}

	public void ProgressNext() {
		int lvlInd;

		if(!isProceed) { //generate proper progress (when testing)
			var curScene = M8.SceneManager.instance.curScene;

			if(curScene == endScene)
				lvlInd = levels.Length - 1;
			else {
				lvlInd = 0;
				for(int i = 1; i < levels.Length; i++) {
					if(levels[i].scene == curScene) {
						lvlInd = i;
						break;
					}
				}
			}

			isProceed = true;
		}
		else
			lvlInd = LoLManager.instance.userData.GetInt(levelIndKey);

		var lvlIndNext = lvlInd + 1;

		LoLManager.instance.userData.SetInt(levelIndKey, lvlIndNext);

		//ensure progress is correct
		int progressCurMax = GetProgressMax(lvlInd);

		LoLManager.instance.ApplyProgress(progressCurMax);

		if(lvlIndNext < levels.Length)
			levels[lvlIndNext].scene.Load();
		else
			endScene.Load();
	}

	protected override void OnInstanceInit() {
		isProceed = false;

		//compute max progress
		if(LoLManager.isInstantiated)
			LoLManager.instance.progressMax = progressMax;
	}
}
