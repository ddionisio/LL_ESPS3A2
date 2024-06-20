using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PuzzleMechanicSlider))]
public class PuzzleMechanicSliderInspector : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if(GUILayout.Button("Apply Value")) {
			var dat = target as PuzzleMechanicSlider;

			var valProp = serializedObject.FindProperty("_value");

			//correct value if there are angle counts
			if(dat.stepCount > 0) {
				var t = dat.valueScalar;

				valProp.floatValue = Mathf.Lerp(dat.minValue, dat.maxValue, Mathf.Round(t * dat.stepCount) / dat.stepCount);
			}
			else { //clamp value
				valProp.floatValue = Mathf.Clamp(valProp.floatValue, dat.minValue, dat.maxValue);
			}

			serializedObject.ApplyModifiedProperties();


			//apply root telemetry based on value
			if(dat.handleRoot) {
				dat.handleRoot.position = dat.transform.TransformPoint(new Vector3(dat.GetHandleLocalX(), 0f, 0f));
				dat.handleRoot.rotation = dat.transform.rotation;

				EditorUtility.SetDirty(dat.handleRoot);
			}
		}
	}
}
