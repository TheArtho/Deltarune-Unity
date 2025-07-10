using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteList", menuName = "Scriptable Objects/SpriteList")]
public class SpriteList : ScriptableObject
{
    [Serializable]
    public struct SpriteData
    {
        public string name;
        public Sprite sprite;
    }
    
    public List<SpriteData> list;
}
