using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleDropOff : MonoBehaviour {
	public DropOffData data; //data ref. that a pickup should match with dropOffData

	public Transform anchor; //point to place pickup
	public Collider2D coll;
	
	[SerializeField]
	bool _active = true;
	[SerializeField]
	GameObject _inactiveGO;

	[Header("Events")]
	public UnityEvent<bool> onDropOffHighlight;
	public UnityEvent<PuzzleMechanicPickUp> onDropOff;
	public UnityEvent<bool> onDropOffPickupAttach;

	public bool active {
		get { return _active; }
		set {
			if(_active != value) {
				_active = value;

				ApplyActive();

				if(!_active) {
					onDropOffHighlight?.Invoke(false);

					if(pickUpAttached) //TODO: this allows turning off control attached to this drop off, can be buggy in certain circumstances
						onDropOffPickupAttach?.Invoke(false);
				}
			}
		}
	}

	public Vector2 anchorPosition { get { return anchor ? anchor.position : transform.position; } }

	public Vector2 anchorUp { get { return anchor ? anchor.up : Vector2.up; } }

	public float anchorRotation { get { return anchor ? anchor.eulerAngles.z : 0f; } }

	public PuzzleMechanicPickUp pickUpAttached { 
		get { return mPickUpAttached; }

		set {
			mPickUpAttached = value;

			onDropOffPickupAttach?.Invoke(mPickUpAttached != null);
		}
	}
		
	private PuzzleMechanicPickUp mPickUpAttached;

	void OnEnable() {
		ApplyActive();
	}

	private void ApplyActive() {
		if(!coll) coll = GetComponent<Collider2D>();

		coll.enabled = _active;

		if(_inactiveGO) _inactiveGO.SetActive(!_active);
	}
}
