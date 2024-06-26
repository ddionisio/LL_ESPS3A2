using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntityRigidbody : MonoBehaviour, IPuzzleEntityStateBegin, IPuzzleEntityStateUpdate {

	public Rigidbody2D body;

	public bool applySimulated;
    public PuzzleEntityState[] simulatedStates;

    public bool applyKinematic;
	public PuzzleEntityState[] kinematicStates;

    public bool finishSpeedThresholdEnabled; //if true, finish current state if speed is below threshold
    public float finishSpeedThreshold;

    void IPuzzleEntityStateBegin.OnStateBegin(PuzzleEntityState state) {
        if(body) {
            var applyReset = false;

            if(applySimulated) {
                var isSimulated = false;
                for(int i = 0; i < simulatedStates.Length; i++) {
                    if(simulatedStates[i] == state) {
                        isSimulated = true;
                        break;
                    }
                }

                body.simulated = isSimulated;
                applyReset = true;
			}

            if(applyKinematic) {
                var isKinematic = false;
                for(int i = 0; i < kinematicStates.Length; i++) {
                    if(kinematicStates[i] == state) {
                        isKinematic = true;
                        break;
                    }
                }

                body.isKinematic = isKinematic;
				applyReset = true;
			}

            if(applyReset)
                ResetPhysics();
        }
    }

    bool IPuzzleEntityStateUpdate.OnStateUpdate(PuzzleEntityState state) {
        var ret = true;

        if(finishSpeedThresholdEnabled && body && body.simulated && !body.isKinematic) {
            var velSpdSqr = body.velocity.sqrMagnitude;

            ret = velSpdSqr <= finishSpeedThreshold * finishSpeedThreshold;
        }

        return ret;
    }

    private void ResetPhysics() {
		body.velocity = Vector2.zero;
		body.angularVelocity = 0f;
	}
}
