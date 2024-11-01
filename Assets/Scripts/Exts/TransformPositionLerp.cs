using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPositionLerp : MonoBehaviour {
	public Transform target;
	public Vector3 positionStart;
	public Vector3 positionEnd;
	public bool isLocal = true;
	[SerializeField]
	[Range(0f, 1f)]
	float _time;

	public float time {
		get { return _time; }
		set {
			SetTime(value);
		}
	}

	public void SetTime(float t) {
		var val = Mathf.Clamp01(t);
		if(_time != val) {
			_time = val;
			Refresh();
		}
	}

	public void Refresh() {
		if(target) {
			if(isLocal)
				target.localPosition = Vector3.Lerp(positionStart, positionEnd, _time);
			else
				target.position = Vector3.Lerp(positionStart, positionEnd, _time);
		}
	}

	void OnDidApplyAnimationProperties() {
		Refresh();
	}

	void OnEnable() {
		Refresh();
	}

	void Awake() {
		if(!target)
			target = transform;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;

		var trans = target ? target : transform;
		var parent = trans.parent;

		var pos1 = isLocal && parent ? parent.TransformPoint(positionStart) : positionStart;
		var pos2 = isLocal && parent ? parent.TransformPoint(positionEnd) : positionEnd;

		Gizmos.DrawSphere(pos1, 0.1f);
		Gizmos.DrawSphere(pos2, 0.1f);

		Gizmos.color = Color.yellow * 0.5f;

		Gizmos.DrawLine(pos1, pos2);
	}
}
