using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Client.Combat.Events;
using Client.Combat.UI;
using Core.Combat;
using Core.Combat.Events;
using UnityEngine;

namespace Client.Combat
{
    public partial class BattleScene
    {
        public IEnumerator PlaySequenceIE(List<BattleSequence> sequence, Action callback)
        {
            foreach (var s in sequence)
            {
                switch (s)
                {
                    case DialogSequence dialog:
                        if (dialog.RunInParallel)
                        {
                            StartCoroutine(PlayDialogSequence(dialog));
                        }
                        else
                        {
                            yield return StartCoroutine(PlayDialogSequence(dialog));
                        }
                        
                        break;

                    case TextSequence text:
                        if (text.RunInParallel)
                        {
                            StartCoroutine(PlayTextSequence(text));
                        }
                        else
                        {
                            yield return StartCoroutine(PlayTextSequence(text));
                        }
                        
                        break;
                    
                    case PlayerAnimationSequence anim:
                        if (anim.RunInParallel)
                        {
                            StartCoroutine(PlayPlayerAnimSequence(anim));
                        }
                        else
                        {
                            yield return StartCoroutine(PlayPlayerAnimSequence(anim));
                        }
                        
                        break;

                    case { } battle:
                        if (battle.RunInParallel)
                        {
                            StartCoroutine(PlayBattleSequence(battle));
                        }
                        else
                        {
                            yield return StartCoroutine(PlayBattleSequence(battle));
                        }
                        break;

                    default:
                        Debug.LogWarning($"Unknown sequence type: {s.GetType().Name}");
                        break;
                }
            }

            callback?.Invoke();
        }
        
        private IEnumerator PlayTextSequence(TextSequence seq)
        {
            Debug.Log($"[BattleScene] Text Sequence : {seq.text}");
            dialogBox.Clear();
            yield return StartCoroutine(dialogBox.DrawText(seq.text));
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator PlayDialogSequence(DialogSequence seq)
        {
            Debug.Log("Show dialog here...");
            yield return new WaitForSeconds(1f);
        }
        
        private IEnumerator PlayPlayerAnimSequence(PlayerAnimationSequence seq)
        {
            Debug.Log($"[BattleScene] Player Animation for player {seq.character} : {seq.animation}");
            playerBattleSprites[seq.character].Play(seq.animation);
            yield return new WaitForSeconds(seq.time);
        }

        private IEnumerator PlayBattleSequence(BattleSequence seq)
        {
            Debug.Log("Execute battle logic...");
            yield return new WaitForSeconds(0.8f);
        }
    }
}
