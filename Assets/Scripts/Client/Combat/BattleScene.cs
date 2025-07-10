using System;
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
            Debug.Log("test");
            playerBattleSprites[evt.Player].OnPlayerChooseAction(evt);
        }
        
        public void OnPlayerCancelAction(CancelActionEvent evt)
        {
            playerBattleSprites[evt.Player].OnPlayerCancelAction(evt);
        }

        public void StartBulletHell()
        {
            dialogBox.Clear();
            bulletHell.gameObject.SetActive(true);
            bulletHell.StartPhase();
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
