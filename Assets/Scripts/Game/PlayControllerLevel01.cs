using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoLExt;

public class PlayControllerLevel01 : PlayControllerBase {
	[Header("Scene")]
	public Transform landRoot;
	public Transform skyRoot;

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

	[Header("Complete")]
	public SheepController sheepAries;

	public M8.AnimatorTargetParamTrigger landLightOn;

	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;
	public ModalDialogFlowIncremental dlgPlayIntro;
	public ModalDialogFlowIncremental dlgPlayIntroMechanic;
	public ModalDialogFlowIncremental dlgOuterComplete;
	public ModalDialogFlowIncremental dlgInnerComplete;
	public ModalDialogFlowIncremental dlgVictory;

	[Header("Signals")]
	public M8.Signal signalListenPuzzleOuterComplete;
	public M8.Signal signalListenPuzzleInnerComplete;

	protected override IEnumerator Intro() {
		yield return dlgIntro.Play();

		var camTrans = GameCameraTransition.instance;

		camTrans.Transition(skyRoot);

		while(camTrans.isBusy)
			yield return null;

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

		LoLManager.instance.ApplyProgress(LoLManager.instance.curProgress + 1);

		//victory

		var camTrans = GameCameraTransition.instance;

		camTrans.Transition(landRoot);

		while(camTrans.isBusy)
			yield return null;

		landLightOn.Set();

		yield return new WaitForSeconds(2f);

		sheepAries.PerformAction(SheepController.Action.Wake);

		yield return new WaitForSeconds(1f);

		yield return dlgVictory.Play();

		sheepAries.MoveOffscreen(SheepController.Side.Right);

		while(sheepAries.isBusy)
			yield return null;
	}

	protected override void OnInstanceDeinit() {
		signalListenPuzzleOuterComplete.callback -= OnPuzzleOuterComplete;
		signalListenPuzzleInnerComplete.callback -= OnPuzzleInnerComplete;

		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		skyRoot.gameObject.SetActive(false);

		puzzleGO.SetActive(false);
		puzzleInstructionGO.SetActive(false);

		outerInput.locked = true;
		innerInput.locked = true;		

		spiritGO.SetActive(false);
		spiritLightBeamGO.SetActive(false);

		GameCameraTransition.instance.SetCurrentRoot(landRoot);

		signalListenPuzzleOuterComplete.callback += OnPuzzleOuterComplete;
		signalListenPuzzleInnerComplete.callback += OnPuzzleInnerComplete;
	}

	IEnumerator DoPuzzleOuterComplete() {
		puzzleInstructionGO.SetActive(false);

		spiritAction.Set();

		yield return new WaitForSeconds(1f);

		yield return dlgOuterComplete.Play();

		innerInput.locked = false;

		LoLManager.instance.ApplyProgress(LoLManager.instance.curProgress + 1);
	}

	void OnPuzzleOuterComplete() {
		StartCoroutine(DoPuzzleOuterComplete());
	}

	void OnPuzzleInnerComplete() {
		isPuzzleComplete = true;
	}
}
