using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepTriggerWakeAndMove : MonoBehaviour {
    public SheepController target;

	public float wakeDelay = 1f;
	public SheepController.Side moveSide = SheepController.Side.Right;

    private bool mIsTriggered;

	void OnTriggerEnter2D(Collider2D collision) {
        if(mIsTriggered) return;

		if(collision.CompareTag(GameData.instance.sheepMainTag)) {
            StartCoroutine(DoWakeAndMove());
            mIsTriggered = true;
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
