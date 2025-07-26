using System;
using System.Collections;
using System.Collections.Generic;
using Client.Combat.Events;
using Client.Combat.UI;
using Client.Effects;
using Core.Combat;
using Core.Combat.Events;
using UnityEngine;

namespace Client.Combat
{
    public partial class BattleScene : MonoBehaviour, IEventSource<IBattleSceneEvent>
    {
        public static BattleScene Instance;
        
        [SerializeField] private BulletHellScene bulletHell;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private BattleSprite[] playerBattleSprites;
        [SerializeField] private BattleSprite[] enemyBattleSprites;
        [SerializeField] private Shaker2D cameraShake;
        [SerializeField] private GameOverHandler gameOverHandler;

        public List<int> Targets { get; private set; }
        
        private EventBus events = new EventBus();
        
        public SpriteRenderer Background => background;
        public BattleSprite[] PlayerBattleSprites => playerBattleSprites;
        public BattleSprite[] EnemyBattleSprites => enemyBattleSprites;

        public BattleDialogBox dialogBox;

        private Queue<IEnumerator> sequence;
        private bool sequencePaused;

        private bool readyForBulletHell;

        private bool attackAnimation;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            // StartCoroutine(SequenceLoop());
        }

        /*
        private IEnumerator SequenceLoop()
        {
            while (true)
            {
                if (!sequencePaused && sequence.Count > 0)
                {
                    yield return StartCoroutine(sequence.Dequeue());
                }
                else
                {
                    yield return null;
                }
            }
        }
        */

        public void PlayBattleSequence(PlayBattleSequenceEvent evt)
        {
            BattleInterface.Instance.DialogBox.Clear();
            StartCoroutine(PlaySequenceIE(evt.BattleSequence, () =>
            {
                // Emit to the battle system the end of the battle sequence
                dialogBox.Clear();
                EmitEvent(new BattleSequenceEnded()
                {
                    Player = 0
                });
                EmitEvent(new BattleSequenceEnded()
                {
                    Player = 1
                });
                EmitEvent(new BattleSequenceEnded()
                {
                    Player = 2
                });
            }));
        }

        public void ResetAnimations()
        {
            foreach (var p in PlayerBattleSprites)
            {
                p.ResetAnimations();
            }

            foreach (var e in EnemyBattleSprites)
            {
                e.ResetAnimations();
            }
        }

        public void OnPlayerChooseAction(ChooseActionEvent evt)
        {
            playerBattleSprites[evt.Player].OnPlayerChooseAction(evt);
        }
        
        public void OnPlayerCancelAction(CancelActionEvent evt)
        {
            playerBattleSprites[evt.Player].OnPlayerCancelAction(evt);
        }

        public void EndBattleSequence(PlayEndBattleSequenceEvent evt)
        {
            StartCoroutine(EndBattleSequenceIE(evt.sequence));
        }
        
        public IEnumerator EndBattleSequenceIE(List<BattleSequence> sequence)
        {
            yield return new WaitUntil(() => !attackAnimation);
            yield return StartCoroutine(PlaySequenceIE(sequence));
            EmitEvent(new EndBattleReadyEvent()
            {
                Player = 0
            });
            EmitEvent(new EndBattleReadyEvent()
            {
                Player = 1
            });
            EmitEvent(new EndBattleReadyEvent()
            {
                Player = 2
            });
        }

        public void EndBattle()
        {
            // TODO Reset battle scene
        }

        public void PrepareBulletPhase(BulletHellWaitReady evt)
        {
            Debug.Log("PrepareBulletPhase");
            foreach (var p in playerBattleSprites)
            {
                p.OnBulletHellPrepare();
            }
            // TODO Prepare the bullet hell prefab
            StartCoroutine(PreBulletSequence(evt.Targets, evt.BattleSequence));
        }

