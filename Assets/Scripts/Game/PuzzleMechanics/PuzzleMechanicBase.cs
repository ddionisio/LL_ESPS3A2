using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class PuzzleMechanicBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField]
    Collider2D _collider;

	public UnityEvent<bool> onInputInteractable;
	public UnityEvent<bool> onInputDown;
	public UnityEvent<bool> onInputEnter;

	public bool locked {
		get { return mLocked; }
		set {
			if(mLocked != value) {
				mLocked = value;
				RefreshInput();

				if(_collider) _collider.enabled = !mLocked && mCollEnabled;
			}
		}
	}

	public bool colliderEnabled {
		get { return mCollEnabled; }
		set {
			if(mCollEnabled != value) {
				mCollEnabled = value;

				if(_collider) _collider.enabled = !mLocked && mCollEnabled;
			}
		}
	}

	public bool interactable { get { return !mLocked && mInteractable; } }

	public bool isDragging { get; private set; }

	public bool isDown {
		get { return mIsDown; }
		private set {
			if(mIsDown != value) {
				mIsDown = value;

				InputDown(mIsDown);

				onInputDown?.Invoke(mIsDown);
			}
		}
	}

	public bool isEnter {
		get { return mIsEnter; }
		private set {
			if(mIsEnter != value) {
				mIsEnter = value;

				InputEnter(mIsEnter);

				onInputEnter?.Invoke(mIsEnter);
			}
		}
	}

	private bool mLocked;
	private bool mInteractable;
	private bool mCollEnabled = true; //assume collider is enabled by default...
	private bool mIsDown;
	private bool mIsEnter;

	protected virtual void InputEnter(bool isEnter) { }

	protected virtual void InputDown(bool isDown) { }

	protected virtual void InputClick(PointerEventData eventData) { }

	protected virtual void InputDragBegin(PointerEventData eventData) { }
	protected virtual void InputDrag(PointerEventData eventData) { }
	protected virtual void InputDragEnd(PointerEventData eventData) { }

	protected virtual void OnApplicationFocus(bool focus) {
		EndDragging(null);

		isDown = false;
		isEnter = false;
	}

	protected virtual void OnDestroy() {
		if(GameData.isInstantiated)
			GameData.instance.signalPuzzleInteractable.callback -= OnSignalPuzzleInteractable;
	}

	protected virtual void OnEnable() {
		RefreshInput();
	}

	protected virtual void OnDisable() {

	}

	protected virtual void Awake() {
		GameData.instance.signalPuzzleInteractable.callback += OnSignalPuzzleInteractable;
	}

	protected virtual void RefreshInput() {
		if(!interactable) {
			//reset input
			EndDragging(null);

			isDown = false;
			isEnter = false;
		}

		onInputInteractable?.Invoke(interactable);
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
		if(!mInteractable) return;

		isEnter = true;
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
		if(!mInteractable) return;

		isEnter = false;
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		if(!mInteractable) return;

		isDown = true;
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		if(!mInteractable) return;

		isDown = false;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		if(!mInteractable) return;

		isDown = false;

		InputClick(eventData);
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		if(!mInteractable) return;

		isDragging = true;

		InputDragBegin(eventData);
	}

	void IDragHandler.OnDrag(PointerEventData eventData) {
		if(!mInteractable) return;

		if(isDragging)
			InputDrag(eventData);
	}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
		if(!mInteractable) return;

		EndDragging(eventData);
	}

	private void EndDragging(PointerEventData eventData) {
		if(isDragging) {
			isDragging = false;

			InputDragEnd(eventData);
		}
	}

	void OnSignalPuzzleInteractable(bool interactable) {
		mInteractable = interactable;

		RefreshInput();
	}
}
