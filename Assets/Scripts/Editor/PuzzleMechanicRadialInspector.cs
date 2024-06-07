using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PuzzleMechanicRadial))]
public class PuzzleMechanicRadialInspector : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if(GUILayout.Button("Apply Value")) {
			var dat = target as PuzzleMechanicRadial;

			var valProp = serializedObject.FindProperty("_value");

			//correct value if there are angle counts
			if(dat.angleCount > 0) {
				var t = dat.valueScalar;

				valProp.floatValue = Mathf.Lerp(dat.minValue, dat.maxValue, Mathf.Round(t * dat.angleCount) / dat.angleCount);
			}
			else { //clamp value
				valProp.floatValue = Mathf.Clamp(valProp.floatValue, dat.minValue, dat.maxValue);
			}

			serializedObject.ApplyModifiedProperties();


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
}