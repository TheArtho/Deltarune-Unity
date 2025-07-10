using System;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Global instance;
    
    [SerializeField]
    private MusicList musicList;
    [SerializeField]
    private SoundList soundList;
    [SerializeField]
    private SpriteList spriteList;

    public static SoundList Sounds => instance.soundList;
    public static MusicList Musics => instance.musicList;
    public static SpriteList Sprites => instance.spriteList;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
