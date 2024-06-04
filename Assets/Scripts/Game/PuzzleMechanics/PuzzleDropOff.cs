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
}
