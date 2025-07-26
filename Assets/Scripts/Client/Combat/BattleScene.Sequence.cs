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
        public IEnumerator PlaySequenceIE(List<BattleSequence> sequence, Action callback = null)
        {
            foreach (var s in sequence)
            {
                switch (s)
                {
                    case EnemyDialogSequence dialog:
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
                    
                    case ClearTextSequence text:
                        if (text.RunInParallel)
                        {
                            StartCoroutine(PlayClearTextSequence());
                        }
                        else
                        {
                            yield return StartCoroutine(PlayClearTextSequence());
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
                    
                    case HealPlayerSequence heal:
                        if (heal.RunInParallel)
                        {
                            StartCoroutine(PlayHealPlayerSequence(heal));
                        }
                        else
                        {
                            yield return StartCoroutine(PlayHealPlayerSequence(heal));
                        }
                        
                        break;
                    
                    case HealEnemySequence heal:
                        if (heal.RunInParallel)
                        {
                            StartCoroutine(PlayHealEnemySequence(heal));
                        }
                        else
                        {
                            yield return StartCoroutine(PlayHealEnemySequence(heal));
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
            Debug.Log($"[BattleScene] Text Sequence : {seq.Text}");
            if (seq.ClearText)
            {
                dialogBox.Clear();
            }
            yield return StartCoroutine(dialogBox.DrawText(seq.Text, seq.Sound, seq.timePerChar));
            yield return new WaitForSeconds(seq.Delay);
        }

        private IEnumerator PlayClearTextSequence()
        {
            dialogBox.Clear();
            yield return null;
        }

        private IEnumerator PlayDialogSequence(EnemyDialogSequence seq)
        {
            Debug.Log($"[BattleScene] Enemy Dialog Sequence from {seq.EnemyId} : {seq.Text}");
            DialogBox dialog = BattleInterface.Instance.EnemyDialogBoxes[seq.EnemyId];
            dialog.gameObject.SetActive(true);
            if (seq.ClearText)
            {
                dialog.Clear();
            }
            yield return StartCoroutine(dialog.DrawText(seq.Text, seq.Sound, seq.TimePerChar));
            yield return new WaitForSeconds(seq.Time);
            dialog.gameObject.SetActive(false);
        }
        
        private IEnumerator PlayPlayerAnimSequence(PlayerAnimationSequence seq)
        {
            Debug.Log($"[BattleScene] Played Animation for player {seq.Player} : {seq.Animation}");
            playerBattleSprites[seq.Player].Play(seq.Animation);
            yield return new WaitForSeconds(seq.Time);
        }
        
        private IEnumerator PlayHealPlayerSequence(HealPlayerSequence seq)
        {
            BattleInterface.Instance.OnHealPlayerSequence(seq);
            SfxHandler.Play("heal");
            StartCoroutine(playerBattleSprites[seq.Player].ShowPlayerHealIE(seq.HealAmount.ToString()));
            playerBattleSprites[seq.Player].SetDowned(seq.CurrentHp <= 0);
            yield return new WaitForSeconds(1f);
        }
        
        private IEnumerator PlayHealEnemySequence(HealEnemySequence seq)
        {
            SfxHandler.Play("heal");
            StartCoroutine(playerBattleSprites[seq.Enemy].ShowPlayerHealIE(seq.HealAmount.ToString()));
            playerBattleSprites[seq.Enemy].SetDowned(seq.CurrentHp <= 0);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator PlayBattleSequence(BattleSequence seq)
        {
            Debug.Log("Execute battle logic...");
            yield return new WaitForSeconds(0.8f);
        }
    }
}
