using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalStateSpriteColor : MonoBehaviour, IGoalStateChanged {
    public SpriteRenderer target;

    public Color inactiveColor = Color.red;
    public Color activeColor = Color.green;
    public float delay = 0.3f;
    public DG.Tweening.Ease ease = DG.Tweening.Ease.OutSine;

    private bool mIsActive;
    private bool mIsTweening;
    private float mCurTime;

    private DG.Tweening.EaseFunction mEaseFunc;

    void OnEnable() {
        if(target) {
            if(mIsTweening) {
                target.color = mIsActive ? activeColor : inactiveColor;
                mIsTweening = false;
            }
        }
    }

    void Awake() {
        if(!target)
            target = GetComponent<SpriteRenderer>();

        if(target)
            target.color = inactiveColor;

		mEaseFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(ease);
	}

    void Update() {
        if(mIsTweening) {
            mCurTime += Time.deltaTime;

            var t = mEaseFunc(mCurTime, delay, 0f, 0f);

            if(mIsActive)
                target.color = Color.Lerp(inactiveColor, activeColor, t);
            else
				target.color = Color.Lerp(activeColor, inactiveColor, t);

            if(mCurTime >= delay)
                mIsTweening = false;
		}
    }

    void IGoalStateChanged.GoalStateChanged(GoalState state) {
        var newActive = state == GoalState.Active;

        if(mIsActive != newActive) {
            mIsActive = newActive;

			if(target) {
                if(mIsTweening) {
                    mCurTime = delay - mCurTime;
                    if(mCurTime < 0f)
                        mCurTime = 0f;
                }
                else {
                    mIsTweening = true;
                    mCurTime = 0f;
                }
            }
        }
    }
}
