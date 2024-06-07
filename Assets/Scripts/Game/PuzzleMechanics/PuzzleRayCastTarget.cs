using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// React to PuzzleRayCast when it is hit. Should put collider here
/// </summary>
public class PuzzleRayCastTarget : MonoBehaviour {
	public CastData castData;

	public UnityEvent<bool> onCastHitActive;
    public UnityEvent<PuzzleRayCast> onCastHit;

    public UnityEvent onCastActiveUpdate;
	public UnityEvent<PuzzleRayCast> onCastUpdate;

	public Collider2D coll {
		get {
			if(!mColl)
				mColl = GetComponent<Collider2D>();
			return mColl;
		}
	}

	private Collider2D mColl;

	/// <summary>
	/// Call when cast to new target
	/// </summary>
	public void ApplyCast(PuzzleRayCast puzzleRayCast) {
		onCastHitActive?.Invoke(puzzleRayCast != null);

		onCastHit?.Invoke(puzzleRayCast);
	}

	/// <summary>
	/// Call during update while target is valid
	/// </summary>
    public void UpdateCast(PuzzleRayCast puzzleRayCast) {
		onCastActiveUpdate?.Invoke();

		onCastUpdate?.Invoke(puzzleRayCast);
    }
}
