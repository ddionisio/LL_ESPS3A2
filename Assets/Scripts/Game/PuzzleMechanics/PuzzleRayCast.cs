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

    private const int castCapacity = 4;

    private RaycastHit2D[] mCastHits = new RaycastHit2D[castCapacity];

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
        int castCount;

		if(castRadius > 0f)
			castCount = Physics2D.CircleCastNonAlloc(position, castRadius, castDir, mCastHits, castLength, castLayerMask);
		else
			castCount = Physics2D.RaycastNonAlloc(position, castDir, mCastHits, castLength, castLayerMask);

		PuzzleRayCastTarget nearestTarget = null;
		castHit = new RaycastHit2D();

		if(castCount > 0) {
			for(int i = 0; i < castCount; i++) {
                var hit = mCastHits[i];

                var hitColl = hit.collider;
                if(!hitColl)
                    continue;

                //ensure hit is not in our hierarchy
                if(hitColl.gameObject == gameObject || M8.Util.IsParentOf(transform, hitColl.transform) || M8.Util.IsParentOf(hitColl.transform, transform))
                    continue;

				var newTarget = hitColl.GetComponent<PuzzleRayCastTarget>();
                if(newTarget.castData != castData)
                    continue;

                if(!nearestTarget || hit.distance < castHit.distance) {
                    nearestTarget = newTarget;
					castHit = hit;
                }
			}
		}

        target = nearestTarget;

        if(target)
            castPoint = castHit.point;
        else
			castPoint = position + castDir * castLength;
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
