using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class PlayControllerLevel04 : PlayControllerBase {
	[Header("Puzzle")]
	public GameObject puzzleGO;
	public GoalController[] puzzleGoals;
	public PuzzleMechanicBase[] puzzleSwitches;

	[Header("Spirit")]
	public M8.AnimatorTargetParamTrigger spiritEnter;
	public M8.AnimatorTargetParamTrigger spiritAction;

	[Header("Instructs")]
	public GameObject switchInstructGO;
	public GameObject goalOtherPointersGO;

	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;
	public ModalDialogFlowIncremental dlgWater;
	public ModalDialogFlowIncremental dlgSwitchInstruct;
	public ModalDialogFlowIncremental dlgGoalFirstReached;
	public ModalDialogFlowIncremental dlgGoalOthersPointer;
	//public ModalDialogFlowIncremental dlgVictory;

	[Header("Signals")]
	public M8.Signal signalInvokeFillRefresh;

	protected override IEnumerator Intro() {
		yield return null;

		//show puzzle
		puzzleGO.SetActive(true);

		yield return new WaitForSeconds(1f);

		yield return dlgIntro.Play();
	}

	protected override IEnumerator GameBegin() {
		spiritEnter.Set();

		yield return new WaitForSeconds(0.5f);

		signalInvokeFillRefresh?.Invoke();

		yield return new WaitForSeconds(0.5f);

		yield return dlgWater.Play();

		puzzleSwitches[0].locked = false;
		puzzleSwitches[1].locked = false;

		switchInstructGO.SetActive(true);

		yield return dlgSwitchInstruct.Play();

		switchInstructGO.SetActive(false);
	}

	//protected override void GameUpdate() { }

	protected override IEnumerator GameEnd() {
		yield return new WaitForSeconds(2f);

		//yield return dlgVictory.Play();
	}

	protected override void OnInstanceDeinit() {


		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		puzzleGO.SetActive(false);

		for(int i = 0; i < puzzleGoals.Length; i++)
			puzzleGoals[i].powerFullyCharged.AddListener(OnGoalFullPowerWater);

		puzzleGoals[0].powerFullyCharged.AddListener(OnGoalFirstFullPower);

		for(int i = 0; i < puzzleSwitches.Length; i++)
			puzzleSwitches[i].locked = true;

		switchInstructGO.SetActive(false);
		goalOtherPointersGO.SetActive(false);
	}

	IEnumerator DoGoalFirstAchieved() {
		yield return dlgGoalFirstReached.Play();

		goalOtherPointersGO.SetActive(true);

		yield return dlgGoalOthersPointer.Play();

		goalOtherPointersGO.SetActive(false);

		for(int i = 2; i < puzzleSwitches.Length; i++)
			puzzleSwitches[i].locked = false;
	}

	void OnGoalFirstFullPower(bool isActive) {
		if(isActive) {
			puzzleGoals[0].powerFullyCharged.RemoveListener(OnGoalFirstFullPower);

			StartCoroutine(DoGoalFirstAchieved());
		}
	}

	void OnGoalFullPowerWater(bool isActive) {
		if(isActive)
			spiritAction.Set();
	}
}
