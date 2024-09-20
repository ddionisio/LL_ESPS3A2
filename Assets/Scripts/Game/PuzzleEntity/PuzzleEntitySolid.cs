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

	[Header("Action")]
	public M8.RangeFloat actionMoveForceRange; //when called to move with given 't'
	public M8.RangeFloat actionMoveImpulseRange; //when called to move with given 't'
	public float actionMoveForceDuration = 0.5f;
	public bool actionEndEnabled = true;
	public float actionEndDelay = 2f;
	public float actionEndSpeedThreshold = 0.5f;	

	[Header("Animation")]
	public M8.AnimatorParamTrigger animSpawn;
	public M8.AnimatorParamTrigger animDespawn;
	public M8.AnimatorParamTrigger animIdle;
	public M8.AnimatorParamTrigger animAction;
	public M8.AnimatorParamTrigger animVictory;

	public bool active { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }

	public PuzzleEntityState state {
        get { return mState; }
        private set {
			if(mState != value) {
				mState = value;
				ApplyState();
			}
		}
    }

	public Rigidbody2D body { get { return mBody; } }

	public Collider2D coll { get { return mColl; } }

	public Animator animator { get { return mAnim; } }

	public bool isBusy { get { return mRout != null; } }

	public System.Action<PuzzleEntitySolid> onSpawnComplete;

	private Animator mAnim;
	private Rigidbody2D mBody;
	private Collider2D mColl;

	private PuzzleEntityState mState;

	private Coroutine mRout;

	public void Action(float t) {
		if(state == PuzzleEntityState.Idle) {
			ClearRout();

			mRout = StartCoroutine(DoAction(t));
		}
	}

	public void RespawnIfAction() {
		if(mState != PuzzleEntityState.Action)
			return;

		ClearRout();

		mRout = StartCoroutine(DoSpawn());
	}

	public void Respawn() {
		ClearRout();

		mRout = StartCoroutine(DoSpawn());
	}

	void OnDisable() {
		ClearRout();

		mState = PuzzleEntityState.None;
		ApplyState();
	}

	void OnDestroy() {
		if(GameData.isInstantiated)
			GameData.instance.signalPlayEnd.callback -= OnPlayEnd;
	}

	void Awake() {
		GameData.instance.signalPlayEnd.callback += OnPlayEnd;

		mAnim = GetComponent<Animator>();
		mBody = GetComponent<Rigidbody2D>();
		mColl = GetComponentInChildren<Collider2D>();

		mState = PuzzleEntityState.None;
		ApplyState();

		ApplyAttachRoot();
	}

	void Start() {
		mRout = StartCoroutine(DoSpawn());
	}

	void Update() {
		if(mState == PuzzleEntityState.Spawn || mState == PuzzleEntityState.Idle)
			ApplyAttachRoot();
	}

	private void ApplyState() {
		if(mAnim) {
			switch(mState) {
				case PuzzleEntityState.Spawn:
					animSpawn.Set(mAnim);
					break;

				case PuzzleEntityState.Despawn:
					animDespawn.Set(mAnim);
					break;

				case PuzzleEntityState.Action:
					animAction.Set(mAnim);
					break;

				case PuzzleEntityState.Idle:
					animIdle.Set(mAnim);
					break;

				case PuzzleEntityState.Victory:
					animVictory.Set(mAnim);
					break;
			}
		}

		if(mState == PuzzleEntityState.Action) {
			mBody.velocity = Vector2.zero;
			mBody.angularVelocity = 0f;

			mBody.simulated = true;
		}
		else {
			mBody.simulated = false;
		}
	}

	private void ClearRout() {
		if(mRout != null) {
			StopCoroutine(mRout);
			mRout = null;
		}
	}

	private void ApplyAttachRoot() {
		if(attachRoot) {
			Vector2 pos = attachRoot.position;
			transform.position = pos;

			transform.up = attachRoot.up;
		}
	}

	IEnumerator DoSpawn() {
		if(state != PuzzleEntityState.None || state != PuzzleEntityState.Despawn) {
			state = PuzzleEntityState.Despawn;

			yield return null;

			if(mAnim)
				yield return M8.AnimatorUtil.WaitNextState(mAnim);
		}

		state = PuzzleEntityState.Spawn;

		yield return null;

		if(mAnim)
			yield return M8.AnimatorUtil.WaitNextState(mAnim);

		state = PuzzleEntityState.Idle;

		mRout = null;

		onSpawnComplete?.Invoke(this);
	}

	IEnumerator DoAction(float actionT) {
		state = PuzzleEntityState.Action;

		Vector2 dir = transform.up;

		var impulse = actionMoveImpulseRange.Lerp(actionT);
		var force = actionMoveForceRange.Lerp(actionT);

		if(impulse > 0f)
			mBody.AddForce(dir * impulse, ForceMode2D.Impulse);

		var curTime = 0f;

		if(force > 0f) {			
			while(curTime < actionMoveForceDuration) {
				yield return null;

				mBody.AddForce(dir * force, ForceMode2D.Force);

				curTime += Time.deltaTime;
			}
		}

		if(actionEndEnabled) {
			//wait for velocity to lower and we are touching ground
			var spdThresholdSqr = actionEndSpeedThreshold * actionEndSpeedThreshold;

			curTime = 0f;
			while(curTime < actionEndDelay) {
				yield return null;

				if(mBody.velocity.sqrMagnitude <= spdThresholdSqr && mBody.IsTouchingLayers())
					curTime += Time.deltaTime;
				else
					curTime = 0f;
			}

			mRout = StartCoroutine(DoSpawn());
		}
		else
			mRout = null;
	}

	void OnPlayEnd() {
		ClearRout();

		state = PuzzleEntityState.Victory;
	}
}
