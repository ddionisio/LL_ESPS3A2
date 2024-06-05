using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleDropOff : MonoBehaviour {
	public DropOffData data; //data ref. that a pickup should match with dropOffData

	public Transform anchor; //point to place pickup

	[Header("Events")]
	public UnityEvent<bool> onDropOffHighlight;
	public UnityEvent<PuzzleMechanicPickUp> onDropOff;

	public Vector2 anchorPosition { get { return anchor ? anchor.position : transform.position; } }

	public Vector2 anchorUp { get { return anchor ? anchor.up : Vector2.up; } }

	public float anchorRotation { get { return anchor ? anchor.eulerAngles.z : 0f; } }

	public PuzzleMechanicPickUp pickUpAttached { 
		get { return mPickUpAttached; }
		set {
			if(mPickUpAttached != value) {
				mPickUpAttached = value;
			}
		}
	}

	private PuzzleMechanicPickUp mPickUpAttached;
}
