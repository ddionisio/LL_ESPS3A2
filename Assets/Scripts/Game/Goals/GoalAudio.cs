using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class GoalAudio : MonoBehaviour {
    public AudioSource source;

    private float mLastTime;
	private bool mIsPlaying;

	public void ApplyVolume(float scale) {
		source.volume = M8.UserSettingAudio.instance.soundVolume * scale;
	}

	void OnEnable() {
		mLastTime = Time.time;
		mIsPlaying = false;
	}

	void OnDisable() {
		source.Stop();
	}

	void Awake() {
		source.volume = 0f;
	}

	void Update() {
		if(!source.isPlaying) {
			if(mIsPlaying) {
				mLastTime = Time.time;
				mIsPlaying = false;
			}
			else if(Time.time - mLastTime >= GameData.instance.goalIntervalDelay) {
				source.Play();
				mIsPlaying = true;
			}
		}
	}
}
