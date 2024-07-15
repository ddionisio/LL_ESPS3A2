using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class PuzzleMechanicBase : MonoBehaviour {
    [SerializeField]
    PuzzleMechanicInput _input;

	public UnityEvent<bool> onInputInteractable;
	public UnityEvent<bool> onInputDown;
	public UnityEvent<bool> onInputEnter;

	public PuzzleMechanicInput input { get { return _input; } }

	public bool locked {
		get { return mLocked; }
		set {
			if(mLocked != value) {
				mLocked = value;
				RefreshInput();
			}
		}
	}

	public bool interactable { get { return !mLocked && mPuzzleInteractable; } }

	private bool mLocked;
	private bool mPuzzleInteractable;

	protected virtual void InputEnter(bool isEnter) { }

	protected virtual void InputDown(bool isDown) { }

	protected virtual void InputClick(PointerEventData eventData) { }

	protected virtual void InputDragBegin(PointerEventData eventData) { }
	protected virtual void InputDrag(PointerEventData eventData) { }
	protected virtual void InputDragEnd(PointerEventData eventData) { }

	protected virtual void OnDestroy() {
		if(_input) {
			_input.enterCallback -= InputEnter;
			_input.enterCallback -= onInputEnter.Invoke;

			_input.downCallback -= InputDown;
			_input.downCallback -= onInputDown.Invoke;

			_input.clickCallback -= InputClick;
			_input.dragBeginCallback -= InputDragBegin;
			_input.dragCallback -= InputDrag;
			_input.dragEndCallback -= InputDragEnd;
		}

		if(GameData.isInstantiated)
			GameData.instance.signalPuzzleInteractable.callback -= OnSignalPuzzleInteractable;
	}

	protected virtual void OnEnable() {
		RefreshInput();
	}

	protected virtual void OnDisable() {

	}

	protected virtual void Awake() {
		if(_input) {
			_input.enterCallback += InputEnter;
			_input.enterCallback += onInputEnter.Invoke;

			_input.downCallback += InputDown;
			_input.downCallback += onInputDown.Invoke;

			_input.clickCallback += InputClick;
			_input.dragBeginCallback += InputDragBegin;
			_input.dragCallback += InputDrag;
			_input.dragEndCallback += InputDragEnd;
		}
				
		GameData.instance.signalPuzzleInteractable.callback += OnSignalPuzzleInteractable;
	}

	protected virtual void RefreshInput() {
		if(_input) {
			_input.interactable = interactable;

			onInputInteractable?.Invoke(interactable);
		}
	}

	void OnSignalPuzzleInteractable(bool interactable) {
		mPuzzleInteractable = interactable;

		RefreshInput();
	}
}
