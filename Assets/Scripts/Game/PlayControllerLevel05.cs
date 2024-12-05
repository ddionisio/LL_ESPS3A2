using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class PlayControllerLevel05 : PlayControllerBase {
	[Header("Puzzle")]
	public GameObject puzzleCoalTosserGO;
	public GameObject[] puzzleCoalGOs;

	public GameObject puzzleAirSpiritGO;
	public GameObject puzzleFireHearthGO;
	public GameObject puzzleWaterTankGO;
	public GameObject puzzlePowerGeneratorGO;
	public GameObject puzzleBrainGO;

	public GoalController[] puzzleGoals;

	[Header("Mechanics")]
	public PuzzleEntitySpiritFire spiritFire;
	public PuzzleMechanicBase coalTosserInteract;
	public PuzzleGameplayPowerConnect[] powerConnectors;

	[Header("Animations")]
	public M8.AnimatorTargetParamTrigger animSpiritWaterEnter;
	public M8.AnimatorTargetParamTrigger animSpiritWaterAction;

	[Header("Pointers")]
	public GameObject coalTosserPointerGO;
	public GameObject airSpiritPointerGO;
	public GameObject powerConnectDragInstructGO;

	[Header("Music")]
	[M8.MusicPlaylist]
	public string musicEnd;

	[Header("Dialogs")]
	public ModalDialogFlowIncremental dlgIntro;
	public ModalDialogFlowIncremental dlgTurbineIntro;
	public ModalDialogFlowIncremental dlgWaterIntro;
	public ModalDialogFlowIncremental dlgFireIntro;
	public ModalDialogFlowIncremental dlgCoalIntro;
	public ModalDialogFlowIncremental dlgElectricExplain;
	public ModalDialogFlowIncremental dlgCoalInteract;
	public ModalDialogFlowIncremental dlgPowerFirstActive;
	public ModalDialogFlowIncremental dlgPowerConnectInstruct;
	public ModalDialogFlowIncremental dlgGoalFirstActive;
	public ModalDialogFlowIncremental dlgWindIntro;
	public ModalDialogFlowIncremental dlgWindInstruct;
	public ModalDialogFlowIncremental dlgPowerAllActive;
	public ModalDialogFlowIncremental dlgEnd;

	private PuzzleMechanicBase[] mPowerConnectorMechanics;

	private bool mPowerAllActiveCheck = false;

	protected override IEnumerator Intro() {
		puzzleBrainGO.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		yield return dlgIntro.Play();

		//turbine
		puzzlePowerGeneratorGO.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		yield return dlgTurbineIntro.Play();

		//water
		puzzleWaterTankGO.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		animSpiritWaterEnter.Set();

		yield return dlgWaterIntro.Play();

		//fire
		puzzleFireHearthGO.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		yield return dlgFireIntro.Play();

		//coal
		puzzleCoalTosserGO.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		for(int i = 0; i < puzzleCoalGOs.Length; i++)
			puzzleCoalGOs[i].SetActive(true);

		yield return dlgCoalIntro.Play();

		yield return dlgElectricExplain.Play();
	}

	protected override IEnumerator GameBegin() {
		coalTosserPointerGO.SetActive(true);

		yield return dlgCoalInteract.Play();

		coalTosserPointerGO.SetActive(false);

		coalTosserInteract.locked = false;
	}

	protected override void GameUpdate() {
		if(mPowerAllActiveCheck) {
			var powerActiveCount = 0;
			for(int i = 0; i < powerConnectors.Length; i++) {
				var powerConnector = powerConnectors[i];
				if(powerConnector.isPowerActive)
					powerActiveCount++;
			}

			if(powerActiveCount == powerConnectors.Length) {
				mPowerAllActiveCheck = false;
				StartCoroutine(DoPowerAllActive());
			}
		}
	}

	protected override IEnumerator GameEnd() {
		for(int i = 0; i < puzzleGoals.Length; i++)
			puzzleGoals[i].ForceAudioStop();

		if(!string.IsNullOrEmpty(musicEnd))
			M8.MusicPlaylist.instance.Play(musicEnd, false, true);

		animSpiritWaterAction.Set();

		yield return dlgEnd.Play();
	}

	protected override void OnInstanceDeinit() {


		base.OnInstanceDeinit();
	}

	protected override void OnInstanceInit() {
		base.OnInstanceInit();

		puzzleCoalTosserGO.SetActive(false);

		for(int i = 0; i < puzzleCoalGOs.Length; i++)
			puzzleCoalGOs[i].SetActive(false);

		puzzleAirSpiritGO.SetActive(false);
		puzzleFireHearthGO.SetActive(false);
		puzzleWaterTankGO.SetActive(false);
		puzzlePowerGeneratorGO.SetActive(false);
		puzzleBrainGO.SetActive(false);

		spiritFire.consumeEnable = false;

		coalTosserInteract.locked = true;

		mPowerConnectorMechanics = new PuzzleMechanicBase[powerConnectors.Length];

		for(int i = 0; i < powerConnectors.Length; i++) {
			var powerConnector = powerConnectors[i];

			mPowerConnectorMechanics[i] = powerConnector.gameObject.GetComponent<PuzzleMechanicBase>();
			if(mPowerConnectorMechanics[i])
				mPowerConnectorMechanics[i].locked = true;
		}

		powerConnectors[0].powerActiveEvent.AddListener(OnPowerFirstActive);

		puzzleGoals[0].powerFullyCharged.AddListener(OnGoalFirstActive);

		coalTosserPointerGO.SetActive(false);
		airSpiritPointerGO.SetActive(false);
		powerConnectDragInstructGO.SetActive(false);
	}

	IEnumerator DoPowerFirstActive() {
		animSpiritWaterAction.Set();

		yield return dlgPowerFirstActive.Play();

		powerConnectDragInstructGO.SetActive(true);

		yield return dlgPowerConnectInstruct.Play();

		mPowerConnectorMechanics[0].locked = false;
	}

	IEnumerator DoGoalFirstActive() {
		animSpiritWaterAction.Set();

		powerConnectDragInstructGO.SetActive(false);

		yield return dlgGoalFirstActive.Play();

		puzzleAirSpiritGO.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		yield return dlgWindIntro.Play();

		airSpiritPointerGO.SetActive(true);

		yield return dlgWindInstruct.Play();

		airSpiritPointerGO.SetActive(false);

		mPowerAllActiveCheck = true;
	}

	IEnumerator DoPowerAllActive() {
		animSpiritWaterAction.Set();

		yield return dlgPowerAllActive.Play();

		for(int i = 0; i < mPowerConnectorMechanics.Length; i++)
			mPowerConnectorMechanics[i].locked = false;

		spiritFire.consumeEnable = true;
	}

	void OnPowerFirstActive(bool aActive) {
		if(aActive) {
			powerConnectors[0].powerActiveEvent.RemoveListener(OnPowerFirstActive);

			StartCoroutine(DoPowerFirstActive());
		}
	}

	void OnGoalFirstActive(bool aActive) {
		if(aActive) {
			puzzleGoals[0].powerFullyCharged.RemoveListener(OnGoalFirstActive);

			StartCoroutine(DoGoalFirstActive());
		}
	}
}
