using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set this to the root of the artifact object, contains info for all goals
/// </summary>
public class PuzzleController : MonoBehaviour {
	public GameObject goalRoot;

	public GoalController[] goals { get; private set; }

	void Awake() {
		goals = goalRoot ? goalRoot.GetComponentsInChildren<GoalController>(true) : GetComponentsInChildren<GoalController>(true);

		for(int i = 0; i < goals.Length; i++) {
			var goal = goals[i];

			goal.stateChangedCallback += OnGoalStateChanged;
		}
	}

	void OnGoalStateChanged(GoalController goal) {
		//determine if all goals are active
	}
}
