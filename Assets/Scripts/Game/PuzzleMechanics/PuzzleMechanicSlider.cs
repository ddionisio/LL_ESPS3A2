using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PuzzleMechanicSlider : PuzzleMechanicBase {
	[Header("Slider Config")]
	public float length;

	[Tooltip("Set to <= 0 for no limit.")]
	public int stepCount;

	public bool isReversed;

	[Header("Slider Display")]
	public Transform handleRoot;
	public float handleMoveDelay = 0.3f;

	[Header("Value")]
	public float minValue = 0f;
	public float maxValue = 100f;
	[SerializeField]
	float _value = 0f;

	public UnityEvent<float> onValueChanged;

	public float value {
		get { return _value; }
		set {
			var v = Mathf.Clamp(value, minValue, maxValue);
			if(_value != v) {
				_value = v;

				mHandleLocalX = GetHandleLocalX();

				onValueChanged.Invoke(_value);
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

	public bool isHandleMoving { get; private set; }

	private float mHandleLocalX;
	private float mHandleLocalXCur;
	private float mHandleLocalXVel;

	public float GetHandleLocalX() {
		var hLen = length * 0.5f;

		var t = isReversed ? 1.0f - valueScalar : valueScalar;

		if(stepCount > 0)
			t = Mathf.Round(stepCount * t) / stepCount;

		return Mathf.Lerp(-hLen, hLen, t);
	}

	protected override void InputDrag(PointerEventData eventData) {
		var pos = GetWorldPos(eventData);
		UpdateValueFromPosition(pos);
	}

	protected override void InputDragEnd(PointerEventData eventData) {
		if(eventData != null) {
			var pos = GetWorldPos(eventData);
			UpdateValueFromPosition(pos);
		}
	}

	protected override void OnEnable() {
		base.OnEnable();

		mHandleLocalXCur = mHandleLocalX = GetHandleLocalX();
		mHandleLocalXVel = 0f;

		ApplyHandleTransform();
	}

	void Update() {
		isHandleMoving = mHandleLocalXCur != mHandleLocalX;
		if(isHandleMoving) {
			mHandleLocalXCur = Mathf.SmoothDamp(mHandleLocalXCur, mHandleLocalX, ref mHandleLocalXVel, handleMoveDelay);
			ApplyHandleTransform();
		}
	}

	private Vector2 GetWorldPos(PointerEventData eventData) {
		Vector2 pos;

		//TODO: use main camera for now
		var cam = Camera.main;
		pos = cam.ScreenToWorldPoint(eventData.position);

		return pos;
	}

	private void UpdateValueFromPosition(Vector2 pos) {
		var hLen = length * 0.5f;

		var lpos = transform.InverseTransformPoint(pos);

		var t = (lpos.x + hLen) / length;

		valueScalar = isReversed ? 1.0f - t : t;
	}

	private void ApplyHandleTransform() {
		if(handleRoot) {
			handleRoot.position = transform.TransformPoint(new Vector3(mHandleLocalXCur, 0f, 0f));
			handleRoot.rotation = transform.rotation;
		}
	}

	void OnDrawGizmos() {
		var hLen = length * 0.5f;

		var sPos = new Vector3(-hLen, 0f, 0f);
		var ePos = new Vector3(hLen, 0f, 0f);

		Gizmos.color = Color.yellow;

		Gizmos.DrawLine(transform.TransformPoint(sPos), transform.TransformPoint(ePos));

		//draw steps
		for(int i = 1; i <= stepCount - 1; i++) {
			float t = (float)i / stepCount;
			M8.Gizmo.DrawStepLineY(transform, Vector3.Lerp(sPos, ePos, t), 0.4f);
		}

		Gizmos.color = isReversed ? Color.red : Color.green;
		M8.Gizmo.DrawStepLineY(transform, sPos, 0.5f);

		Gizmos.color = isReversed ? Color.green : Color.red;
		M8.Gizmo.DrawStepLineY(transform, ePos, 0.5f);
	}
}
