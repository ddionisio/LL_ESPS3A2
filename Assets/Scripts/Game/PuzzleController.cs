using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set this to the root of the artifact object, contains info for all goals
/// </summary>
public class PuzzleController : MonoBehaviour {
	public GameObject goalRoot;

	public GoalController[] goals { get; private set; }

	void OnDestroy() {
		if(GameData.isInstantiated) {
			GameData.instance.signalPlayBegin.callback -= OnPuzzleBegin;
		}

		if(goals != null) {
			for(int i = 0; i < goals.Length; i++) {
				var goal = goals[i];
				if(goal)
					goal.powerFullyCharged.RemoveListener(OnGoalFullyCharged);
			}
		}
	}

	void Awake() {
		goals = goalRoot ? goalRoot.GetComponentsInChildren<GoalController>(true) : GetComponentsInChildren<GoalController>(true);

		for(int i = 0; i < goals.Length; i++) {
			var goal = goals[i];

			goal.powerFullyCharged.AddListener(OnGoalFullyCharged);
		}

		GameData.instance.signalPlayBegin.callback += OnPuzzleBegin;
	}

	void OnPuzzleBegin() {
		
	}

	void OnGoalFullyCharged(bool isFull) {
		//determine if all goals are active
		int goalActiveCount = 0;
		for(int i = 0; i < goals.Length; i++) {
			var _goal = goals[i];
			if(_goal.isPowerFull)
				goalActiveCount++;
		}

		if(goalActiveCount == goals.Length) {
			for(int i = 0; i < goals.Length; i++)
				goals[i].ForceAudioPlay();

			GameData.instance.signalPuzzleComplete.Invoke();
		}
	}
}
