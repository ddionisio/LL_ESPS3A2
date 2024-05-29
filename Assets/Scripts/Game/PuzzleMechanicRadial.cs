using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleMechanicRadial : PuzzleMechanicBase {
    [Header("Radial Config")]
	public float radius;

	public float angleStart; //starting from top

    public bool angleLimitEnabled;
    public M8.RangeFloat angleLimitRange = new M8.RangeFloat(0f, 360f);

    [Tooltip("Set to <= 0 for no limit.")]
    public int angleCount;

    [Header("Rotator Display")]
    public float rotatorRadiusOffset; //position of rotator from root in radial space
    public float rotatorAngleStart;
    public Transform rotatorRoot;
    public float rotatorRotateDelay = 0.3f;

    private Vector2 mRotatorDir;
    private float mRotatorAngleVel;
    
	protected override void InputDragBegin(PointerEventData eventData) { 
    }

	protected override void InputDrag(PointerEventData eventData) { 
    }

	protected override void InputDragEnd(PointerEventData eventData) { 
    }

	protected override void Awake() {
		base.Awake();



		ApplyRotatorTransform();
	}

    private void ApplyRotatorTransform() {
        if(rotatorRoot) {
            if(rotatorRadiusOffset > 0f)
                rotatorRoot.localPosition = mRotatorDir * rotatorRadiusOffset;

            rotatorRoot.up = mRotatorDir;
		}
    }

	void OnDrawGizmos() {
		var pos = (Vector2)transform.position;

		if(radius > 0f) {
			Gizmos.color = Color.red;

			Gizmos.DrawWireSphere(pos, radius);

            var dir = Vector2.up;

            dir = M8.MathUtil.RotateAngle(dir, angleStart);

            Gizmos.color = Color.red * 0.3f;

            if(angleCount > 0) {
                var angleDelta = 360f / angleCount;

                for(int i = 0; i < angleCount; i++) {
					Gizmos.DrawLine(pos, pos + dir * radius);

                    dir = M8.MathUtil.RotateAngle(dir, angleDelta);
				}
            }
            else {
                Gizmos.DrawLine(pos, pos + dir * radius);
            }
        }

        if(rotatorRadiusOffset > 0f) {
            Gizmos.color = Color.red * 0.7f;

            Gizmos.DrawWireSphere(pos, rotatorRadiusOffset);

			var dir = Vector2.up;

			dir = M8.MathUtil.RotateAngle(dir, rotatorAngleStart);

			Gizmos.DrawLine(pos, pos + dir * radius);
		}
	}
}
