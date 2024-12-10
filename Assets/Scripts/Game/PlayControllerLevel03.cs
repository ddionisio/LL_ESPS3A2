using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class PlayControllerLevel03 : PlayControllerBase {
	[Header("Puzzle")]
	public GameObject puzzleGO;
	public GameObject[] puzzleRockGOs;
	public PuzzleMechanicBase puzzlePlunger;
	public PuzzleMechanicRadial puzzleRotator;
	public PuzzleMechanicBase puzzleSlider;
	public M8.TriggerEnterExitDelayEvent2D puzzlePlatformSpring;

	[Header("Instructs")]
	public GameObject instructPlungerGO;
	public GameObject instructPushGO;
	public GameObject instructGoalsGO;
	public GameObject instructRotateGO;
	public GameObject instructSliderGO;

	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;
	public ModalDialogFlowIncremental dlgPlungerInstruct;
	public ModalDialogFlowIncremental dlgPushInstruct;
	public ModalDialogFlowIncremental dlgTaskComplete;
	public ModalDialogFlowIncremental dlgGoalPointer;
	public ModalDialogFlowIncremental dlgRotatePointer;
	public ModalDialogFlowIncremental dlgSliderPointer;
	public ModalDialogFlowIncremental dlgGameStart;

	protected override IEnumerator Intro() {
		yield return null;

		//show puzzle
		puzzleGO.SetActive(true);

		yield return new WaitForSeconds(1f);

		yield return dlgIntro.Play();
	}

	protected override IEnumerator GameBegin() {
		var wait = new WaitForSeconds(0.5f);

		for(int i = 0; i < puzzleRockGOs.Length; i++) {			
			puzzleRockGOs[i].SetActive(true);

			yield return wait;
		}
				
		instructPlungerGO.SetActive(true);

		yield return dlgPlungerInstruct.Play();

		instructPushGO.SetActive(true);

		yield return dlgPushInstruct.Play();

		puzzlePlunger.locked = false;

		puzzlePlatformSpring.exitCallback.AddListener(OnPlatformPushed);
	}

	//protected override void GameUpdate() { }

	protected override IEnumerator GameEnd() {
		yield return new WaitForSeconds(5f);

		//more dialog?
	}

	protected override void OnInstanceDeinit() {
		

		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		puzzleGO.SetActive(false);

		for(int i = 0; i < puzzleRockGOs.Length; i++)
			puzzleRockGOs[i].SetActive(false);

		puzzlePlunger.locked = true;
		puzzleRotator.locked = true;
		puzzleSlider.locked = true;

		instructPlungerGO.SetActive(false);
		instructPushGO.SetActive(false);
		instructGoalsGO.SetActive(false);
		instructRotateGO.SetActive(false);
		instructSliderGO.SetActive(false);
	}

	IEnumerator DoPlatformPushed() {
		puzzlePlunger.locked = true;

		yield return new WaitForSeconds(0.5f);

		//goals stuff
		yield return dlgTaskComplete.Play();

		instructGoalsGO.SetActive(true);

		yield return dlgGoalPointer.Play();

		instructGoalsGO.SetActive(false);

		var wait = new WaitForSeconds(0.3f);

		//rotator thing
		//puzzleRotator.locked = false;
		yield return wait;

		instructRotateGO.SetActive(true);

		yield return dlgRotatePointer.Play();

		instructRotateGO.SetActive(false);

		puzzleRotator.value = 7.5f;

		yield return wait;

		//slider thing
		puzzleSlider.locked = false;
		yield return wait;

		instructSliderGO.SetActive(true);

		yield return dlgSliderPointer.Play();

		instructSliderGO.SetActive(false);

		puzzlePlunger.locked = false;

		yield return dlgGameStart.Play();
	}

	void OnPlatformPushed(Collider2D coll) {
		puzzlePlatformSpring.exitCallback.RemoveListener(OnPlatformPushed);

		instructPlungerGO.SetActive(false);
		instructPushGO.SetActive(false);

		StartCoroutine(DoPlatformPushed());
	}
}
