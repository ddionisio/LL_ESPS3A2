using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMechanicOnValueMotionRotate : MonoBehaviour {
	public enum State {
        None,
        Enter,
        Play,
        Exit
    }

    public Transform rootTarget;

    public float rotateSpeed;
    public bool isClockwise;

    public float startDelay = 0.3f;
	public float playDelay = 0.5f;
	public float endDelay = 0.3f;

	[Tooltip("Ensure this is a value mechanic.")]
    public SignalMechanic signalListenMechanicValueChanged;

    private float rotation {
        get { return rootTarget.eulerAngles.z; }
        set {
            var rots = rootTarget.eulerAngles;
            if(rots.z != value) {
                rots.z = value;
                rootTarget.eulerAngles = rots;
            }
        }
    }

    private State mState;
    private float mRotateCurTime;
    private float mRotateSign;

	void OnDisable() {
		mState = State.None;
	}

	void OnDestroy() {
		if(signalListenMechanicValueChanged)
			signalListenMechanicValueChanged.callback -= OnMechanicValueChanged;
	}

	void Awake() {
        if(signalListenMechanicValueChanged)
            signalListenMechanicValueChanged.callback += OnMechanicValueChanged;
	}

	void Update() {
		switch(mState) {
            case State.Play:
				rotation += rotateSpeed * Time.deltaTime * mRotateSign;

				mRotateCurTime += Time.deltaTime;
				if(mRotateCurTime >= playDelay)
					ApplyMotion(0f);
				break;

            case State.Enter:
                mRotateCurTime += Time.deltaTime;
				if(mRotateCurTime < startDelay) {
					float t = 1f - Mathf.Cos((mRotateCurTime / startDelay) * Mathf.PI * 0.5f);

					rotation += rotateSpeed * Time.deltaTime * t * mRotateSign;
				}
				else {
					mRotateCurTime = 0f;
					mState = State.Play;
				}
				break;

			case State.Exit:
				mRotateCurTime += Time.deltaTime;
				if(mRotateCurTime < endDelay) {
					float t = 1.0f - Mathf.Sin((mRotateCurTime / endDelay) * Mathf.PI * 0.5f);

					rotation += rotateSpeed * Time.deltaTime * t * mRotateSign;
				}
				else
					mState = State.None;
				break;
		}
	}

	void OnMechanicValueChanged(PuzzleMechanicBase mechanic) {
		var mechanicVal = mechanic as PuzzleMechanicValueBase;
		if(mechanicVal)
			ApplyMotion(mechanicVal.motionDir);
		else
			ApplyMotion(0f);
	}

    private void ApplyMotion(float motionDir) {
		if(motionDir == 0f) {
			if(mState == State.Play) {
				mRotateCurTime = 0f;
				mState = State.Exit;
			}
			else if(mState == State.Enter) {
				mRotateCurTime = Mathf.Clamp01(mRotateCurTime / startDelay) * endDelay;
				mState = State.Exit;
			}
		}
		else {
			if(mState == State.None) {
				mRotateCurTime = 0f;
				mState = State.Enter;
			}
			else if(mState == State.Play)
				mRotateCurTime = 0f;
			else if(mState == State.Exit) {
				mRotateCurTime = 1.0f - Mathf.Clamp01(mRotateCurTime / endDelay) * startDelay;
				mState = State.Enter;
			}

			mRotateSign = isClockwise ? -motionDir : motionDir;
		}
	}
}
