using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PuzzleMechanicValueBase : PuzzleMechanicBase {
	[Header("Value")]
	[SerializeField]
	float _minValue = 0f;
	[SerializeField]
	float _maxValue = 100f;
	[Tooltip("Set to <= 0 for no limit.")]
	[SerializeField]
	int _stepCount;
	[SerializeField]
	float _value = 0f;
		
	public UnityEvent<float> onValueChanged;
	public SignalMechanic signalInvokeValueChanged;

	public float minValue { get { return _minValue; } }
	public float maxValue { get { return _maxValue; } }
	public int stepCount { get { return _stepCount; } }

	public float value {
		get { return _value; }
		set {
			var v = Mathf.Clamp(value, _minValue, _maxValue);
			if(_value != v) {
				_value = v;

				ValueRefresh();

				onValueChanged.Invoke(_value);
				signalInvokeValueChanged?.Invoke(this);
			}
		}
	}

	public float valueScalar {
		get {
			var delta = _maxValue - _minValue;
			return delta > 0f ? Mathf.Clamp01((value - _minValue) / delta) : 0f;
		}

		set {
			this.value = Mathf.Lerp(_minValue, _maxValue, Mathf.Clamp01(value));
		}
	}

	/// <summary>
	/// The motion 'dir' of the mechanic when value is changed. Returns 0 when it has stopped moving.
	/// </summary>
	public virtual float motionDir { get  { return 0f; } }

	protected abstract void ValueRefresh();

	protected override void OnEnable() {
		base.OnEnable();

		//invoke current value
		onValueChanged.Invoke(_value);
		signalInvokeValueChanged?.Invoke(this);
	}
}
