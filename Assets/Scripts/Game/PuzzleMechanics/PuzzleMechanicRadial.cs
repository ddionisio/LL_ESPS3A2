using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleMechanicRadial : PuzzleMechanicBase {
    [Header("Radial Config")]
	public float radius;

	public float angleStart; //starting from top

    public M8.RangeFloat angleRange = new M8.RangeFloat(0f, 360f);

    [Tooltip("Set to <= 0 for no limit.")]
    public int angleCount;

    [Header("Rotator Display")]
    public Transform rotatorRoot;
	public bool rotatorAttachToRadius; //set rotate root on radius?
    public float rotatorRotateDelay = 0.3f;

	[Header("Value")]
	public float minValue = 0f;
	public float maxValue = 100f;
	[SerializeField]
	float _value = 0f;
	[SerializeField]
	Slider.SliderEvent _onValueChanged;

	public float value {
		get { return _value; }
		set {
			var v = Mathf.Clamp(value, minValue, maxValue);
			if(_value != v) {
				_value = v;
				UpdateCurDirFromValue();
				UpdateRotator();

				_onValueChanged.Invoke(_value);
			}
		}
	}

	public float valueScalar {
		get {
			var delta = maxValue - minValue;
			return delta > 0f ? Mathf.Clamp01((value - minValue) / delta) : 0f;
		}

		set {
			this.value = Mathf.Lerp(minValue, maxValue, Mathf.Clamp01(value));
		}
	}

	public Vector2 dir { get; private set; }

	/// <summary>
	/// Angle in [0, 360] within angleRange, relative to angleStart
	/// </summary>
	public float angle { get { return AngleAbs(angleRange.Lerp(valueScalar)); } }

	public Slider.SliderEvent onValueChanged { get { return _onValueChanged; } }

	private Vector2 mInitDir;

	//for display purpose
	private Vector2 mRotatorDir;

	private float mRotatorAngle;
	private float mRotatorAngleVel;

	//protected override void InputDragBegin(PointerEventData eventData) { 
    //}

	protected override void InputDrag(PointerEventData eventData) {
		var pos = GetWorldPos(eventData);
		UpdateCurDirAndValueFromPosition(pos);
	}

	protected override void InputDragEnd(PointerEventData eventData) {
		if(eventData != null) {
			var pos = GetWorldPos(eventData);
			UpdateCurDirAndValueFromPosition(pos);
		}
	}

	protected override void Awake() {
		base.Awake();

		mInitDir = M8.MathUtil.RotateAngle(Vector2.up, angleStart);

		UpdateCurDirFromValue();

		mRotatorDir = dir;
		mRotatorAngle = 0f;
		mRotatorAngleVel = 0f;

		ApplyRotatorTransform();
	}

	void Update() {
		if(mRotatorAngle != 0f) {
			mRotatorAngle = Mathf.SmoothDampAngle(mRotatorAngle, 0f, ref mRotatorAngleVel, rotatorRotateDelay);

			mRotatorDir = M8.MathUtil.RotateAngle(dir, mRotatorAngle);

			ApplyRotatorTransform();
		}
	}

	private Vector2 GetWorldPos(PointerEventData eventData) {
		Vector2 pos;

		//TODO: use main camera for now
		var cam = Camera.main;
		pos = cam.ScreenToWorldPoint(eventData.position);

		return pos;
	}

	private void UpdateCurDirAndValueFromPosition(Vector2 pos) {
		var delta = pos - (Vector2)transform.position;
		var norm = delta.normalized;

		var startAngleAbs = AngleAbs(angleRange.min);
		var endAngleAbs = AngleAbs(angleRange.max);

		var angleOfs = Vector2.SignedAngle(norm, mInitDir);
		angleOfs = AngleAbs(angleOfs);

		float curAngle;
		if(startAngleAbs < endAngleAbs)
			curAngle = angleOfs - startAngleAbs;
		else
			curAngle = angleOfs - endAngleAbs;
				
		//set value via scalar
		float angleLen = Mathf.Abs(endAngleAbs - startAngleAbs);
		if(angleLen > 0f) {
			//round up angle
			if(angleCount > 0) {
				var t = Mathf.Clamp01(curAngle / angleLen);
				curAngle = (Mathf.Round(angleCount * t) / angleCount) * angleLen;
			}

			valueScalar = curAngle / angleLen;
		}
	}

	private float AngleAbs(float a) {
		if(a == 360f) return a;

		float _a = a % 360f;
		if(_a < 0f)
			_a = 360f + _a;
		return _a;
	}

	private void UpdateCurDirFromValue() {
		dir = M8.MathUtil.RotateAngle(mInitDir, angleRange.Lerp(valueScalar));
		dir.Normalize();
	}

	private void UpdateRotator() {
		if(rotatorRotateDelay > 0f) {
			mRotatorAngle = Vector2.SignedAngle(mRotatorDir, dir);
		}
		else {
			mRotatorDir = dir;
			ApplyRotatorTransform();
		}
	}

	private void ApplyRotatorTransform() {
        if(rotatorRoot) {
            if(rotatorAttachToRadius && radius > 0f)
                rotatorRoot.localPosition = mRotatorDir * radius;

            rotatorRoot.up = mRotatorDir;
		}
    }

	void OnDrawGizmos() {
		var pos = (Vector2)transform.position;

		if(radius > 0f) {
			Gizmos.color = Color.yellow;

			Gizmos.DrawWireSphere(pos, radius);

            var dir = M8.MathUtil.RotateAngle(Vector2.up, angleStart);
			var startDir = M8.MathUtil.RotateAngle(dir, angleRange.min);
			var endDir = M8.MathUtil.RotateAngle(dir, angleRange.max);

			//draw angle counts
            if(angleCount > 0) {
				Gizmos.color = Color.yellow * 0.3f;

				var startAngleAbs = AngleAbs(angleRange.min);
				var endAngleAbs = AngleAbs(angleRange.max);
				float angleLen = Mathf.Abs(endAngleAbs - startAngleAbs);

				var angleDelta = angleLen / angleCount;

                for(int i = 0; i < angleCount; i++) {
					var _dir = M8.MathUtil.RotateAngle(startDir, angleDelta * i);

					Gizmos.DrawLine(pos, pos + _dir * radius);
				}
            }

			//draw begin range
			Gizmos.color = Color.green;
			Gizmos.DrawLine(pos, pos + startDir * radius);

			//draw end range
			Gizmos.color = Color.red;
			Gizmos.DrawLine(pos, pos + endDir * radius);
		}
	}
}
