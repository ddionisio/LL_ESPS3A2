using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntitySolidTriggerDespawn : MonoBehaviour {
	[M8.TagSelector]
	public string checkTag;

	void OnTriggerEnter2D(Collider2D collision) {
		if(!string.IsNullOrEmpty(checkTag) && collision.CompareTag(checkTag)) {
			var ent = collision.GetComponent<PuzzleEntitySolid>();
			ent.Respawn();
		}
	}
}
