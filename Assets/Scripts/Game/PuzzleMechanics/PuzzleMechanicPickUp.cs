using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PuzzleMechanicPickUp : PuzzleMechanicBase {
	public DropOffData dropOffData; //determines which drop-off this can be placed in

	public PuzzleDropOff initialDropOff;

	[Header("Displays")]
	public M8.RendererGroupSortingOrderOffset renderGroupSortOrder;

	[Header("Motion")]
	public float moveDelay = 0.15f;
	public float rotateDelay = 0.15f; //rotation towards anchor

	[Header("Events")]
	public UnityEvent onPickUp;
	public UnityEvent onDropOff;

	public Vector2 position { 
		get { return transform.position; } 
		set {
			var pos = transform.position;
			
			pos.x = value.x;
			pos.y = value.y;

			transform.position = pos;
		} 
	}

	public float rotation { 
		get { return transform.eulerAngles.z; } 
		set {
			var angles = transform.eulerAngles;
			angles.z = value;

			transform.eulerAngles = angles; 
		} 
	}

	public bool isMoving { get; private set; }

	public PuzzleDropOff currentDropOff { get; private set; }

	private PointerEventData mPointer;
	private PuzzleDropOff mPointerDropOff;

	private Vector2 mMoveVel;
	private float mMoveAngleVel;

	public void SwapDropOff(PuzzleMechanicPickUp other) {
		var _dropOff = currentDropOff;

		currentDropOff = other.currentDropOff;

		other.currentDropOff = _dropOff;

		if(currentDropOff) currentDropOff.pickUpAttached = this;
		if(other.currentDropOff) other.currentDropOff.pickUpAttached = other;
	}

	public void ApplyDropOff(PuzzleDropOff aDropOff) {
		if(currentDropOff) currentDropOff.pickUpAttached = null;

		currentDropOff = aDropOff;

		if(currentDropOff) currentDropOff.pickUpAttached = this;
	}

	void OnApplicationFocus(bool focus) {
		if(!focus) {
			if(mPointer != null)
				DropOff();
		}
	}

	void Update() {
		var isPosChanged = false;
		var isRotChanged = false;

		if(mPointer != null) {
			RefreshDropOff();

			var dt = Time.deltaTime;

			//update move dest.
			var cam = Camera.main;
			var moveDest = (Vector2)cam.ScreenToWorldPoint(mPointer.position);

			isPosChanged = position != moveDest;
			if(isPosChanged)
				position = Vector2.SmoothDamp(position, moveDest, ref mMoveVel, dt);

			isRotChanged = rotation != 0f;
			if(isRotChanged)
				rotation = Mathf.SmoothDampAngle(rotation, 0f, ref mMoveAngleVel, dt);
		}
		else if(currentDropOff) {
			var dt = Time.deltaTime;

			//move local towards origin
			isPosChanged = position != currentDropOff.anchorPosition;
			if(isPosChanged)
				position = Vector2.SmoothDamp(position, currentDropOff.anchorPosition, ref mMoveVel, Time.deltaTime);

			isRotChanged = rotation != currentDropOff.anchorRotation;
			if(isRotChanged)
				rotation = Mathf.SmoothDampAngle(rotation, currentDropOff.anchorRotation, ref mMoveAngleVel, dt);
		}

		isMoving = isPosChanged || isRotChanged;
	}

	protected override void Awake() {
		base.Awake();

		if(!renderGroupSortOrder)
			renderGroupSortOrder = GetComponent<M8.RendererGroupSortingOrderOffset>();

		currentDropOff = initialDropOff;
		if(currentDropOff)
			currentDropOff.pickUpAttached = this;
	}

	protected override void InputClick(PointerEventData eventData) {
		if(mPointer != null) {
			DropOff();
		}
		else {
			ApplyPickUp(eventData);
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
		if(mPointer == null)
			return;

		PuzzleDropOff newDropOff = null;

		//check drop off on current point
		var rayDat = mPointer.pointerCurrentRaycast;
		if(rayDat.isValid) {
			PuzzleDropOff dropOff = null;

			if((GameData.instance.layerDropOff & (1 << rayDat.gameObject.layer)) != 0) {
				dropOff = rayDat.gameObject.GetComponent<PuzzleDropOff>();
			}
			else if(rayDat.gameObject != input.gameObject) {
				var otherPickUp = rayDat.gameObject.GetComponent<PuzzleMechanicPickUp>();
				if(otherPickUp)
					dropOff = otherPickUp.currentDropOff;
			}

			if(dropOff && dropOffData == dropOff.data)
				newDropOff = dropOff;
		}

		if(mPointerDropOff != newDropOff) {
			if(mPointerDropOff)
				mPointerDropOff.onDropOffHighlight?.Invoke(false);

			mPointerDropOff = newDropOff;
			if(mPointerDropOff)
				mPointerDropOff.onDropOffHighlight?.Invoke(true);
		}
	}

	private void ApplyPickUp(PointerEventData eventData) {
		mPointer = eventData;

		if(mPointer != null) {
			if(renderGroupSortOrder)
				renderGroupSortOrder.ApplyOffset(GameData.instance.mechanicPuzzlePickUpRenderOrder);

			input.collision.enabled = false;

			onPickUp?.Invoke();
		}
		else
			input.collision.enabled = true;

		mMoveVel = Vector2.zero;
		mMoveAngleVel = 0f;
	}
		
	private void DropOff() {
		var isPicked = mPointer != null || mPointerDropOff;

		//check drop off object
		if(mPointerDropOff) {
			mPointerDropOff.onDropOffHighlight?.Invoke(false);
			mPointerDropOff.onDropOff?.Invoke(this);

			//apply to drop off
			if(mPointerDropOff.pickUpAttached != this) {
				if(mPointerDropOff.pickUpAttached)
					SwapDropOff(mPointerDropOff.pickUpAttached);
				else
					ApplyDropOff(mPointerDropOff);
			}

			mPointerDropOff.pickUpAttached = this;

			mPointerDropOff = null;
		}

		ApplyPickUp(null);

		if(isPicked) {
			if(renderGroupSortOrder)
				renderGroupSortOrder.Revert();

			onDropOff?.Invoke();
		}
	}
}
