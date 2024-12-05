using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class EndController : GameModeController<EndController> {
	[System.Serializable]
	public class SheepMove {
		public SheepController sheep;
		public Transform wp;

		public bool isBusy { get { return sheep.isBusy; } }

		public void Move() {
			sheep.MoveTo(wp.position.x, SheepController.Action.Victory);
		}
	}

	public SheepMove[] sheepMoves;

	public M8.AnimatorTargetParamTrigger endPlay;

	public GameObject endThanksGO;

	[M8.MusicPlaylist]
	public string music;
	
	protected override IEnumerator Start() {
		yield return base.Start();

		if(!M8.MusicPlaylist.instance.isPlaying && !string.IsNullOrEmpty(music))
			M8.MusicPlaylist.instance.Play(music, false, true);

		var sheepMoveWait = new WaitForSeconds(1f);

		for(int i = 0; i < sheepMoves.Length; i++) {
			yield return sheepMoveWait;

			sheepMoves[i].Move();
		}

		yield return new WaitForSeconds(3f);

		endPlay.Set();

		yield return new WaitForSeconds(2f);

		endThanksGO.SetActive(true);

		yield return new WaitForSeconds(8f);

		LoLManager.instance.Complete();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		endThanksGO.SetActive(false);
	}
}
