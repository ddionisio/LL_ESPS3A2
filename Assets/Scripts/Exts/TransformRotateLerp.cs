using UnityEngine;

public class TransformRotateLerp : MonoBehaviour {
    public Transform target;
	public Vector3 rotateStart;
	public Vector3 rotateEnd;
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
				target.localEulerAngles = Vector3.Lerp(rotateStart, rotateEnd, _time);
			else
				target.eulerAngles = Vector3.Lerp(rotateStart, rotateEnd, _time);
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
}
