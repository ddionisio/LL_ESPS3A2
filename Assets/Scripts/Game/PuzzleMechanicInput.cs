using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleMechanicInput : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	public bool interactable {
		get { return mInteractable; }
		set {
			if(mInteractable != value) {
				mInteractable = value;
				ApplyInteractable();
			}
		}
	}

	public Collider2D collision {
		get {
			if(!mColl)
				mColl = GetComponent<Collider2D>();

			return mColl;
		}
	}

	public bool isDragging { get; private set; }

	public event System.Action<PointerEventData> clickCallback;

	public event System.Action<PointerEventData> dragBeginCallback;
	public event System.Action<PointerEventData> dragCallback;
	public event System.Action<PointerEventData> dragEndCallback;

	private bool mInteractable;
    private Collider2D mColl;

	void OnApplicationFocus(bool focus) {
		EndDragging(null);
	}

	void Awake() {
		mInteractable = false;
		ApplyInteractable();
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		clickCallback?.Invoke(eventData);
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
		isDragging = true;

		dragBeginCallback?.Invoke(eventData);
	}

	void IDragHandler.OnDrag(PointerEventData eventData) {
		if(isDragging)
			dragCallback?.Invoke(eventData);
	}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
		EndDragging(eventData);
	}

	private void EndDragging(PointerEventData eventData) {
		if(isDragging) {
			isDragging = false;

			dragEndCallback?.Invoke(eventData);
		}
	}

	private void ApplyInteractable() {
		collision.enabled = mInteractable;

		//reset input
		EndDragging(null);
	}
}
