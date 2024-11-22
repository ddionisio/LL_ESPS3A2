using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoLExt;

public class PlayControllerLevel01 : PlayControllerBase {
	[Header("Puzzle")]
	public GameObject puzzleGO;
	public GameObject puzzleInstructionGO;
	public PuzzleMechanicBase outerInput;
	public PuzzleMechanicBase innerInput;	

	[Header("Spirit")]
	public GameObject spiritGO;
	public M8.AnimatorTargetParamTrigger spiritEnter;
	public M8.AnimatorTargetParamTrigger spiritAction;
	public M8.AnimatorTargetParamTrigger spiritVictory;

	public GameObject spiritLightBeamGO;
		
	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgPlayIntro;
	public ModalDialogFlowIncremental dlgPlayIntroMechanic;
	public ModalDialogFlowIncremental dlgOuterComplete;
	public ModalDialogFlowIncremental dlgInnerComplete;

	[Header("Signals")]
	public M8.Signal signalListenPuzzleOuterComplete;
	public M8.Signal signalListenPuzzleInnerComplete;

	protected override IEnumerator Intro() {
		puzzleGO.SetActive(true);

		yield return new WaitForSeconds(1f);

		spiritGO.SetActive(true);

		spiritEnter.Set();
	}

	protected override IEnumerator GameBegin() {
		yield return dlgPlayIntro.Play();
				
		puzzleInstructionGO.SetActive(true);

		yield return dlgPlayIntroMechanic.Play();

		outerInput.locked = false;
	}

	protected override void GameUpdate() {
	}

	protected override IEnumerator GameEnd() {
		spiritAction.Set();

		yield return new WaitForSeconds(1f);

		yield return dlgInnerComplete.Play();

		spiritVictory.Set();

		spiritLightBeamGO.SetActive(true);
				
		yield return new WaitForSeconds(2f);
	}

	protected override void OnInstanceDeinit() {
		signalListenPuzzleOuterComplete.callback -= OnPuzzleOuterComplete;
		signalListenPuzzleInnerComplete.callback -= OnPuzzleInnerComplete;

		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		puzzleGO.SetActive(false);
		puzzleInstructionGO.SetActive(false);

		outerInput.locked = true;
		innerInput.locked = true;		

		spiritGO.SetActive(false);
		spiritLightBeamGO.SetActive(false);

		signalListenPuzzleOuterComplete.callback += OnPuzzleOuterComplete;
		signalListenPuzzleInnerComplete.callback += OnPuzzleInnerComplete;
	}

	IEnumerator DoPuzzleOuterComplete() {
		puzzleInstructionGO.SetActive(false);

		spiritAction.Set();

		yield return new WaitForSeconds(1f);

		yield return dlgOuterComplete.Play();

		innerInput.locked = false;
	}

	void OnPuzzleOuterComplete() {
		StartCoroutine(DoPuzzleOuterComplete());
	}

	void OnPuzzleInnerComplete() {
		isPuzzleComplete = true;
	}
}
