using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class PuzzleMechanicBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	[SerializeField]
	bool _interactable;
	[SerializeField]
	bool _colliderEnabled = true; //assume collider is enabled by default...

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

				if(_collider) _collider.enabled = !mLocked && _colliderEnabled;
			}
		}
	}

	public bool colliderEnabled {
		get { return _colliderEnabled; }
		set {
			if(_colliderEnabled != value) {
				_colliderEnabled = value;

				if(_collider) _collider.enabled = !mLocked && _colliderEnabled;
			}
		}
	}

	public bool interactable { get { return !mLocked && _interactable; } }

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

		if(_collider) _collider.enabled = !mLocked && _colliderEnabled;
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
		if(!_interactable) return;

		isEnter = true;
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
		if(!_interactable) return;

		isEnter = false;
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		if(!_interactable) return;

		isDown = true;
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		if(!_interactable) return;

		isDown = false;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		if(!_interactable) return;

		isDown = false;

		InputClick(eventData);
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		if(!_interactable) return;

		isDragging = true;

		InputDragBegin(eventData);
	}

	void IDragHandler.OnDrag(PointerEventData eventData) {
		if(!_interactable) return;

		if(isDragging)
			InputDrag(eventData);
	}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
		if(!_interactable) return;

		EndDragging(eventData);
	}

	private void EndDragging(PointerEventData eventData) {
		if(isDragging) {
			isDragging = false;

			InputDragEnd(eventData);
		}
	}

	void OnSignalPuzzleInteractable(bool interactable) {
		_interactable = interactable;

		RefreshInput();
	}
}
