using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleEntityState {
	None,

	Spawn,
	Despawn,

	Idle,
	Action,

	Victory,
}

public interface IPuzzleEntityStateBegin {
    void OnStateBegin(PuzzleEntityState state);
}

/// <summary>
/// Update state, return true if complete.
/// </summary>
public interface IPuzzleEntityStateUpdate {
	bool OnStateUpdate(PuzzleEntityState state);
}

public interface IPuzzleEntityStateEnd {
	void OnStateEnd(PuzzleEntityState state);
}


public class PuzzleEntity : MonoBehaviour {
    public PuzzleEntityState initialState = PuzzleEntityState.None;

    public PuzzleEntityState state {
        get { return mState; }
        set {
			mToState = value; //allow one update cycle before changing
		}
    }

    public bool stateIsComplete { get { return mStateIsComplete && mToState == null; } }

	public Vector2 position { get { return transform.position; } set { transform.position = value; } }

	public float rotation { get { return Vector2.SignedAngle(Vector2.up, transform.up); } set { transform.up = M8.MathUtil.Rotate(Vector2.up, value); } }

	public event System.Action<PuzzleEntityState> onStateChanged;
	public event System.Action<PuzzleEntityState> onStateBegin;
	public event System.Action<PuzzleEntityState> onStateEnd;

	private PuzzleEntityState? mToState;
	private PuzzleEntityState mState;

	private bool mStateIsComplete = false;

    private IPuzzleEntityStateBegin[] mIStateBegins;
	private IPuzzleEntityStateUpdate[] mIStateUpdates;
	private IPuzzleEntityStateEnd[] mIStateEnds;

	void Awake() {
        //grab interfaces
        var behaviours = GetComponentsInChildren<MonoBehaviour>(true);

        var stateBeginList = new List<IPuzzleEntityStateBegin>();
        var stateEndList = new List<IPuzzleEntityStateEnd>();
		var stateUpdateList = new List<IPuzzleEntityStateUpdate>();

		for(int i = 0; i < behaviours.Length; i++) {
            var behaviour = behaviours[i];

            var stateBegin = behaviour as IPuzzleEntityStateBegin;
            if(stateBegin != null)
                stateBeginList.Add(stateBegin);

            var stateEnd = behaviour as IPuzzleEntityStateEnd;
            if(stateEnd != null)
				stateEndList.Add(stateEnd);

            var stateUpdate = behaviour as IPuzzleEntityStateUpdate;
            if(stateUpdate != null)
                stateUpdateList.Add(stateUpdate);
		}

        mIStateBegins = stateBeginList.ToArray();
        mIStateUpdates = stateUpdateList.ToArray();
        mIStateEnds = stateEndList.ToArray();
	}

	void Start() {
        mState = initialState;

        StateBegin();
	}

	void Update() {
        if(mToState.HasValue) {
			var toState = mToState.Value;
			mToState = null;

			if(mState != toState) {
				//end previous state if it is not complete
				if(!stateIsComplete)
					StateEnd();

				mState = toState;
				mStateIsComplete = false;

				StateBegin();

				onStateChanged?.Invoke(mState);
			}
		}

		if(!stateIsComplete) {
            var updateCompleteCount = 0;

            for(int i = 0; i < mIStateUpdates.Length; i++) {
                if(mIStateUpdates[i].OnStateUpdate(mState))
                    updateCompleteCount++;
            }

            if(updateCompleteCount == mIStateUpdates.Length) {
				mStateIsComplete = true;
                StateEnd();
			}
		}
	}

    private void StateBegin() {
		for(int i = 0; i < mIStateBegins.Length; i++)
			mIStateBegins[i].OnStateBegin(mState);

        onStateBegin?.Invoke(mState);
	}

    private void StateEnd() {
		for(int i = 0; i < mIStateEnds.Length; i++)
			mIStateEnds[i].OnStateEnd(mState);

		onStateEnd?.Invoke(mState);
	}
}
