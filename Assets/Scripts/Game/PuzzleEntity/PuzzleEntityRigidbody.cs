using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntityRigidbody : MonoBehaviour, IPuzzleEntityStateBegin {

	public Rigidbody2D body;

	public bool applySimulated;
    public PuzzleEntityState[] simulatedStates;

    public bool applyKinematic;
	public PuzzleEntityState[] kinematicStates;	

    void IPuzzleEntityStateBegin.OnStateBegin(PuzzleEntityState state) {
        if(body) {
            if(applySimulated) {
                var isSimulated = false;
                for(int i = 0; i < simulatedStates.Length; i++) {
                    if(simulatedStates[i] == state) {
                        isSimulated = true;
                        break;
                    }
                }

                body.simulated = isSimulated;
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
            }
        }
    }
}
