using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicList", menuName = "Scriptable Objects/MusicList")]
public class MusicList : ScriptableObject
{
    [Serializable]
    public struct MusicData
    {
        public string name;
        public Music music;
    }
    
    public List<MusicData> list;
}
