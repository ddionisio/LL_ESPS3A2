using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PuzzleMechanicRadial))]
public class PuzzleMechanicRadialInspector : PuzzleMechanicValueInspector {
	protected override void OnValueChanged() {
		var dat = target as PuzzleMechanicRadial;

		//apply root telemetry based on value
		if(dat.rotatorRoot) {
			var initDir = M8.MathUtil.RotateAngle(Vector2.up, dat.angleStart);
			var dir = M8.MathUtil.RotateAngle(initDir, dat.angleRange.Lerp(dat.valueScalar));

			if(dat.rotatorAttachToRadius && dat.radius > 0f)
				dat.rotatorRoot.localPosition = dir * dat.radius;

			dat.rotatorRoot.up = dat.rotatorRootUpInverse ? -dir : dir;

			EditorUtility.SetDirty(dat.rotatorRoot);
		}
	}
}