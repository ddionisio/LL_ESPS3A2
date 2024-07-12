using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PuzzleMechanicValueBase : PuzzleMechanicBase {
	[Header("Value")]
	public float minValue = 0f;
	public float maxValue = 100f;
	[SerializeField]
	float _value = 0f;

	public UnityEvent<float> onValueChanged;
	public SignalMechanic signalInvokeValueChanged;

	public float value {
		get { return _value; }
		set {
			var v = Mathf.Clamp(value, minValue, maxValue);
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
			var delta = maxValue - minValue;
			return delta > 0f ? Mathf.Clamp01((value - minValue) / delta) : 0f;
		}

		set {
			this.value = Mathf.Lerp(minValue, maxValue, Mathf.Clamp01(value));
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
