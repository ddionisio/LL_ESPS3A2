using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PuzzleMechanicSwitch))]
public class PuzzleMechanicSwitchInspector : Editor {
	public override void OnInspectorGUI() {
		var indexProp = serializedObject.FindProperty("_index");

		serializedObject.UpdateIfRequiredOrScript();
		SerializedProperty iterator = serializedObject.GetIterator();
		bool enterChildren = true;
		while(iterator.NextVisible(enterChildren)) {
			//skip value properties
			if(iterator.name == "_index")
				continue;

			using(new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath)) {
				EditorGUILayout.PropertyField(iterator, true);
			}

			enterChildren = false;
		}

		M8.EditorExt.Utility.DrawSeparator();

		var isValuePropsChanged = false;

		var dat = target as PuzzleMechanicSwitch;

		if(dat.switches.Length > 0) {
			var _index = EditorGUILayout.IntSlider("Index", indexProp.intValue, 0, dat.switches.Length - 1); 
			if(indexProp.intValue != _index) {
				indexProp.intValue = _index;
				isValuePropsChanged = true;
			}
		}
		else {
			EditorGUILayout.LabelField("Need at least one switch for index.");
		}

		if(isValuePropsChanged)
			dat.ApplyCurrentIndex();

		serializedObject.ApplyModifiedProperties();
	}
}
