using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMechanicRadial : PuzzleMechanicBase {
    [Header("Radial Config")]
    public float angleStart;

    public bool angleLimitEnabled;
    public M8.RangeFloat angleLimitRange = new M8.RangeFloat(0f, 360f);

    [Tooltip("Set to <= 0 for no limit.")]
    public int angleCount;

    public float radius;

    [Header("Radial Display")]
    public Transform handleRoot;
}
