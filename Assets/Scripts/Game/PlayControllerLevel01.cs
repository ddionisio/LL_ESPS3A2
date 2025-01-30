using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoLExt;

public class PlayControllerLevel01 : PlayControllerBase {
	[Header("Puzzle")]
	public GameObject puzzleGO;
	public GameObject puzzleInstructionGO;
	public PuzzleMechanicValueBase outerInput;
	public PuzzleMechanicValueBase innerInput;

	[Header("Light")]
	public PuzzleRayCast lightCast;
	public GameObject lightBlockInnerGO;

	[Header("Spirit")]
	public GameObject spiritGO;
	public M8.AnimatorTargetParamTrigger spiritEnter;
	public M8.AnimatorTargetParamTrigger spiritAction;
	public M8.AnimatorTargetParamTrigger spiritVictory;

	public GameObject spiritLightBeamGO;
		
	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;
	public ModalDialogFlowIncremental dlgPlayIntro;
	public ModalDialogFlowIncremental dlgPlayIntroMechanic;
	public ModalDialogFlowIncremental dlgInnerComplete;
	public ModalDialogFlowIncremental dlgOuterComplete;	

	public bool mCheckInnerBlocker;
	public bool mCheckNoBlocker;

	protected override IEnumerator Intro() {
		yield return dlgIntro.Play();

		puzzleGO.SetActive(true);

		yield return new WaitForSeconds(1f);

		spiritGO.SetActive(true);

		spiritEnter.Set();

		yield return new WaitForSeconds(0.5f);

		lightCast.gameObject.SetActive(true);
	}

	protected override IEnumerator GameBegin() {
		yield return dlgPlayIntro.Play();
				
		puzzleInstructionGO.SetActive(true);

		yield return dlgPlayIntroMechanic.Play();

		innerInput.locked = false;
		mCheckInnerBlocker = true;
	}

	protected override void GameUpdate() {
		if(mCheckInnerBlocker) {
			if(lightCast.castHitGO != lightBlockInnerGO) {
				StartCoroutine(DoPuzzleInnerComplete());
				mCheckInnerBlocker = false;
			}
		}
		else if(mCheckNoBlocker) {
			if(lightCast.castHitGO == null) {
				StartCoroutine(DoPuzzleOuterComplete());
				mCheckNoBlocker = false;
			}
		}
	}

	protected override IEnumerator GameEnd() {
		spiritLightBeamGO.SetActive(true);

		spiritVictory.Set();
								
		yield return new WaitForSeconds(3f);
	}

	protected override void OnInstanceDeinit() {

		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		puzzleGO.SetActive(false);
		puzzleInstructionGO.SetActive(false);

		outerInput.locked = true;
		innerInput.locked = true;

		lightCast.gameObject.SetActive(false);

		spiritGO.SetActive(false);
		spiritLightBeamGO.SetActive(false);
	}

	IEnumerator DoPuzzleInnerComplete() {
		innerInput.locked = true;
		innerInput.value = 0.5f;

		puzzleInstructionGO.SetActive(false);

		spiritAction.Set();

		yield return new WaitForSeconds(1f);

		yield return dlgInnerComplete.Play();
				
		outerInput.locked = false;
		mCheckNoBlocker = true;
	}


	IEnumerator DoPuzzleOuterComplete() {
		outerInput.locked = true;
		outerInput.value = 0.5f;

		spiritAction.Set();

		yield return new WaitForSeconds(1f);

		yield return dlgOuterComplete.Play();
				
		isPuzzleComplete = true;
	}
}
