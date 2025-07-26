using System;
using Scriptables;
using UnityEngine;

public class BgmHandler : MonoBehaviour
{
    private enum TrackType
    {
        Main,
        Overlay,
        Mfx
    }
    
    public Music mainTrack;
    public Music overlayTrack;
    public Music mfxTrack;
    
    public static BgmHandler instance;
    
    private AudioSource sourceMain;
    private AudioSource sourceOverlay;
    private AudioSource sourceMfx;
    
    private TrackType trackType;

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

    void FixedUpdate()
    {
        var track = mainTrack;
        var source = sourceMain;
        
        switch (trackType)
        {
            case TrackType.Main:
            {
                if (!sourceMain.isPlaying) return;
                break;
            }
            case TrackType.Overlay:
            {
                if (!sourceOverlay.isPlaying) return;
                source = sourceOverlay;
                break;
            }
            case TrackType.Mfx:
            {
                if (!sourceMfx.isPlaying) return;
                source = sourceMfx;
                break;
            }
        }

        if (!track || !track.clip)
        {
            return;
        }

        if (track.endLoop <= 0)
        {
            return;
        }
        
        if (track.endLoop > track.clip.length || source.time > track.endLoop)
        {
            source.time = track.startLoop + (source.time - track.endLoop);
        }
    }

    public static void PlayMain(string clip_id)
    {
        instance.mainTrack = Global.Musics.list.Find(sfx => sfx.name == clip_id).music;

        if (!instance.mainTrack || !instance.mainTrack.clip) return;
        instance.sourceMain.clip = instance.mainTrack.clip;
        instance.sourceMain.Play();
    }

    public static void Stop()
    {
        instance.sourceMain.Stop();
        instance.sourceMfx.Stop();
        instance.sourceOverlay.Stop();
    }
}
