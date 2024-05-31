using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour {
    public GoalState state { 
        get { return mState; }
        set {
            if(mState != value) {
                mState = value;

                stateChangedCallback?.Invoke(this);

				if(!mIsInterfacesInit)
					InitInterfaces();

				for(int i = 0; i < mIGoalStateChanged.Length; i++)
					mIGoalStateChanged[i].GoalStateChanged(mState);
			}
        }
    }

    public event System.Action<GoalController> stateChangedCallback;

    private bool mIsInterfacesInit;
    private IGoalStateChanged[] mIGoalStateChanged;

	private GoalState mState;

    void Awake() {
		if(!mIsInterfacesInit)
			InitInterfaces();
	}

    void InitInterfaces() {
		var comps = GetComponentsInChildren<MonoBehaviour>(true);

		var iGoalStateChangedList = new List<IGoalStateChanged>();

		for(int i = 0; i < comps.Length; i++) {
			var comp = comps[i];

			var stateChanged = comp as IGoalStateChanged;
			if(stateChanged != null)
				iGoalStateChangedList.Add(stateChanged);
		}

		mIGoalStateChanged = iGoalStateChangedList.ToArray();

		mIsInterfacesInit = true;
	}
}
