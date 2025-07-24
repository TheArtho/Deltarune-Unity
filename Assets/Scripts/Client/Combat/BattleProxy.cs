using Client.Combat.Events;
using Client.Combat.UI;
using Core.Combat.Events;
using UnityEngine;

namespace Client.Combat
{
    public class BattleProxy : MonoBehaviour
    {
        [SerializeField] private BattleScene scene;
        [SerializeField] private BattleInterface @interface;
        
        private SubscriptionGroup subscriptions = new();
        
        /// <summary>
        /// Battle only exists for the hosting client / server
        /// </summary>
        private Battle battle;

        private void Start()
        {
            Player Kris = new Player("Kris", 160, 160, 14, 2, 0);
            Player Susie = new Player("Susie", 190, 190, 18, 2, 3);
            Player Ralsei = new Player("Ralsei", 140, 140, 12, 2, 11);

            Enemy RoaringKnight = new Enemy("Roaring Knight", 7300, 40, 0);

            Battle battleInstance = new Battle(
            new Player[]
            {
                Kris, Susie, Ralsei
            },
            new Enemy[]
            {
                RoaringKnight
            });
            
            Init(battleInstance);
        }

        /// <summary>
        /// Subscribes events to the battle system
        /// </summary>
        /// <param name="battleInstance"></param>
        public void Init(Battle battleInstance)
        {
            Stop();
            battle = battleInstance;

            // Battle Server ===> Proxy
            subscriptions.AddFrom<IBattleEvent, GlobalStateEvent>(battle, OnGlobalState);
            subscriptions.AddFrom<IBattleEvent, PlayerStateEvent>(battle, OnPlayerState);
            subscriptions.AddFrom<IBattleEvent, StartTurnEvent>(battle, OnStartTurn);
            subscriptions.AddFrom<IBattleEvent, CancelActionEvent>(battle, OnCancelAction);
            subscriptions.AddFrom<IBattleEvent, ChooseActionEvent>(battle, OnChooseAction);
            subscriptions.AddFrom<IBattleEvent, PlayBattleSequenceEvent>(battle, OnPlayBattleSequence);
            subscriptions.AddFrom<IBattleEvent, ReqFightQuickTimeDataEvent>(battle, OnFightQuickTimeData);
            subscriptions.AddFrom<IBattleEvent, FightQteStartEvent>(battle, OnStartQuickTimeEvent);
            subscriptions.AddFrom<IBattleEvent, BulletHellWaitReady>(battle, OnBulletHellWaitReady);
            subscriptions.AddFrom<IBattleEvent, BulletHellStartEvent>(battle, OnBulletHellStart);
            subscriptions.AddFrom<IBattleEvent, PlayerAttackEvent>(battle, OnPlayerAttack);
            subscriptions.AddFrom<IBattleEvent, PlayerMissedEvent>(battle, OnPlayerMissed);
            subscriptions.AddFrom<IBattleEvent, AddTpEvent>(battle, OnAddTp);
            subscriptions.AddFrom<IBattleEvent, RemoveTpEvent>(battle, OnRemoveTp);
            subscriptions.AddFrom<IBattleEvent, DamagePlayerEvent>(battle, OnPlayerDamage);
            subscriptions.AddFrom<IBattleEvent, KnockOutEvent>(battle, OnKnockOut);
            subscriptions.AddFrom<IBattleEvent, GameOverEvent>(battle, OnGameOver);

            if (scene)
            {
                // Scene ===> Proxy
                subscriptions.AddFrom<IBattleSceneEvent, BattleSequenceEnded>(scene, OnBattleSequenceEnded);
                subscriptions.AddFrom<IBattleSceneEvent, BulletHellEndedEvent>(scene, OnBulletHellEnded);
                subscriptions.AddFrom<IBattleSceneEvent, BulletHellReadyEvent>(scene, OnBulletHellReady);
                subscriptions.AddFrom<IBattleSceneEvent, GrazeEvent>(scene, OnGraze);
                subscriptions.AddFrom<IBattleSceneEvent, PlayerHurtEvent>(scene, OnPlayerHurt);
            }

            if (@interface)
            {
                // Interface ===> Proxy
                subscriptions.AddFrom<IBattleInterfaceEvent, PlayerCommandEvent>(@interface, OnReceivePlayerAction);
                subscriptions.AddFrom<IBattleInterfaceEvent, PlayerCancelCommandEvent>(@interface, OnCancelPlayerAction);
                subscriptions.AddFrom<IBattleInterfaceEvent, AnsFightQuickTimeEvent>(@interface, OnReceiveFightQuickTime);
            }

            battle.Start();
        }