        IEnumerator PreBulletSequence(List<int> targets, List<BattleSequence> sequence)
        {
            Targets = targets;
            // Wait for animation routine to end
            yield return new WaitUntil(() => !attackAnimation);
            // Darken the background
            LeanTween.value(gameObject, Color.white, Color.gray, 0.8f).setOnUpdate(color =>
            {
                if (Background)
                {
                    Background.color = color;
                }
            }).setEaseOutQuad();
            for (int i = 0; i < playerBattleSprites.Length; i++)
            {
                if (!targets.Contains(i))
                {
                    playerBattleSprites[i].Darken();
                }
            }
            BattleInterface.Instance.ShowTargets(targets);
            yield return StartCoroutine(PlaySequenceIE(sequence));
            yield return new WaitForSeconds(0.5f);
            BattleInterface.Instance.HideTargets();
            yield return new WaitForSeconds(0.5f);
            EmitEvent(new BulletHellReadyEvent()
            {
                Player = 0
            });
            EmitEvent(new BulletHellReadyEvent()
            {
                Player = 1
            });
            EmitEvent(new BulletHellReadyEvent()
            {
                Player = 2
            });
        }

        public void StartBulletHell()
        {
            dialogBox.Clear();
            bulletHell.gameObject.SetActive(true);
            bulletHell.StartPhase();
        }

        public IEnumerator PlayerAttack(PlayerAttackEvent evt)
        {
            attackAnimation = true;
            yield return StartCoroutine(PlayerAttackAnimation(evt.Player));
            yield return StartCoroutine(EnemyHurtAnimation(evt.Player, evt.Target, evt.Damage.ToString()));
            if (evt.Fainted)
            {
                // TODO Play death animation
            }
            attackAnimation = false;
        }
        
        public IEnumerator PlayerMiss(int playerId, int targetId)
        {
            attackAnimation = true;
            yield return StartCoroutine(PlayerAttackAnimation(playerId));
            // TODO Show miss
            yield return new WaitForSeconds(1f);
            attackAnimation = false;
        }

        private IEnumerator PlayerAttackAnimation(int playerId)
        {
            playerBattleSprites[playerId].Play("Fight");
            // Play Attack SFX
            SfxHandler.Play("cut");
            yield return new WaitForSeconds(0.8f);
        }

        private IEnumerator EnemyHurtAnimation(int playerId, int targetId, string damage)
        {
            enemyBattleSprites[targetId].Play("Hurt");
            // Play Damaged SFX
            SfxHandler.Play("damage_enemy");
            // Show damage indicator
            StartCoroutine(enemyBattleSprites[targetId].ShowEnemyDamageIE(playerId, damage));
            yield return new WaitForSeconds(1.5f);
        }
        
        public void OnDamagePlayerEvent(DamagePlayerEvent evt)
        {
            SfxHandler.Play("hurt");
            // Play Animation
            playerBattleSprites[evt.Player].Play("Hurt");
            StartCoroutine(playerBattleSprites[evt.Player].ShowPlayerDamageIE(evt.Damage));
            // Shake screen
            LeanTween.value(gameObject, 1, 0, 0.5f).setOnUpdate(flt =>
            {
                cameraShake.horizontalStrength = flt;
                cameraShake.verticalStrength = flt;
            });
        }

        public void OnHealPlayerEvent(HealPlayerEvent evt)
        {
            SfxHandler.Play("heal");
            StartCoroutine(playerBattleSprites[evt.Player].ShowPlayerHealIE(evt.HealAmount.ToString()));
            playerBattleSprites[evt.Player].SetDowned(evt.CurrentHp <= 0);
        }

        public void OnKnockOutEvent(KnockOutEvent evt)
        {
            Targets = evt.NewTargets;
            playerBattleSprites[evt.Player].SetDowned(true);
            for (int i = 0; i < playerBattleSprites.Length; i++)
            {
                if (!evt.NewTargets.Contains(i))
                {
                    playerBattleSprites[i].Darken();
                }
                else
                {
                    playerBattleSprites[i].Lighten();
                }
            }
        }

        public void OnGameOver()
        {
            foreach (var instanceDamageIndicator in BattleInterface.Instance.DamageIndicators)
            {
                instanceDamageIndicator.SetActive(false);
            }
            SoulController.Player.DisableInput();
            gameOverHandler.gameObject.SetActive(true);
            bulletHell.Stop();
            gameOverHandler.Play();
        }
        
        #region Scene Events
    
        public void SubscribeEvent<T>(Action<T> callback) where T : class, IBattleSceneEvent
        {
            events.Subscribe(callback);
        }

        public void UnsubscribeEvent<T>(Action<T> callback) where T : class,  IBattleSceneEvent
        {
            events.Unsubscribe(callback);
        }

        public void EmitEvent<T>(T evt) where T : class, IBattleSceneEvent
        {
            events.Emit(evt);
        }
        
        #endregion
    }
}
