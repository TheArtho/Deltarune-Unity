using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Client.Combat.Events;
using Core.Combat.Events;
using UnityEngine;

namespace Client.Combat
{
    public class BattleScene : MonoBehaviour
    {
        public static BattleScene Instance;
        
        [SerializeField] private BulletHellScene bulletHell;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private BattleSprite[] playerBattleSprites;
        [SerializeField] private BattleSprite[] enemyBattleSprites;
        
        private EventBus events = new EventBus();
        
        public SpriteRenderer Background => background;
        public BattleSprite[] PlayerBattleSprites => playerBattleSprites;
        public BattleSprite[] EnemyBattleSprites => enemyBattleSprites;

        public DialogBox dialogBox;

        private Queue<IEnumerator> sequence;
        private bool sequencePaused;

        private bool readyForBulletHell;

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
            // Emit to the battle system the end of the battle sequence
            // TODO Change this hard coded part
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

        public void StartBulletHell()
        {
            foreach (var p in playerBattleSprites)
            {
                p.OnBulletHellStart();
            }
            dialogBox.Clear();
            bulletHell.gameObject.SetActive(true);
            bulletHell.StartPhase();
        }

        public IEnumerator PlayerAttack(int playerId, int targetId)
        {
            yield return StartCoroutine(PlayerAttackAnimation(playerId));
            yield return StartCoroutine(EnemyHurtAnimation(targetId));
        }
        
        public IEnumerator PlayerMiss(int playerId, int targetId)
        {
            yield return StartCoroutine(PlayerAttackAnimation(playerId));
        }

        private IEnumerator PlayerAttackAnimation(int playerId)
        {
            playerBattleSprites[playerId].Play("Fight");
            // Play Attack SFX
            SfxHandler.Play("cut");
            yield return new WaitForSeconds(0.8f);
        }

        private IEnumerator EnemyHurtAnimation(int targetId)
        {
            enemyBattleSprites[targetId].Play("Hurt");
            // Play Damaged SFX
            SfxHandler.Play("damage_enemy");
            // Show damage indicator
            yield return new WaitForSeconds(0.8f);
        }
        
        #region Scene Events
    
        public void SubscribeEvent<T>(Action<T> callback) where T : class, IBattleSceneEvents
        {
            events.Subscribe(callback);
        }

        public void UnsubscribeEvent<T>(Action<T> callback) where T : class,  IBattleSceneEvents
        {
            events.Unsubscribe(callback);
        }

        public void EmitEvent<T>(T evt) where T : class, IBattleSceneEvents
        {
            events.Emit(evt);
        }
        
        #endregion
    }
}
