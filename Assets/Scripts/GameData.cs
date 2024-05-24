using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

[CreateAssetMenu(fileName = "gameData", menuName = "Game/GameData")]
public class GameData : M8.SingletonScriptableObject<GameData> {
    [Header("Scenes")]
    public M8.SceneAssetPath[] levelScenes;
    public M8.SceneAssetPath endScene;

	public bool isProceed { get; private set; }

	public void ProgressReset() {
		//LoLManager.instance.userData.Delete();

		LoLManager.instance.ApplyProgress(0, 0);

		isProceed = false;
	}

	protected override void OnInstanceInit() {
		isProceed = false;
	}
}
