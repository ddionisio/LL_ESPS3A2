using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGameplaySector : MonoBehaviour {
	[System.Serializable]
	public class Link {
		public string label;
		public PuzzleMechanicSwitch switchRef;
		public PuzzleGameplaySector[] sectors; //sectors affected by this link

	}

	[SerializeField]
	bool _alwaysFilled;

	public Link[] links;

	public bool isFilled { get; private set; }
}
