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
}
