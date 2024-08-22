using LoLExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControllerLevel02 : PlayControllerBase {
	[Header("Scene")]
	public Transform landRoot;
	public Transform skyRoot;

	[Header("Puzzle")]
	public GameObject puzzleGO;
	public GameObject puzzleDragInstructGO;
	public GameObject puzzleDragInstructSecondGO;
	public GameObject puzzleHandleInstructGO;

	public GameObject[] puzzleLightGOs;

	public GameObject[] puzzlePickupRootGOs;
	public PuzzleDropOff[] puzzleDropOffs;

	public GoalController[] puzzleGoals;
		
	[Header("Spirit")]
	public GameObject spiritGO;
	public M8.AnimatorTargetParamTrigger spiritEnter;
	public M8.AnimatorTargetParamTrigger spiritAction;
	public M8.AnimatorTargetParamTrigger spiritVictory;

	[Header("Complete")]
	public M8.AnimatorTargetParamTrigger landLightOn;

	public SheepController sheepAries;

	public SheepController[] sheepOthers;

	protected override IEnumerator Intro() {
		yield return null;

		//dialog
		yield return new WaitForSeconds(1f);

		//show puzzle
		puzzleGO.SetActive(true);

		yield return new WaitForSeconds(1f);

		//show spirit
		spiritGO.SetActive(true);

		spiritEnter.Set();

		//some more dialog

		yield return new WaitForSeconds(1f);

		//show lights

		spiritAction.Set();

		for(int i = 0; i < puzzleLightGOs.Length; i++)
			puzzleLightGOs[i].SetActive(true);

		yield return new WaitForSeconds(1f);

		//
	}

	protected override IEnumerator GameBegin() {
		yield return null;

		//show first pickup
		puzzleDropOffs[0].active = true;

		puzzlePickupRootGOs[0].SetActive(true);

		yield return new WaitForSeconds(0.5f);

		puzzleDragInstructGO.SetActive(true);

		//dialog
	}

	protected override void GameUpdate() {
	}

	protected override IEnumerator GameEnd() {
		yield return null;

		spiritVictory.Set();

		yield return new WaitForSeconds(2f);

		//more dialog stuff

		//progress
		LoLManager.instance.ApplyProgress(LoLManager.instance.curProgress + 1);

		//victory

		var camTrans = GameCameraTransition.instance;

		camTrans.Transition(landRoot);

		while(camTrans.isBusy)
			yield return null;

		landLightOn.Set();

		yield return new WaitForSeconds(2f);

		sheepAries.MoveOffscreen(SheepController.Side.Right);

		//wait for other sheeps to all be offscreen
		int offCount = 0;
		while(offCount < sheepOthers.Length) {
			yield return null;

			offCount = 0;
			for(int i = 0; i < sheepOthers.Length; i++) {
				if(sheepOthers[i].isOffscreen)
					offCount++;
			}
		}

		//finish
	}

	protected override void OnInstanceDeinit() {
		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		//puzzle
		puzzleGO.SetActive(false);

		puzzleDragInstructGO.SetActive(false);
		puzzleDragInstructSecondGO.SetActive(false);
		puzzleHandleInstructGO.SetActive(false);

		for(int i = 0; i < puzzlePickupRootGOs.Length; i++)
			puzzlePickupRootGOs[i].SetActive(false);

		for(int i = 0; i < puzzleLightGOs.Length; i++)
			puzzleLightGOs[i].SetActive(false);

		for(int i = 0; i < puzzleDropOffs.Length; i++)
			puzzleDropOffs[i].active = false;

		puzzleDropOffs[0].onDropOff.AddListener(OnFirstSlotDropOff);
		puzzleDropOffs[1].onDropOff.AddListener(OnSecondSlotDropOff);

		puzzleGoals[0].powerFullyCharged.AddListener(OnGoalOneFullyCharged);

		//spirit		
		spiritGO.SetActive(false);

		//land
	}

	void OnFirstSlotDropOff(PuzzleMechanicPickUp pickup) {
		//pickup.locked = true;

		puzzleDropOffs[0].onDropOff.RemoveListener(OnFirstSlotDropOff);
		//puzzleDropOffs[0].active = false;

		puzzleDragInstructGO.SetActive(false);

		//show second pickup, enable second drop-off
		puzzleDropOffs[1].active = true;

		puzzlePickupRootGOs[1].SetActive(true);

		puzzleDragInstructSecondGO.SetActive(true);
	}

	void OnSecondSlotDropOff(PuzzleMechanicPickUp pickup) {

		puzzleDropOffs[1].onDropOff.RemoveListener(OnSecondSlotDropOff);

		puzzleDragInstructSecondGO.SetActive(false);

		//show handle instruction
		StartCoroutine(DoHandleInstruction());
	}

	void OnGoalOneFullyCharged(bool charged) {
		if(charged) {
			puzzleGoals[0].powerFullyCharged.RemoveListener(OnGoalOneFullyCharged);

			spiritAction.Set();

			//progress
			LoLManager.instance.ApplyProgress(LoLManager.instance.curProgress + 1);

			//show final pick ups and some dialog
			StartCoroutine(DoGoalOneFullyCharged());
		}
	}

	IEnumerator DoHandleInstruction() {
		yield return null;
				
		puzzleHandleInstructGO.SetActive(true);

		//dialog
		yield return new WaitForSeconds(1f);

		puzzleHandleInstructGO.SetActive(false);
	}

	IEnumerator DoGoalOneFullyCharged() {
		yield return null;

		//dialog

		puzzleDropOffs[2].active = true;
		puzzleDropOffs[3].active = true;

		puzzlePickupRootGOs[2].SetActive(true);

		yield return new WaitForSeconds(1f);

		puzzlePickupRootGOs[3].SetActive(true);
	}
}