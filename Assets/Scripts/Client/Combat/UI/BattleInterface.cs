using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Client.Combat.Events;
using Core.Combat.Events;
using UnityEngine;

namespace Client.Combat.UI
{
    public class BattleInterface : MonoBehaviour, IEventSource<IBattleInterfaceEvent>
    {
        public static BattleInterface Instance;
        
        [SerializeField] private List<PlayerBattleMenu> playerMenus;
        [SerializeField] private List<int> controlledPlayerIds;
        [SerializeField] private List<int> alivePlayerIds;
        [SerializeField] private BattleSubMenu subMenu;
        [SerializeField] private BattleEnemyMenu enemyMenu;
        [SerializeField] private GameObject fightInterface;
        [SerializeField] private List<FightBar> fightBars;
        [SerializeField] private DialogBox dialogBox;
        [SerializeField] private List<DialogBox> playerDialogBoxes;
        [SerializeField] private List<DialogBox> enemyDialogBoxes;
        [SerializeField] private TpBar tpBar;
        [SerializeField] private List<GameObject> targets;
        [SerializeField] private List<GameObject> damageIndicators;
        
        private EventBus events = new EventBus();

        public GlobalStateEvent GlobalStateEvent;
        public PlayerStateEvent[] PlayerStateEvents;
        public ReqFightQuickTimeDataEvent[] FightQteDataEvents;

        public DialogBox DialogBox => dialogBox;
        public List<DialogBox> PlayerDialogBoxes => playerDialogBoxes;
        public List<DialogBox> EnemyDialogBoxes => enemyDialogBoxes;

        public List<GameObject> DamageIndicators => damageIndicators;

        public TpBar TpBar => tpBar;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                PlayerStateEvents = new PlayerStateEvent[playerMenus.Count];
                FightQteDataEvents = new ReqFightQuickTimeDataEvent[playerMenus.Count];
            }
            else
            {
                Destroy(gameObject);
            }

