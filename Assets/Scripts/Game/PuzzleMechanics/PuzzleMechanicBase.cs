using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PuzzleMechanicBase : MonoBehaviour {
    [SerializeField]
    PuzzleMechanicInput _input;

    public PuzzleMechanicInput input { get { return _input; } }

	public bool locked {
		get { return mLocked; }
		set {
			if(mLocked != value) {
				mLocked = value;
			}
		}
	}

	private bool mLocked;
	private bool mPuzzleInteractable;

	protected virtual void InputClick(PointerEventData eventData) { }

	protected virtual void InputDragBegin(PointerEventData eventData) { }
	protected virtual void InputDrag(PointerEventData eventData) { }
	protected virtual void InputDragEnd(PointerEventData eventData) { }

	protected virtual void OnDestroy() {
		if(_input) {
			_input.clickCallback -= InputClick;
			_input.dragBeginCallback -= InputDragBegin;
			_input.dragCallback -= InputDrag;
			_input.dragEndCallback -= InputDragEnd;
		}

		if(GameData.isInstantiated)
			GameData.instance.signalPuzzleInteractable.callback -= OnSignalPuzzleInteractable;
	}

	protected virtual void Awake() {
		if(_input) {
			_input.clickCallback += InputClick;
			_input.dragBeginCallback += InputDragBegin;
			_input.dragCallback += InputDrag;
			_input.dragEndCallback += InputDragEnd;
		}

		RefreshInput();

		GameData.instance.signalPuzzleInteractable.callback += OnSignalPuzzleInteractable;
	}

	private void RefreshInput() {
		if(_input)
			_input.interactable = !mLocked && mPuzzleInteractable;
	}

	void OnSignalPuzzleInteractable(bool interactable) {
		mPuzzleInteractable = interactable;

		RefreshInput();
	}
}