        /// <summary>
        /// Unsubscribes events to the battle system
        /// </summary>
        /// <param name="battleInstance"></param>
        public void Stop()
        {
            subscriptions.UnsubscribeAll();
        }

        #region Battle Server ===> Proxy

        // Battle Events

        // Battle Interface Events
        
        private void OnGlobalState(GlobalStateEvent evt)
        {
            // Update interface
            @interface.UpdateGlobalState(evt);
        }
        
        private void OnPlayerState(PlayerStateEvent evt)
        {
            // Update interface
            @interface.UpdatePlayerState(evt);
        }

        private void OnStartTurn(StartTurnEvent evt)
        {
            @interface.StartTurn();
            scene.ResetAnimations();
        }
        
        private void OnChooseAction(ChooseActionEvent evt)
        {
            // Update interface actions
            scene.OnPlayerChooseAction(evt);
            @interface.OnPlayerChooseAction(evt);
        }
        
        private void OnCancelAction(CancelActionEvent evt)
        {
            // Cancel interface action
            scene.OnPlayerCancelAction(evt);
            @interface.OnPlayerCancelAction(evt);
        }
        
        private void OnPlayBattleSequence(PlayBattleSequenceEvent evt)
        {
            // Update interface actions
            scene.PlayBattleSequence(evt);
        }

        private void OnFightQuickTimeData(ReqFightQuickTimeDataEvent evt)
        {
            // Enable Quick Time Event for specific player
            @interface.EnableFightQte(evt);
        }
        
        private void OnStartQuickTimeEvent(FightQteStartEvent evt)
        {
            // Start Quick Time Event
            @interface.StartFightQte();
        }

        private void OnPlayerAttack(PlayerAttackEvent evt)
        {
            scene.StartCoroutine(scene.PlayerAttack(evt.Player, evt.Target, evt.Damage));
        }
        
        private void OnPlayerMissed(PlayerMissedEvent evt)
        {
            scene.StartCoroutine(scene.PlayerMiss(evt.Player, evt.Target));
        }

        private void OnBulletHellWaitReady(BulletHellWaitReady evt)
        {
            scene.PrepareBulletPhase(evt);
        }

        private void OnBulletHellStart(BulletHellStartEvent evt)
        {
            scene.StartBulletHell();
        }

        private void OnPlayerDamage(DamagePlayerEvent evt)
        {
            // Play damage animation
            // Update Hp
            @interface.OnDamagePlayerEvent(evt);
            scene.OnDamagePlayerEvent(evt);
        }
        
        private void OnKnockOut(KnockOutEvent evt)
        {
            
        }
        
        private void OnGameOver(GameOverEvent evt)
        {
            Debug.Log("Game Over");
        }

        #endregion
        
        #region Interface ===> Proxy

        private void OnReceivePlayerAction(PlayerCommandEvent evt)
        {
            battle.ReceivePlayerCommand(evt.Player, evt);
        }
        
        private void OnCancelPlayerAction(PlayerCancelCommandEvent evt)
        {
            battle.CancelPlayerCommand(evt.Player);
        }
        
        private void OnReceiveFightQuickTime(AnsFightQuickTimeEvent evt)
        {
            // Emit qte result to battle
            battle.ReceiveFightQte(evt);
        }
        
        private void OnAddTp(AddTpEvent evt)
        {
            @interface.TpBar.SetTp(evt.PreviousValue);
            @interface.TpBar.AddTp(evt.Amount);
        }
        
        private void OnRemoveTp(RemoveTpEvent evt)
        {
            @interface.TpBar.SetTp(evt.PreviousValue);
            @interface.TpBar.RemoveTp(evt.Amount);
        }
        
        #endregion
        
        #region Scene ===> Proxy
        
        private void OnBattleSequenceEnded(BattleSequenceEnded evt)
        {
            battle.ReceiveBattleSequenceEnded(evt.Player);
        }
        
        private void OnBulletHellReady(BulletHellReadyEvent evt)
        {
            battle.ReceiveBulletPhaseReady(evt.Player);
        }
        
        private void OnBulletHellEnded(BulletHellEndedEvent evt)
        {
            battle.ReceiveBulletPhaseEnded(evt.Player);
        }

        private void OnPlayerHurt(PlayerHurtEvent evt)
        {
            battle.PlayerHurt(evt);
        }

        private void OnGraze(GrazeEvent evt)
        {
            battle.Graze(evt);
        }
        
        #endregion
    }
}
