using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuzzleMechanicValueBase), true)]
public class PuzzleMechanicValueInspector : Editor {
	public override void OnInspectorGUI() {
		var minValProp = serializedObject.FindProperty("_minValue");
		var maxValProp = serializedObject.FindProperty("_maxValue");
		var stepCountProp = serializedObject.FindProperty("_stepCount");
		var valProp = serializedObject.FindProperty("_value");
		var eventProp = serializedObject.FindProperty("onValueChanged");
		var signalProp = serializedObject.FindProperty("signalInvokeValueChanged");

		serializedObject.UpdateIfRequiredOrScript();
		SerializedProperty iterator = serializedObject.GetIterator();
		bool enterChildren = true;
		while(iterator.NextVisible(enterChildren)) {
			//skip value properties
			if(iterator.name == "_minValue" || iterator.name == "_maxValue" || iterator.name == "_stepCount" || iterator.name == "_value" || iterator.name == "onValueChanged" || iterator.name == "signalInvokeValueChanged")
				continue;

			using(new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath)) {
				EditorGUILayout.PropertyField(iterator, true);
			}

			enterChildren = false;
		}
				
		M8.EditorExt.Utility.DrawSeparator();

		//draw values
		var isValuePropsChanged = false;

		EditorGUILayout.BeginHorizontal();

		EditorGUIUtility.labelWidth = 80f;

		EditorGUILayout.LabelField("Value Range");
				
		EditorGUIUtility.labelWidth = 30f;

		var _min = EditorGUILayout.FloatField("Min", minValProp.floatValue);
		if(minValProp.floatValue != _min) {
			minValProp.floatValue = _min;
			isValuePropsChanged = true;
		}

		var _max = EditorGUILayout.FloatField("Max", maxValProp.floatValue);
		if(maxValProp.floatValue != _max) {
			maxValProp.floatValue = _max;
			isValuePropsChanged = true;
		}

		EditorGUIUtility.labelWidth = 0f;

		EditorGUILayout.EndHorizontal();

		var _stepCount = EditorGUILayout.IntField("Step Count", stepCountProp.intValue);
		if(stepCountProp.intValue != _stepCount) {
			stepCountProp.intValue = _stepCount;
			isValuePropsChanged = true;
		}

		var _val = EditorGUILayout.Slider("Value", valProp.floatValue, _min, _max);

		//correct value if there are step counts
		if(_stepCount > 0) {
			var delta = _max - _min;
			var t = delta > 0f ? Mathf.Clamp01((_val - _min) / delta) : 0f;

			_val = Mathf.Lerp(_min, _max, Mathf.Round(t * _stepCount) / _stepCount);
		}

		if(valProp.floatValue != _val) {
			valProp.floatValue = _val;
			isValuePropsChanged = true;
		}

		EditorGUILayout.PropertyField(eventProp, true);
		EditorGUILayout.PropertyField(signalProp, true);

		serializedObject.ApplyModifiedProperties();

		if(isValuePropsChanged)
			OnValueChanged();
	}

	protected virtual void OnValueChanged() {

	}
}