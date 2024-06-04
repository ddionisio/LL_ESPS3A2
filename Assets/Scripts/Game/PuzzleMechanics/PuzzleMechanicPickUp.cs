using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;



public class PuzzleMechanicPickUp : PuzzleMechanicBase {
	public DropOffData dropOffData; //determines which drop-off this can be placed in

	[Header("Motion")]
	public float moveDelay = 0.15f;

	[Header("Events")]
	public UnityEvent onPickUp;
	public UnityEvent onDropOff;

	public Vector2 position { get { return transform.position; } set { transform.position = value; } }

	public Vector2 pointerWorldPosition {
		get {
			if(mPointer != null) {
				//TODO: use main camera for now
				var cam = Camera.main;
				return cam.ScreenToWorldPoint(mPointer.position);
			}

			return position;
		}
	}

	public bool isMoving { get; private set; }

	private PointerEventData mPointer;
	private PuzzleDropOff mDropOff;

	private Vector2 mMoveDest;
	private Vector2 mMoveVel;

	private Vector2 mLastPos; //position before being picked up

	void OnApplicationFocus(bool focus) {
		if(!focus) {
			if(mPointer != null)
				DropOff();
		}
	}

	void Update() {
		if(mPointer != null) {
			RefreshDropOff();

			//update move dest.
			mMoveDest = pointerWorldPosition;
		}

		var pos = position;
		isMoving = pos != mMoveDest;
		if(isMoving) {
			position = Vector2.SmoothDamp(pos, mMoveDest, ref mMoveVel, Time.deltaTime);
		}
	}

	protected override void InputClick(PointerEventData eventData) {
		if(mPointer != null) {
			DropOff();
		}
		else {
			ApplyPickUp(eventData);

			onPickUp?.Invoke();
		}
	}

	protected override void InputDragBegin(PointerEventData eventData) {
		ApplyPickUp(eventData);
	}

	protected override void InputDrag(PointerEventData eventData) {
		mPointer = eventData;
	}

	protected override void InputDragEnd(PointerEventData eventData) {
		mPointer = eventData;
		RefreshDropOff();
		DropOff();
	}

	protected override void RefreshInput() {
		base.RefreshInput();

		if(!interactable)
			DropOff();
	}

	private void RefreshDropOff() {
		PuzzleDropOff newDropOff = null;

		//check drop off on current point
		var rayDat = mPointer.pointerCurrentRaycast;
		if(rayDat.isValid) {
			if((GameData.instance.layerDropOff & (1 << rayDat.gameObject.layer)) != 0) {
				var dropOff = rayDat.gameObject.GetComponent<PuzzleDropOff>();
				if(dropOff && dropOffData == dropOff.data)
					newDropOff = dropOff;
			}
		}

		if(mDropOff != newDropOff) {
			if(mDropOff)
				mDropOff.onDropOffHighlight?.Invoke(false);

			mDropOff = newDropOff;
			if(mDropOff)
				mDropOff.onDropOffHighlight?.Invoke(true);
		}
	}

	private void ApplyPickUp(PointerEventData eventData) {
		mPointer = eventData;

		if(mPointer != null) {
			mLastPos = position;
			mMoveDest = pointerWorldPosition;

			onPickUp?.Invoke();
		}

		mMoveVel = Vector2.zero;
	}
		
	private void DropOff() {
		var isPicked = mPointer != null || mDropOff;

		//check drop off object
		if(mDropOff) {
			mDropOff.onDropOffHighlight?.Invoke(false);
			mDropOff.onDropOff?.Invoke(this);

			mMoveDest = mDropOff.anchorPosition;

			mDropOff = null;
		}
		else //revert to original position
			mMoveDest = mLastPos;

		ApplyPickUp(null);

		if(isPicked)
			onDropOff?.Invoke();
	}
}
