using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRayCast : MonoBehaviour {
    public CastData castData;

	public bool castInitialActive;

	public GameObject castActiveGO;
    public Transform castDirRoot;
    public Transform castPointEndRoot;
        
    public float castRadius; //set to <= 0 as line
    public float castLength;
    public float castEndOfs; //slight offset from collision point for display
    public LayerMask castTargetLayerMask;

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

    public float castDistance { 
        get { return mCastDistance; }
        private set {
            if(mCastDistance != value) {
                mCastDistance = value;

				castPointEnd = position + castDir * (mCastDistance + castEndOfs);
			}
        }
    }

    public Vector2 castPointEnd {
        get { return castPointEndRoot ? castPointEndRoot.position : position; }

        private set {
            if(castPointEndRoot) {
                Vector2 pos = castPointEndRoot.position;
                if(pos != value)                    
                    castPointEndRoot.position = new Vector3(value.x, value.y, castPointEndRoot.position.z);
            }
        }
    }

    public GameObject castHitGO { get; private set; }

	public Vector2 position { get { return transform.position; } }

	public RaycastHit2D[] castHits { get { return mCastHits; } }
    public int castHitCount { get; private set; }

    private bool mCastActive;
    private float mCurUpdateTime;

    private float mCastDistance;

    private const int castCapacity = 6;

    private PuzzleRayCastTarget mTarget;

	private RaycastHit2D[] mCastHits = new RaycastHit2D[castCapacity];

    public void ClearTarget() {
        if(mTarget) {
            mTarget.ApplyCast(null);
			mTarget = null;
		}
	}

	void OnEnable() {
		ApplyActive();

        if(mCastActive)
		    UpdateCast();
	}

	void Awake() {
		mCastActive = castInitialActive;
	}

	void Update() {
        if(mCastActive) {
            mCurUpdateTime += Time.deltaTime;
            if(mCurUpdateTime >= GameData.instance.mechanicRayCastUpdateDelay) {
                mCurUpdateTime = 0f;

                UpdateCast();
			}
        }
	}

    private void UpdateCast() {
		Cast();

        if(mTarget)
            mTarget.UpdateCast(this);
	}

	private void Cast() {
        //grab targets
		var contactTargetFilter = new ContactFilter2D() { useLayerMask = true, layerMask = castTargetLayerMask };

		if(castRadius > 0f)
			castHitCount = Physics2D.CircleCast(position, castRadius, castDir, contactTargetFilter, mCastHits, castLength);
		else
			castHitCount = Physics2D.Raycast(position, castDir, contactTargetFilter, mCastHits, castLength);

        var nearestDist = castLength;
        GameObject nearestHitGO = null;

		if(castHitCount > 0) {
            PuzzleRayCastTarget newTarget = null;

			for(int i = 0; i < castHitCount; i++) {
                var hit = mCastHits[i];

				var hitColl = hit.collider;
				if(!hitColl)
					continue;

				//ensure hit is not in our hierarchy
				if(hitColl.gameObject == gameObject || M8.Util.IsParentOf(transform, hitColl.transform) || M8.Util.IsParentOf(hitColl.transform, transform))
					continue;
                				
				newTarget = hitColl.GetComponent<PuzzleRayCastTarget>();
                if(newTarget && newTarget.castData != castData)
                    continue;

				nearestHitGO = hit.collider.gameObject;
				nearestDist = hit.distance;

				break;
			}

            if(mTarget != newTarget) {
                if(mTarget)
                    mTarget.ApplyCast(null);

                mTarget = newTarget;
                if(mTarget)
                    mTarget.ApplyCast(this);
			}
		}
        else
            ClearTarget();

		castDistance = nearestDist;
        castHitGO = nearestHitGO;
	}

    private void ApplyActive() {
        if(castActiveGO) castActiveGO.SetActive(mCastActive);

        if(!mCastActive)
            ClearTarget();
	}

	void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        var _castDirRoot = castDirRoot ? castDirRoot : transform;

        Vector2 _up = _castDirRoot.up;
                
		M8.Gizmo.ArrowLine2D(position, position + _up);

		Gizmos.DrawWireSphere(position, castRadius);

		if(castPointEndRoot)
            Gizmos.DrawSphere(castPointEndRoot.position, 0.1f);
	}
}
