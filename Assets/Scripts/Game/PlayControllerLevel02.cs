using LoLExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControllerLevel02 : PlayControllerBase {
	[Header("Puzzle")]
	public GameObject puzzleGO;	
	public GameObject puzzleDragInstructGO;
	public GameObject puzzleDragInstructSecondGO;
	public GameObject puzzleHandleInstructGO;

	public GameObject[] puzzleLightGOs;

	public GameObject[] puzzlePickupRootGOs;
	public PuzzleDropOff[] puzzleDropOffs;

	public GoalController[] puzzleGoals;

	public PuzzleMechanicRadial puzzleSlotRadialCheck;
		
	[Header("Spirit")]
	public GameObject spiritGO;
	public M8.AnimatorTargetParamTrigger spiritEnter;
	public M8.AnimatorTargetParamTrigger spiritAction;
	public M8.AnimatorTargetParamTrigger spiritVictory;

	[Header("Pointers")]
	public GameObject pointerGoalGO;
	public GameObject pointerGemFirstGO;
	public GameObject pointerGoalNextGO;

	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;
	public ModalDialogFlowIncremental dlgGoalPointer;
	public ModalDialogFlowIncremental dlgGemIntro;
	public ModalDialogFlowIncremental dlgGemInstruct;
	public ModalDialogFlowIncremental dlgGemPlaced;
	public ModalDialogFlowIncremental dlgGemNextIntro;
	public ModalDialogFlowIncremental dlgGemNextPlaced;
	public ModalDialogFlowIncremental dlgGoalFirstReached;
	public ModalDialogFlowIncremental dlgNextTask;

	protected override IEnumerator Intro() {
		yield return null;

		//show puzzle
		puzzleGO.SetActive(true);

		yield return new WaitForSeconds(1f);

		//show spirit
		spiritGO.SetActive(true);

		spiritEnter.Set();

		yield return new WaitForSeconds(1f);

		//dialog
		yield return dlgIntro.Play();

		//goal
		pointerGoalGO.SetActive(true);

		//dialog
		yield return dlgGoalPointer.Play();

		pointerGoalGO.SetActive(false);

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

		pointerGemFirstGO.SetActive(true);

		//dialog
		yield return dlgGemIntro.Play();

		pointerGemFirstGO.SetActive(false);

		puzzleDragInstructGO.SetActive(true);

		//dialog
		yield return dlgGemInstruct.Play();
	}

	protected override void GameUpdate() {
	}

	protected override IEnumerator GameEnd() {
		spiritVictory.Set();

		yield return new WaitForSeconds(2f);

		//more dialog stuff
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

		puzzleDropOffs[0].onDropOffPickupChanged.AddListener(OnFirstSlotDropOff);
		puzzleDropOffs[1].onDropOffPickupChanged.AddListener(OnSecondSlotDropOff);
				
		//spirit		
		spiritGO.SetActive(false);

		//pointers
		pointerGoalGO.SetActive(false);
		pointerGemFirstGO.SetActive(false);
		pointerGoalNextGO.SetActive(false);
	}

	void OnFirstSlotDropOff(PuzzleMechanicPickUp pickup) {
		pickup.locked = true;

		puzzleDropOffs[0].onDropOffPickupChanged.RemoveListener(OnFirstSlotDropOff);
		puzzleDropOffs[0].active = false;

		puzzleDragInstructGO.SetActive(false);
				
		StartCoroutine(DoFirstSlotDropOff());
	}

	void OnSecondSlotDropOff(PuzzleMechanicPickUp pickup) {
		pickup.locked = true;

		puzzleDropOffs[1].onDropOffPickupChanged.RemoveListener(OnSecondSlotDropOff);

		puzzleDragInstructSecondGO.SetActive(false);

		//show handle instruction
		StartCoroutine(DoSecondSlotDropOff());
	}

	void OnGoalOneFullyCharged(bool charged) {
		if(charged) {
			puzzleGoals[0].powerFullyCharged.RemoveListener(OnGoalOneFullyCharged);

			spiritAction.Set();

			//show final pick ups and some dialog
			StartCoroutine(DoGoalOneFullyCharged());
		}
	}

	IEnumerator DoFirstSlotDropOff() {

		yield return dlgGemPlaced.Play();

		//show second pickup, enable second drop-off
		puzzleDropOffs[1].active = true;

		puzzlePickupRootGOs[1].SetActive(true);

		puzzleDragInstructSecondGO.SetActive(true);

		yield return dlgGemNextIntro.Play();

		//pickup.locked = false;
	}

	IEnumerator DoSecondSlotDropOff() {
		puzzleHandleInstructGO.SetActive(true);

		yield return dlgGemNextPlaced.Play();

		puzzleHandleInstructGO.SetActive(false);

		if(puzzleGoals[0].isPowerFull)
			OnGoalOneFullyCharged(true);
		else
			puzzleGoals[0].powerFullyCharged.AddListener(OnGoalOneFullyCharged);
	}

	IEnumerator DoGoalOneFullyCharged() {
		//re-align slot and lock it
		puzzleSlotRadialCheck.locked = true;
		puzzleSlotRadialCheck.value = 1.803404f;
		puzzleDropOffs[1].active = false;

		//dialog
		yield return dlgGoalFirstReached.Play();

		puzzleDropOffs[2].active = true;				
		puzzleDropOffs[3].active = true;

		puzzlePickupRootGOs[2].SetActive(true);

		yield return new WaitForSeconds(0.5f);

		puzzlePickupRootGOs[3].SetActive(true);

		pointerGoalNextGO.SetActive(true);

		//dialog
		yield return dlgNextTask.Play();

		pointerGoalNextGO.SetActive(false);
	}
}