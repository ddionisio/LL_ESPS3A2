﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Renegadeware {
    public class SetScreenOrientation : MonoBehaviour {
        public ScreenOrientation orientation = ScreenOrientation.LandscapeLeft;

        void Awake() {
            Screen.orientation = orientation;
        }
    }
}