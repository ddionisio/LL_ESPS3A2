using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntityAddForce : MonoBehaviour, IPuzzleEntityStateBegin, IPuzzleEntityStateUpdate {
	public enum State {
		None,
		Impulse,
		Force
	}

	public Rigidbody2D body;

	public float dirAngle;

	public float impulse;
	public float force;
	public float duration;	

	public PuzzleEntityState state;

	private State mForceState;

	private float mForceCurTime;

	private Vector2 mDir;

	void IPuzzleEntityStateBegin.OnStateBegin(PuzzleEntityState state) {
		if(state == this.state) {
			if(impulse != 0f)
				mForceState = State.Impulse;
			else if(force != 0f)
				mForceState = State.Force;

			mForceCurTime = 0f;

			mDir = M8.MathUtil.RotateAngle(Vector2.up, dirAngle);
		}
		else
			mForceState = State.None;
	}

	bool IPuzzleEntityStateUpdate.OnStateUpdate(PuzzleEntityState state) {
		var ret = false;

		switch(mForceState) {
			case State.Impulse:
				body.AddForce(mDir * impulse, ForceMode2D.Impulse);

				if(force != 0f)
					mForceState = State.Force;
				break;

			case State.Force:
				body.AddForce(mDir * force, ForceMode2D.Force);

				mForceCurTime += Time.deltaTime;
				ret = mForceCurTime >= duration;
				break;

			default:
				ret = true;
				break;
		}

		return ret;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;

		var dir = M8.MathUtil.RotateAngle(Vector2.up, dirAngle);
		M8.Gizmo.Arrow(transform.position, dir);
	}
}
