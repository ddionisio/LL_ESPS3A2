using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class PuzzleMechanicInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	public bool interactable {
		get { return mInteractable; }
		set {
			if(mInteractable != value) {
				mInteractable = value;

				//reset input
				EndDragging(null);

				isDown = false;
			}
		}
	}

	public bool colliderEnabled {
		get {
			if(!mColl) ColliderInit();

			return mColl.enabled; 
		}
		set {
			if(!mColl) ColliderInit();

			mColl.enabled = value;
		}
	}

	public bool isDragging { get; private set; }

	public bool isDown {
		get { return mIsDown; }
		private set {
			if(mIsDown != value) {
				mIsDown = value;

				if(mIsDown)
					downCallback?.Invoke();
				else
					upCallback?.Invoke();
			}
		}
	}

	public event System.Action downCallback;
	public event System.Action upCallback;

	public event System.Action<PointerEventData> clickCallback;

	public event System.Action<PointerEventData> dragBeginCallback;
	public event System.Action<PointerEventData> dragCallback;
	public event System.Action<PointerEventData> dragEndCallback;

	private bool mInteractable;
	private bool mIsDown;
    private Collider2D mColl;

	void OnApplicationFocus(bool focus) {
		EndDragging(null);

		isDown = false;
	}

	void Awake() {
		ColliderInit();
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		isDown = true;
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		isDown = false;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		if(!mInteractable) return;

		isDown = false;

		clickCallback?.Invoke(eventData);
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		if(!mInteractable) return;

		isDragging = true;

		dragBeginCallback?.Invoke(eventData);
	}

	void IDragHandler.OnDrag(PointerEventData eventData) {
		if(!mInteractable) return;

		if(isDragging)
			dragCallback?.Invoke(eventData);
	}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
		if(!mInteractable) return;

		EndDragging(eventData);
	}

	private void EndDragging(PointerEventData eventData) {
		if(isDragging) {
			isDragging = false;

			dragEndCallback?.Invoke(eventData);
		}
	}

	private void ColliderInit() {
		mColl = GetComponent<Collider2D>();
	}
}
