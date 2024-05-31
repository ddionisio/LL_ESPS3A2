
/// <summary>
/// Add to script that will be inside the hierarchy of GoalController to receive a state change
/// </summary>
public interface IGoalStateChanged {
	void GoalStateChanged(GoalState state);
}
