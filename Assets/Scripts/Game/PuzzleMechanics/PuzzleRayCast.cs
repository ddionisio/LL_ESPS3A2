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
    public LayerMask castEndLayerMask;

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

	public Vector2 position { get { return transform.position; } }

    public M8.CacheList<PuzzleRayCastTarget> targets { get { return mTargets; } }

	public RaycastHit2D[] castHits { get { return mCastHits; } }
    public int castHitCount { get; private set; }

    private bool mCastActive;
    private float mCurUpdateTime;

    private float mCastDistance;

    private const int castCapacity = 6;

    private M8.CacheList<PuzzleRayCastTarget> mTargets = new M8.CacheList<PuzzleRayCastTarget>(castCapacity);
	private M8.CacheList<PuzzleRayCastTarget> mPrevTargets = new M8.CacheList<PuzzleRayCastTarget>(castCapacity);

	private RaycastHit2D[] mCastHits = new RaycastHit2D[castCapacity];

    public void ClearTargets() {
        for(int i = 0; i < mTargets.Count; i++) {
            var tgt = mTargets[i];
            if(tgt)
				tgt.ApplyCast(null);
		}

        mTargets.Clear();
	}

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

                for(int i = 0; i < mTargets.Count; i++) {
                    var tgt = mTargets[i];
                    if(tgt)
                        tgt.UpdateCast(this);
				}
			}
        }
	}

	private void Cast() {
		//set distance
		RaycastHit2D endHit;
        if(castRadius > 0f)
            endHit = Physics2D.CircleCast(position, castRadius, castDir, castLength, castEndLayerMask);
        else
			endHit = Physics2D.Raycast(position, castDir, castLength, castEndLayerMask);

        if(endHit.collider)
            castDistance = endHit.distance;
        else
            castDistance = castLength;

        //grab targets
		var contactTargetFilter = new ContactFilter2D() { useLayerMask = true, layerMask = castTargetLayerMask };

		if(castRadius > 0f)
			castHitCount = Physics2D.CircleCast(position, castRadius, castDir, contactTargetFilter, mCastHits, castLength);
		else
			castHitCount = Physics2D.Raycast(position, castDir, contactTargetFilter, mCastHits, castLength);

        if(castHitCount > 0) {
            //copy targets to previous
			mPrevTargets.Clear();
            for(int i = 0; i < mTargets.Count; i++)
                mPrevTargets.Add(mTargets[i]);

			//grab new targets
			mTargets.Clear();
			
			for(int i = 0; i < castHitCount; i++) {
                var hit = mCastHits[i];

				var hitColl = hit.collider;
				if(!hitColl)
					continue;

				//ensure hit is not in our hierarchy
				if(hitColl.gameObject == gameObject || M8.Util.IsParentOf(transform, hitColl.transform) || M8.Util.IsParentOf(hitColl.transform, transform))
					continue;

				var newTarget = hitColl.GetComponent<PuzzleRayCastTarget>();
				if(!newTarget || newTarget.castData != castData)
					continue;

				mTargets.Add(newTarget);
			}

            //check each new target if they are already from previous
            for(int i = 0; i < mTargets.Count; i++) {
                var tgt = mTargets[i];

                int prevInd = mPrevTargets.IndexOf(tgt);
                if(prevInd == -1)
                    tgt.ApplyCast(this); //new target
                else
                    mPrevTargets.RemoveAt(prevInd);
            }

            //update previous targets that are no longer in contact
            for(int i = 0; i < mPrevTargets.Count; i++) {
                var tgt = mPrevTargets[i];
                if(tgt)
                    tgt.ApplyCast(null);
            }

            mPrevTargets.Clear();
		}
        else
            ClearTargets();
    }

    private void ApplyActive() {
        if(castActiveGO) castActiveGO.SetActive(mCastActive);

        if(!mCastActive)
            ClearTargets();
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
