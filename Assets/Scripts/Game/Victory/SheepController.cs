using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour {
	public const float boundExt = 0.9375f;
	public const float boundSize = boundExt * 2f;

	public enum Action {
		None = -1,

		Sleep,
		Wake,
		Idle,
		Victory
	}

	public enum Side {
		Right,
		Left		
	}

	[Header("Display")]
	[SerializeField]
	SpriteRenderer _bodyRender;

	[SerializeField]
	Side _bodySide = Side.Right;

	[Header("Action Config")]
	[SerializeField]
	Action _actionOnEnable;

	[SerializeField]
	M8.RangeFloat _sleepStartDelay;
	[SerializeField]
	M8.RangeFloat _wakeStartDelay;
	[SerializeField]
	M8.RangeFloat _idleStartDelay;

	[Header("Ground Config")]
	[SerializeField]
	bool _isGrounded;

	[SerializeField]
	LayerMask _groundCheckLayer;

	[Header("Move Config")]
	[SerializeField]
	float _moveSpeed = 1f;

	[Header("Animation")]
	[SerializeField]
	M8.AnimatorParamTrigger _animatorTriggerSleep;
	[SerializeField]
	M8.AnimatorParamTrigger _animatorTriggerWake;
	[SerializeField]
	M8.AnimatorParamTrigger _animatorTriggerIdle;	
	[SerializeField]
	M8.AnimatorParamTrigger _animatorTriggerMove;
	[SerializeField]
	M8.AnimatorParamTrigger _animatorTriggerVictory;

	public Animator animator { get { return mAnim; } }

	public Vector2 position {
		get { return transform.position; }
		set {
			var pos = transform.position;

			if(_isGrounded) {
				if(pos.x != value.x)
					ApplyPositionGrounded(value.x);
			}
			else if(pos.x != value.x || pos.y != value.y) {
				pos.x = value.x;
				pos.y = value.y;

				transform.position = pos;
			}
		}
	}

	public bool isOffscreen {
		get {
			var cam2D = M8.Camera2D.main;
			var screenExt = cam2D.screenExtent;

			var pos = position;

			return pos.x + boundSize < screenExt.min.x || pos.x - boundSize > screenExt.max.x || pos.y + boundSize < screenExt.min.y || pos.y - boundSize > screenExt.max.y;
		}
	}

	public Side side {
		get { return _bodySide; }
		set {
			if(_bodySide != value) {
				_bodySide = value;
				ApplySide();
			}
		}
	}

	public bool isBusy { get { return mRout != null; } }

	private Animator mAnim;
	private Coroutine mRout;

	public void PerformAction(Action act) {
		StopRout();

		switch(act) {
			case Action.Idle:
				mRout = StartCoroutine(DoAnimatorDelayedTrigger(_animatorTriggerIdle, _idleStartDelay.random));
				break;

			case Action.Sleep:
				mRout = StartCoroutine(DoAnimatorDelayedTrigger(_animatorTriggerSleep, _sleepStartDelay.random));
				break;

			case Action.Wake:
				mRout = StartCoroutine(DoAnimatorDelayedTrigger(_animatorTriggerWake, _wakeStartDelay.random));
				break;

			case Action.Victory:
				if(mAnim)
					_animatorTriggerVictory.Set(mAnim);
				break;
		}
	}

	public void MoveOffscreen(Side screenSide) {
		StopRout();

		mRout = StartCoroutine(DoMoveOffscreen(screenSide));
	}

	void OnDisable() {
		StopRout();
	}

	void OnEnable() {
		if(_isGrounded)
			ApplyPositionGrounded(position.x);

		if(_actionOnEnable != Action.None)
			PerformAction(_actionOnEnable);
	}

	void Awake() {
		mAnim = GetComponent<Animator>();
	}

	void OnDidApplyAnimationProperties() {

		if(_isGrounded)
			ApplyPositionGrounded(position.x);

		ApplySide();
	}

	IEnumerator DoAnimatorDelayedTrigger(M8.AnimatorParamTrigger trigger, float delay) {
		yield return new WaitForSeconds(delay);

		if(mAnim)
			trigger.Set(mAnim);

		mRout = null;
	}

	IEnumerator DoMoveOffscreen(Side screenSide) {
		var cam2D = M8.Camera2D.main;
		var screenExt = cam2D.screenExtent;

		var pos = position;

		if(mAnim)
			_animatorTriggerMove.Set(mAnim);

		if(screenSide == Side.Right) {
			side = Side.Right;

			yield return M8.AnimatorUtil.WaitNextState(mAnim);

			//assume grounded, so just update x
			while(pos.x - boundSize < screenExt.max.x) {
				yield return null;

				pos.x += _moveSpeed * Time.deltaTime;

				position = pos;
			}
		}
		else {
			side = Side.Left;

			yield return M8.AnimatorUtil.WaitNextState(mAnim);

			//assume grounded, so just update x
			while(pos.x + boundSize > screenExt.min.x) {
				yield return null;

				pos.x -= _moveSpeed * Time.deltaTime;

				position = pos;
			}
		}
		
		mRout = null;
	}

	private void StopRout() {
		if(mRout != null) {
			StopCoroutine(mRout);
			mRout = null;
		}
	}

	private void ApplyPositionGrounded(float x) {
		var cam2D = M8.Camera2D.main;
		var screenExt = cam2D.screenExtent;

		var checkPoint = new Vector2(x, screenExt.max.y);
		var checkDir = Vector2.down;

		var hit = Physics2D.Raycast(checkPoint, checkDir, screenExt.size.y, _groundCheckLayer);

		var pos = transform.position;

		if(hit.collider) {
			pos.x = hit.point.x;
			pos.y = hit.point.y + boundExt;
		}
		else
			pos.x = x;

		transform.position = pos;
	}

	private void ApplySide() {
		if(_bodyRender) {
			switch(_bodySide) {
				case Side.Right:
					_bodyRender.flipX = false;
					break;
				case Side.Left:
					_bodyRender.flipX = true;
					break;
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;

		Gizmos.DrawWireSphere(position, boundExt);
	}
}
