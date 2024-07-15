using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorGroupProxy : MonoBehaviour {
    public M8.SpriteColorGroup target;
    public Color color;
    public bool inverse;

    public void ApplyColor(bool apply) {
        if(inverse)
            apply = !apply;

        if(apply)
            target.ApplyColor(color);
        else
            target.Revert();
    }
}
