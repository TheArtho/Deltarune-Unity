using UnityEngine;
using UnityEngine.Playables;

namespace Client.Combat
{
    public class BulletPattern : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;

        public void Initialize(AudioSource sfxHandler, Animator attacker, Animator battleArea)
        {
            if (!playableDirector || !playableDirector.playableAsset)
                return;

            // Bind the world objects to the Timeline
            foreach (var output in playableDirector.playableAsset.outputs)
            {
                string trackName = output.streamName;

                switch (trackName)
                {
                    case "Battle Area":
                        playableDirector.SetGenericBinding(output.sourceObject, battleArea);
                        break;
                    case "SFXHandler":
                        playableDirector.SetGenericBinding(output.sourceObject, sfxHandler);
                        break;
                    case "Enemy":
                        playableDirector.SetGenericBinding(output.sourceObject, attacker);
                        break;
                }
            }
        }
    }
}