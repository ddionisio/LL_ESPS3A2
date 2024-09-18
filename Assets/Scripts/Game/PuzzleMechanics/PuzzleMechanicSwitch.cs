using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PuzzleMechanicSwitch : PuzzleMechanicBase {
    [System.Serializable]
    public struct SwitchInfo {
        public Transform dirRoot; //use up vector for dir

        public string label { get { return dirRoot ? dirRoot.name : ""; } }

        public Vector2 up { get { return dirRoot ? dirRoot.up : Vector2.up; } }
    }

    [Header("Switch Config")]
    public SwitchInfo[] switches;

    [SerializeField]
    int _index;

    [Header("Switch Display")]
    public Transform switchDisplayRoot;
    public float switchRotateDelay = 0.3f;
    public DG.Tweening.Ease switchRotateEase = DG.Tweening.Ease.OutSine;

    [Header("Switch Callbacks")]
    public M8.Signal signalInvokeSwitch;
	public UnityEvent<int> onSwitchIndex;
	public UnityEvent<string> onSwitchLabel;

	public int index { get { return _index; } }

    public string indexLabel {
        get {
            return _index >= 0 && _index < switches.Length ? switches[_index].label : "";
        }
    }

    public bool isBusy { get { return mRout != null; } }

    private Coroutine mRout;
    private DG.Tweening.EaseFunction mEaseFunc;

    public void ApplyCurrentIndex() {
		StopRout();

        if(_index >= 0 && _index < switches.Length) {
			var curInf = switches[_index];

            switchDisplayRoot.up = curInf.up;
		}
	}

	protected override void InputClick(PointerEventData eventData) {
        _index = (_index + 1) % switches.Length;

        if(mRout != null)
            StopCoroutine(mRout);

        mRout = StartCoroutine(DoRotate());
                
		onSwitchIndex.Invoke(_index);
        onSwitchLabel.Invoke(indexLabel);

		signalInvokeSwitch?.Invoke();
	}

	protected override void OnEnable() {
		base.OnEnable();

        ApplyCurrentIndex();
	}

	protected override void OnDisable() {
		base.OnDisable();

        StopRout();
	}

	protected override void Awake() {
		base.Awake();

        mEaseFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(switchRotateEase);
	}

    IEnumerator DoRotate() {
        var curInf = switches[_index];
        var curUp = curInf.up;

        var startRot = Vector2.SignedAngle(switchDisplayRoot.up, curUp);

        var curTime = 0f;
        while(curTime < switchRotateDelay) {
            yield return null;

            curTime += Time.deltaTime;

            var t = mEaseFunc(curTime, switchRotateDelay, 0f, 0f);

            var rot = Mathf.Lerp(startRot, 0f, t);

            switchDisplayRoot.up = M8.MathUtil.RotateAngle(curUp, rot);
        }

        mRout = null;
    }

    void StopRout() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }
}
