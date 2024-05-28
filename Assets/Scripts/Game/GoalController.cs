using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour {
    public GoalData data;

    public GoalState state { 
        get { return mState; }
        set {
            if(mState != value) {
                mState = value;

                stateChangedCallback?.Invoke(this);
			}
        }
    }

    public event System.Action<GoalController> stateChangedCallback;

    private GoalState mState;
}
