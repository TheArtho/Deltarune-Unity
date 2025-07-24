using System;
using UnityEngine;

public class BgmHandler : MonoBehaviour
{
    public static BgmHandler instance;
    
    private AudioSource sourceMain;
    private AudioSource sourceOverlay;
    private AudioSource sourceMfx;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        sourceMain = GetComponents<AudioSource>()[0];
        sourceOverlay = GetComponents<AudioSource>()[1];
        sourceMfx = GetComponents<AudioSource>()[2];
    }
    
    public static void PlayMain(string clip_id)
    {
        AudioClip clip = Global.Musics.list.Find(sfx => sfx.name == clip_id).music.clip;

        if (clip)
        {
            instance.sourceMain.clip = clip;
            instance.sourceMain.Play();
        }
    }

    public static void Stop()
    {
        instance.sourceMain.Stop();
        instance.sourceMfx.Stop();
        instance.sourceOverlay.Stop();
    }
}
