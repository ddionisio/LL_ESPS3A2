using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// React to PuzzleRayCast when it is hit. Should put collider here
/// </summary>
public class PuzzleRayCastTarget : MonoBehaviour {
	public CastData castData;

	[Header("Reflect")]
	public float reflectCheckAngleLimit = 90f;
	public Transform reflectUpDirRoot;

	[Header("Events")]
	public UnityEvent<bool> onCastHitActive;
    public UnityEvent<PuzzleRayCast> onCastHit;

	public UnityEvent<bool> onCastValid;

    public UnityEvent onCastActiveUpdate;
	public UnityEvent<PuzzleRayCast> onCastUpdate;

	public Collider2D coll {
		get {
			if(!mColl)
				mColl = GetComponent<Collider2D>();
			return mColl;
		}
	}

	public bool collisionEnabled {
		get {
			var _coll = coll;
			return _coll ? _coll.enabled : false; }
		set {
			var _coll = coll;
			if(_coll) {
				_coll.enabled = value;

				if(!value) {
					onCastHitActive?.Invoke(false);
					onCastHit?.Invoke(null);
					onCastValid?.Invoke(false);
				}
			}
		}
	}

	private Collider2D mColl;

	/// <summary>
	/// Call when cast to new target
	/// </summary>
	public void ApplyCast(PuzzleRayCast puzzleRayCast) {
		var isActive = puzzleRayCast != null;

		onCastHitActive?.Invoke(isActive);

		onCastHit?.Invoke(puzzleRayCast);

		if(isActive)
			ApplyValid(puzzleRayCast.castDir);
		else
			onCastValid?.Invoke(false);
	}

	/// <summary>
	/// Call during update while target is valid
	/// </summary>
    public void UpdateCast(PuzzleRayCast puzzleRayCast) {
		onCastActiveUpdate?.Invoke();

		onCastUpdate?.Invoke(puzzleRayCast);

		ApplyValid(puzzleRayCast.castDir);
	}

	private void ApplyValid(Vector2 castDir) {
		var isValid = false;

		if(reflectUpDirRoot) {
			Vector2 upDir = transform.up;

			var angle = Mathf.Abs(Vector2.SignedAngle(-castDir, upDir));
			if(angle <= reflectCheckAngleLimit) {
				reflectUpDirRoot.up = Vector2.Reflect(castDir, upDir);
				isValid = true;
			}
		}

		onCastValid?.Invoke(isValid);
	}
}
