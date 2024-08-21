using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepTriggerWakeAndMove : SensorCheckBase {
    public SheepController target;

	public float wakeDelay = 1f;
	public SheepController.Side moveSide = SheepController.Side.Right;

	protected override void OnSensorCheck(Collider2D coll) {
		if(coll.CompareTag(GameData.instance.sheepMainTag)) {
			StartCoroutine(DoWakeAndMove());
			isActive = false;
		}
	}

	void Awake() {
		if(!target)
			target = GetComponent<SheepController>();
	}

	IEnumerator DoWakeAndMove() {
		target.PerformAction(SheepController.Action.Wake);

        yield return new WaitForSeconds(wakeDelay);

		target.MoveOffscreen(moveSide);
    }
}
