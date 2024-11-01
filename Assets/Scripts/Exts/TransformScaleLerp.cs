using UnityEngine;

public class TransformScaleLerp : MonoBehaviour {
	public Transform target;
	public Vector3 scaleStart = Vector3.one;
	public Vector3 scaleEnd = Vector3.one;
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
			target.localScale = Vector3.Lerp(scaleStart, scaleEnd, _time);
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
