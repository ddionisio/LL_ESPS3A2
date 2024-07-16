using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PuzzleMechanicSlider))]
public class PuzzleMechanicSliderInspector : PuzzleMechanicValueInspector {
	protected override void OnValueChanged() {
		var dat = target as PuzzleMechanicSlider;

		//apply root telemetry based on value
		if(dat.handleRoot) {
			dat.handleRoot.position = dat.transform.TransformPoint(new Vector3(dat.GetHandleLocalX(), 0f, 0f));
			dat.handleRoot.rotation = dat.transform.rotation;

			EditorUtility.SetDirty(dat.handleRoot);
		}
	}
}
