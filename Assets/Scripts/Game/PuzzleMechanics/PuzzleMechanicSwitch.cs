using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PuzzleMechanicSwitch : PuzzleMechanicBase {
    [System.Serializable]
    public struct SwitchInfo {
        public Transform dirRoot; //use up vector for dir

        public M8.AnimatorTargetParamBool openBool;
        public M8.AnimatorTargetParamTrigger openInitialTrigger;
		public M8.AnimatorTargetParamTrigger closeInitialTrigger;

		public string label { get { return dirRoot ? dirRoot.name : ""; } }

        public Vector2 up { get { return dirRoot ? dirRoot.up : Vector2.up; } }

        public void Initialize(bool open) {
            if(open)
                openInitialTrigger.Set();
            else
                closeInitialTrigger.Set();

            openBool.Set(open);
        }

        public void SetOpen(bool open) {
            openBool.Set(open);
        }
    }

    [Header("Switch Config")]
    public SwitchInfo[] switches;

    [SerializeField]
    int _index;

    [Header("Switch Display")]
    public Transform switchDisplayRoot;
    public float switchRotateDelay = 0.3f;

    [Header("Switch SFX")]
    [M8.SoundPlaylist]
    public string switchSFX;

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

    public void ApplyCurrentIndex() {
		StopRout();

        for(int i = 0; i < switches.Length; i++) {
            var curInf = switches[i];

            if(i == _index) {
                curInf.Initialize(false);

				switchDisplayRoot.up = curInf.up;
			}
            else {
				curInf.Initialize(true);
			}
        }
	}

	protected override void InputClick(PointerEventData eventData) {
        var prevInd = _index;

        _index = (_index + 1) % switches.Length;

        if(prevInd >= 0 && prevInd < switches.Length)
            switches[prevInd].SetOpen(true);

        switches[_index].SetOpen(false);

        if(mRout != null)
            StopCoroutine(mRout);

        mRout = StartCoroutine(DoRotate());

        if(!string.IsNullOrEmpty(switchSFX))
            M8.SoundPlaylist.instance.Play(switchSFX, false);
                
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

    IEnumerator DoRotate() {
        var curInf = switches[_index];
        var curUp = curInf.up;

        var startRot = Vector2.SignedAngle(switchDisplayRoot.up, curUp);

        var curTime = 0f;
        while(curTime < switchRotateDelay) {
            yield return null;

            curTime += Time.deltaTime;

            var t = Mathf.Sin((curTime / switchRotateDelay) * Mathf.PI * 0.5f);

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
