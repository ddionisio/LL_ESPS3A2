using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class VictoryControllerLevel03 : GameModeController<VictoryControllerLevel02> {
	[Header("Complete")]
	public M8.AnimatorTargetParamTrigger landLightOn;

	public SheepController sheepAries;
	public SheepController[] sheepFollows;

	public GameObject instrumentFXGO;
	public AudioSource[] instrumentAudios;

	protected override void OnInstanceDeinit() {
		for(int i = 0; i < instrumentAudios.Length; i++) {
			var sfx = instrumentAudios[i];
			if(sfx && sfx.clip)
				sfx.clip.UnloadAudioData();
		}

		base.OnInstanceDeinit();
	}

	protected override IEnumerator Start() {
		yield return base.Start();

		sheepAries.MoveOffscreen(SheepController.Side.Right);

		yield return new WaitForSeconds(2f);

		landLightOn.Set();

		if(instrumentFXGO)
			instrumentFXGO.SetActive(true);

		for(int i = 0; i < instrumentAudios.Length; i++) {
			var sfx = instrumentAudios[i];
			if(sfx) {
				sfx.volume = M8.UserSettingAudio.instance.soundVolume;
				sfx.Play();
			}
		}

		//wait for other sheeps to all be offscreen
		int offCount = 0;
		while(offCount < sheepFollows.Length) {
			yield return null;

			offCount = 0;

			for(int i = 0; i < sheepFollows.Length; i++) {
				if(sheepFollows[i].isOffscreen)
					offCount++;
			}
		}

		//enter next level
		GameData.instance.ProgressNext();
	}
}
