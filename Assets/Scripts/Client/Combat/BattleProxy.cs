using System;
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
        
        /// <summary>
        /// Battle only exists for the hosting client / server
        /// </summary>
        private Battle battle;

        private void Start()
        {
            Player Kris = new Player("Kris", 10, 160, 14, 2, 0);
            Player Susie = new Player("Susie", 190, 190, 18, 2, 3);
            Player Ralsei = new Player("Ralsei", 140, 140, 12, 2, 11);

            Enemy RoaringKnight = new Enemy("Roaring Knight", 7300, 40, 0);

            Battle battleInstance = new Battle(new Player[]
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
            battle = battleInstance;
            
            // Battle Server ===> Proxy
            battle.SubscribeEvent<GlobalStateEvent>(OnGlobalState);
            battle.SubscribeEvent<PlayerStateEvent>(OnPlayerState);
            battle.SubscribeEvent<StartTurnEvent>(OnStartTurn);
            battle.SubscribeEvent<CancelActionEvent>(OnCancelAction);
            battle.SubscribeEvent<ChooseActionEvent>(OnChooseAction);
            battle.SubscribeEvent<PlayBattleSequenceEvent>(OnPlayBattleSequence);
            battle.SubscribeEvent<ReqFightQuickTimeDataEvent>(OnFightQuickTimeData);
            battle.SubscribeEvent<FightQteStartEvent>(OnStartQuickTimeEvent);
            battle.SubscribeEvent<BulletHellWaitReady>(OnBulletHellWaitReady);
            battle.SubscribeEvent<BulletHellStartEvent>(OnBulletHellStart);
            battle.SubscribeEvent<PlayerAttackEvent>(OnPlayerAttack);
            battle.SubscribeEvent<PlayerMissedEvent>(OnPlayerMissed);

            if (scene)
            {
                // Scene ===> Proxy
                scene.SubscribeEvent<BattleSequenceEnded>(OnBattleSequenceEnded);
                scene.SubscribeEvent<BulletHellEndedEvent>(OnBulletHellEnded);
                scene.SubscribeEvent<BulletHellReadyEvent>(OnBulletHellReady);
            }

            if (@interface)
            {
                // Interface ===> Proxy
                @interface.SubscribeEvent<PlayerCommandEvent>(OnReceivePlayerAction);
                @interface.SubscribeEvent<PlayerCancelCommandEvent>(OnCancelPlayerAction);
                @interface.SubscribeEvent<AnsFightQuickTimeEvent>(OnReceiveFightQuickTime);
            }
            
            battle.Start();
        }

        /// <summary>
        /// Unsubscribes events to the battle system
        /// </summary>
        /// <param name="battleInstance"></param>
        public void Stop()
        {
            if (battle != null)
            {
                // Battle Server ===> Proxy
                battle.UnsubscribeEvent<GlobalStateEvent>(OnGlobalState);
                battle.UnsubscribeEvent<PlayerStateEvent>(OnPlayerState);
                battle.UnsubscribeEvent<StartTurnEvent>(OnStartTurn);
                battle.UnsubscribeEvent<CancelActionEvent>(OnCancelAction);
                battle.UnsubscribeEvent<ChooseActionEvent>(OnChooseAction);
                battle.UnsubscribeEvent<PlayBattleSequenceEvent>(OnPlayBattleSequence);
                battle.UnsubscribeEvent<ReqFightQuickTimeDataEvent>(OnFightQuickTimeData);
                battle.UnsubscribeEvent<FightQteStartEvent>(OnStartQuickTimeEvent);
                battle.UnsubscribeEvent<BulletHellWaitReady>(OnBulletHellWaitReady);
                battle.UnsubscribeEvent<BulletHellStartEvent>(OnBulletHellStart);
                battle.UnsubscribeEvent<PlayerAttackEvent>(OnPlayerAttack);
                battle.UnsubscribeEvent<PlayerMissedEvent>(OnPlayerMissed);
            }
            
            if (scene)
            {
                // Scene ===> Proxy
                scene.UnsubscribeEvent<BattleSequenceEnded>(OnBattleSequenceEnded);
                scene.UnsubscribeEvent<BulletHellEndedEvent>(OnBulletHellEnded);
                scene.UnsubscribeEvent<BulletHellReadyEvent>(OnBulletHellReady);
            }

            if (@interface)
            {
                // Scene ===> Proxy
                @interface.UnsubscribeEvent<PlayerCommandEvent>(OnReceivePlayerAction);
                @interface.UnsubscribeEvent<PlayerCancelCommandEvent>(OnCancelPlayerAction);
                @interface.UnsubscribeEvent<AnsFightQuickTimeEvent>(OnReceiveFightQuickTime);
            }
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
        }
        
        private void OnCancelAction(CancelActionEvent evt)
        {
            // Cancel interface action
            scene.OnPlayerCancelAction(evt);
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
        
        #endregion
    }
}
