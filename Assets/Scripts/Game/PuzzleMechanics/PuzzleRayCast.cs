using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRayCast : MonoBehaviour {
    public CastData castData;

	public bool castInitialActive;

	public GameObject castActiveGO;
    public Transform castDirRoot;
    public Transform castPointRoot;
        
    public float castRadius; //set to <= 0 as line
    public float castLength;
    public LayerMask castLayerMask;

    public bool castActive {
        get { return mCastActive; }
        set {
            if(mCastActive != value) {
                mCastActive = value;
                ApplyActive();
			}
        }
    }

    public Vector2 castDir {
        get {
            if(!castDirRoot) castDirRoot = transform;

			return castDirRoot.up;
        }

        set {
			if(!castDirRoot) castDirRoot = transform;

			Vector2 _up = castDirRoot.up;
			if(_up != value) {
				castDirRoot.up = value;
			}
		}
    }

    public float castDistance { get; private set; }
	public RaycastHit2D castHit { get; private set; }

    public Vector2 castPoint {
        get { return castPointRoot ? castPointRoot.position : position; }

        private set {
            if(castPointRoot) {
                Vector2 pos = castPointRoot.position;
                if(pos != value)                    
                    castPointRoot.position = new Vector3(value.x, value.y, castPointRoot.position.z);
            }
        }
    }

	public Vector2 position { get { return transform.position; } }

    public PuzzleRayCastTarget target {
        get { return mTarget; }

        private set {
            if(mTarget != value) {
                if(mTarget)
                    mTarget.ApplyCast(null);

				mTarget = value;

                if(mTarget)
					mTarget.ApplyCast(this);

                mCurUpdateTime = 0f;
			}
        }
    }

    private bool mCastActive;
    private PuzzleRayCastTarget mTarget;
    private float mCurUpdateTime;

	void OnEnable() {
		ApplyActive();
	}

	void Awake() {
		mCastActive = castInitialActive;
	}

	void Update() {
        if(mCastActive) {
            mCurUpdateTime += Time.deltaTime;
            if(mCurUpdateTime >= GameData.instance.mechanicRayCastUpdateDelay) {
                mCurUpdateTime = 0f;

                Cast();

                if(mTarget)
                    mTarget.UpdateCast(this);
			}
        }
	}

	private void Cast() {
        if(castRadius > 0f)
            castHit = Physics2D.CircleCast(position, castRadius, castDir, castLength, castLayerMask);
        else
            castHit = Physics2D.Raycast(position, castDir, castLength, castLayerMask);

        castDistance = castHit.distance;

        var hitColl = castHit.collider;

		if(hitColl) {
            if(!(target && target.coll == hitColl)) {
                var newTarget = hitColl.GetComponent<PuzzleRayCastTarget>();
                if(newTarget.castData == castData)
                    target = newTarget;
            }

            castPoint = castHit.point;
		}
        else {
            target = null;

			castPoint = position + castDir * castLength;
        }
    }

    private void ApplyActive() {
        if(castActiveGO) castActiveGO.SetActive(mCastActive);

		target = null;
	}

	void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        var _castDirRoot = castDirRoot ? castDirRoot : transform;

        Vector2 _up = _castDirRoot.up;
                
		M8.Gizmo.ArrowLine2D(position, position + _up);

		Gizmos.DrawWireSphere(position, castRadius);

		if(castPointRoot)
            Gizmos.DrawSphere(castPointRoot.position, 0.1f);
	}
}
