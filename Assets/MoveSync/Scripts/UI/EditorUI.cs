using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoveSync.UI
{
    public class EditorUI : MonoBehaviour
    {
        public Slider slider;
        public Text tickText;
        public Text timeText;


        void Start()
        {
            slider.onValueChanged.AddListener(LevelEditor.instance.SetSongTime);
            slider.onValueChanged.AddListener(OnTimelineChanged);
            slider.maxValue = LevelSequencer.instance.audioSource.clip.length * LevelSequencer.instance.toBPM;
        }

        void OnTimelineChanged(float tick)
        {
            float time = tick * LevelSequencer.instance.toTime;
            string minutes = Mathf.Floor(time / 60).ToString("00");
            string seconds = (time % 60).ToString("00");

            tickText.text = $"Tick: {tick}";
            timeText.text = $"Time: {minutes}:{seconds}";
        }
    }
}