            Initialize();
        }

        public void Initialize()
        {
            // Delete duplicates in the controlled player list
            controlledPlayerIds = controlledPlayerIds.Distinct().ToList();
            // TODO complete the Initialize method
            // Spawns custom dialog boxes for players and enemies if needed
        }

        public void UpdateGlobalState(GlobalStateEvent evt)
        {
            GlobalStateEvent = evt;
            alivePlayerIds = evt.activePlayers.Distinct().ToList();
        }
        
        public void UpdatePlayerState(PlayerStateEvent evt)
        {
            PlayerStateEvents[evt.Player] = evt;
            playerMenus[evt.Player].UpdateInfo(evt);
        }

        public void StartTurn()
        {
            foreach (var playerBattleMenu in playerMenus)
            {
                playerBattleMenu.PlayerInfo.SetLogo(-1);
            }
            StartCoroutine(nameof(ActionSelect));
        }
        
        public void StartFightQte()
        {
            StartCoroutine(nameof(FightQte));
        }

        public void EnableFightQte(ReqFightQuickTimeDataEvent evt)
        {
            FightQteDataEvents[evt.Player] = evt;
        }

        private IEnumerator ActionSelect()
        {
            yield return StartCoroutine(dialogBox.DrawText(GlobalStateEvent.Text, "text"));
            var players = alivePlayerIds.Intersect(controlledPlayerIds).ToList();
           
            for (int i = 0; i < players.Count; i++)
            {
                var info = playerMenus[players[i]].PlayerInfo;
                var battleChoice = playerMenus[players[i]].BattleChoice;
                int index = i;
                int playerId = players[i];
                bool cancelled = false;
                
                Debug.Log($"[Battle Interface] Choosing action for {PlayerStateEvents[players[i]].State.Name}");
                
                Action<PlayerCommandEvent> onSelect = value =>
                {
                    // Start sub-menu of the action
                    info.Unselect();
                    value.Player = playerId;
                    EmitEvent<PlayerCommandEvent>(value);
                };

                Action onCancel = () =>
                {
                    // Not applicable to first player
                    if (index <= 0) return;
                    
                    // Go back to previous menu
                    cancelled = true;
                    info.Unselect();
                    
                    EmitEvent<PlayerCancelCommandEvent>(new PlayerCancelCommandEvent()
                    {
                        Player = playerId - 1
                    });
                };
                
                // Activate menu GameObject
                info.Select();
                battleChoice.EnableInput();
                battleChoice.playerId = index;
                // First player can't cancel
                battleChoice.canCancel = index > 0;
                // Subscribe events
                battleChoice.OnSelect += onSelect;
                battleChoice.OnCancel += onCancel;
                // Wait until deactivation
                yield return new WaitUntil(() => !info.Selected);
                // Unsubscribe events
                battleChoice.OnSelect -= onSelect;
                battleChoice.OnCancel -= onCancel;
                // Decrease iteration if cancelled
                if (cancelled)
                {
                    i -= 2;
                }
            }
        }

        public IEnumerator SubMenuSelect(string[] options, Action<int> result = null)
        {
            return SubMenuSelect(options, new string[options.Length],  result);
        }

        public IEnumerator SubMenuSelect(string[] options, string[] descriptions, Action<int> result = null)
        {
            subMenu.canCancel = true;
            subMenu.options = options.ToList();
            subMenu.gameObject.SetActive(true);

            yield return null;
            
            subMenu.EnableInput();
            subMenu.UpdateButtons();

            Action<int> onSelect = value =>
            {
                result?.Invoke(value);
                subMenu.gameObject.SetActive(false);
            };

            Action onCancel = () =>
            {
                result?.Invoke(-1);
                subMenu.gameObject.SetActive(false);
            };

            subMenu.OnSelect += onSelect;
            subMenu.OnCancel += onCancel;
            
            yield return new WaitUntil(() => !subMenu.gameObject.activeSelf);
            
            subMenu.OnSelect -= onSelect;
            subMenu.OnCancel -= onCancel;
        }

        public IEnumerator EnemySelect(BattleEnemyMenu.EnemyData[] options, Action<int> result = null)
        {
            enemyMenu.canCancel = true;
            enemyMenu.options = options.ToList();
            enemyMenu.gameObject.SetActive(true);

            yield return null;
            
            enemyMenu.EnableInput();
            enemyMenu.UpdateButtons();

            Action<int> onSelect = value =>
            {
                result?.Invoke(value);
                enemyMenu.gameObject.SetActive(false);
            };

            Action onCancel = () =>
            {
                result?.Invoke(-1);
                enemyMenu.gameObject.SetActive(false);
            };

            enemyMenu.OnSelect += onSelect;
            enemyMenu.OnCancel += onCancel;
            
            yield return new WaitUntil(() => !enemyMenu.gameObject.activeSelf);
            
            enemyMenu.OnSelect -= onSelect;
            enemyMenu.OnCancel -= onCancel;
        }

        private IEnumerator FightQte()
        {
            List<Action<int>> onPressList = new List<Action<int>>();
            
            dialogBox.Clear();
            fightInterface.SetActive(true);

            List<FightBar> fightBars = new List<FightBar>();
            List<ReqFightQuickTimeDataEvent> fightDatas = new List<ReqFightQuickTimeDataEvent>();
            
            for (var i = 0; i < this.fightBars.Count; i++)
            {
                if (FightQteDataEvents[i] == null)
                {
                    this.fightBars[i].gameObject.SetActive(false);
                    continue;
                }
                fightBars.Add(this.fightBars[i]);
                fightDatas.Add(FightQteDataEvents[i]);
            }
            
            for (int i = 0; i < fightBars.Count; i++)
            {
                int index = i;
                Action<int> onPress = value =>
                {
                    EmitEvent(new AnsFightQuickTimeEvent()
                    {
                        Player = fightDatas[index].Player,
                        Accuracy = value
                    });
                    
                    Debug.Log("index is " + index);

                    if (index < fightBars.Count - 1)
                    {
                        fightBars[index + 1].CanPress();
                    }
                };
                
                onPressList.Add(onPress);
                
                fightBars[i].gameObject.SetActive(true);
                fightBars[i].EnableInput();

                fightBars[i].OnPress += onPress;
                
                fightBars[i].StartQte(index * 300, index == 0);
            }
            
            yield return new WaitUntil(() => fightBars.All(x => x.done));

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < fightBars.Count; i++)
            {
                Action<int> onPress = onPressList[i];
                
                fightBars[i].OnPress -= onPress;
                
                fightBars[i].DisableInput();
                fightBars[i].gameObject.SetActive(false);
            }
            
            fightInterface.SetActive(false);
            FightQteDataEvents = new ReqFightQuickTimeDataEvent[playerMenus.Count];
        }

        public void ShowTargets(List<int> targetIndexes)
        {
            foreach (var index in targetIndexes)
            {
                targets[index].SetActive(true);
            }
        }

        public void HideTargets()
        {
            foreach (var target in targets)
            {
                target.SetActive(false);
            }
        }

        public void OnPlayerChooseAction(ChooseActionEvent evt)
        {
            playerMenus[evt.Player].PlayerInfo.SetLogo((int) evt.ActionType - 1);
        }
        
        public void OnPlayerCancelAction(CancelActionEvent evt)
        {
            playerMenus[evt.Player].PlayerInfo.SetLogo(-1);
        }
        
        public void OnDamagePlayerEvent(DamagePlayerEvent evt)
        {
            playerMenus[evt.Player].UpdateHp(evt.currentHp, evt.maxHp);
        }
        
        #region Interface Events
    
        public void SubscribeEvent<T>(Action<T> callback) where T : class, IBattleInterfaceEvent
        {
            events.Subscribe(callback);
        }

        public void UnsubscribeEvent<T>(Action<T> callback) where T : class,  IBattleInterfaceEvent
        {
            events.Unsubscribe(callback);
        }

        private void EmitEvent<T>(T evt) where T : class, IBattleInterfaceEvent
        {
            events.Emit(evt);
        }
        
        #endregion
    }
}
