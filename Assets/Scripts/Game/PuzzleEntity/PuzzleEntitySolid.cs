using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleEntityState {
	None,

	Spawn,
	Despawn,

	Idle,
	Action,

	Victory,
}

public class PuzzleEntitySolid : MonoBehaviour {
	[Header("Setup")]
	public Transform attachRoot;

	public PuzzleEntityState state {
        get { return mState; }
        set {
			if(mState != value) {
				mState = value; //allow one update cycle before changing
				ApplyState();
			}
		}
    }

	public Vector2 position { get { return transform.position; } set { transform.position = value; } }

	public float rotation { get { return Vector2.SignedAngle(Vector2.up, transform.up); } set { transform.up = M8.MathUtil.Rotate(Vector2.up, value); } }

	public Rigidbody2D body { get { return mBody; } }

	public Collider2D coll { get { return mColl; } }

	public Animator animator { get { return mAnim; } }

	public bool isBusy { get { return mRout != null; } }

	private Animator mAnim;
	private Rigidbody2D mBody;
	private Collider2D mColl;

	private PuzzleEntityState mState = PuzzleEntityState.None;

	private Coroutine mRout;

	void OnDisable() {
		ClearRout();
	}

	void Awake() {
		mAnim = GetComponent<Animator>();
		mBody = GetComponent<Rigidbody2D>();
		mColl = GetComponentInChildren<Collider2D>();

		ApplyState();
	}

	void Start() {
        
	}

	void Update() {
       
	}

	private void ApplyState() {
		ClearRout();

		if(mState == PuzzleEntityState.Action) {
			body.velocity = Vector2.zero;
			body.angularVelocity = 0f;

			body.simulated = true;
		}
		else {
			body.simulated = false;
		}
	}

	private void ClearRout() {
		if(mRout != null) {
			StopCoroutine(mRout);
			mRout = null;
		}
	}
}
