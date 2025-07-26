using System;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "BGM_New", menuName = "Scriptable Objects/Music")]
    [Serializable]
    public class Music : ScriptableObject
    {
        public AudioClip clip;
        [Tooltip("Start loop time in seconds.")]
        public float startLoop = 0;
        [Tooltip("End loop time in seconds.")]
        public float endLoop = -1;
        [Space]
        [Tooltip("Volume attenuation in dB.")]
        public float attenuation = 0;

        public static bool CheckLoop(Music music, float time)
        {
            if (music.endLoop <= music.startLoop) return false;
            return time > music.endLoop;
        }
    }
}