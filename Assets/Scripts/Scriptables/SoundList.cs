using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "Scriptable Objects/SoundList")]
public class SoundList : ScriptableObject
{
    [Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip clip;
    }
    
    public List<Sound> list;
}

