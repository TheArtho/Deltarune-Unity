using UnityEngine;

public class SfxHandler : MonoBehaviour
{
    public static SfxHandler instance;
    
    private AudioSource source;
    
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
        
        source = GetComponent<AudioSource>();
    }

    public static void Play(AudioClip clip)
    {
        instance.source.PlayOneShot(clip);
    }
    
    public static void Play(string clip_id)
    {
        AudioClip clip = Global.Sounds.list.Find(sfx => sfx.name == clip_id).clip;

        if (clip)
        {
            instance.source.PlayOneShot(clip);
        }
    }
}
