using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorCheckBase : MonoBehaviour {
    public LayerMask checkMask;
    public float checkRadius;
    public float checkDelay = 0.3f;

	public bool isActive {
		get { return mIsActive; }
		set {
			if(mIsActive != value) {
				mIsActive = value;

				if(mIsActive)
					mLastTime = Time.time;
			}
		}
	}

	private bool mIsActive = true;
	private float mLastTime;

	private const int overlapCapacity = 4;
	private Collider2D[] mOverlapResults = new Collider2D[overlapCapacity];

	protected abstract void OnSensorCheck(Collider2D coll);

	void OnEnable() {
		if(mIsActive)
			mLastTime = Time.time;
	}

	void Update() {
		if(mIsActive) {
			var time = Time.time;
			if(time - mLastTime >= checkDelay) {
				int count = Physics2D.OverlapCircleNonAlloc(transform.position, checkRadius, mOverlapResults, checkMask);

				for(int i = 0; i < count; i++)
					OnSensorCheck(mOverlapResults[i]);

				mLastTime = time;
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, checkRadius);
	}
}
