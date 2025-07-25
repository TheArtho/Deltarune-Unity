using System;
using System.Collections.Generic;
using System.Linq;
using Client.Combat.Events;
using Core;
using Core.Combat;
using Core.Combat.Actions;
using Core.Combat.Events;
using UnityEngine;

public partial class Battle : IEventSource<IBattleEvent>
{
    private enum BattleState
    {
        Initialized,
        AwaitingForPlayerInput,
        ProcessingTurn,
        AwaitingForActionSequence,
        ExecutingPlayerAction,
        AwaitingForPlayerFightQte,
        AwaitingForPlayerFightQteEnd,
        AwaitingForPlayerActQte,
        AwaitingForPlayersBulletPhaseReady,
        BulletPhase,
        EndOfTurn,
        WaitForEndBattleReady,
        Ended,
        GameOver
    }
    
    // Private parameters
    private bool canLoose;
    private bool canSpare = true;

    private Player[] players;
    private Enemy[] enemies;

    /// <summary>
    /// Value between 0 and 100
    /// </summary>
    private int tp = 0;
    private BattleState state;
    private PlayerCommandEvent[] playerCommandBuffer;
    private AnsFightQuickTimeEvent[] fightCommandBuffer;
    private bool[] battleSequenceReadyBuffer;
    private bool[] bulletHellReadyBuffer;
    private bool[] bulletHellEndedBuffer;
    private bool[] battleEndedBuffer;
    
    private List<BattleSequence> battleSequence = new List<BattleSequence>();
    private List<BattleSequence> enemySequence = new List<BattleSequence>();

    private List<int> targetIndexes = new List<int>();
    
    private EventBus bulletHellEvents = new EventBus();
    private EventBus battleEvents = new EventBus();

    private bool fatal = true;
    private string introText = "...";
    private List<BattleItem> inventory = new List<BattleItem>();
    
    // Public parameters
    public int TurnCount { get; protected set; }

    public Player[] Players => players;

    public Enemy[] Enemies => enemies;

    public List<BattleSequence> BattleSequence => battleSequence;
    public List<BattleSequence> EnemySequence => enemySequence;

    public Battle(Player[] players, Enemy[] enemies, bool canLoose = false, bool canSpare = true)
    {
        this.players = players;
        this.enemies = enemies;
        this.canLoose = canLoose;
        this.canSpare = canSpare;

        foreach (var player in players)
        {
            player.Initialize(this);
        }
        
        foreach (var enemy in enemies)
        {
            enemy.Initialize(this);
        }

        ResetBuffers();
    }

    public void SetInventory(Inventory inventory)
    {
       this.inventory.Clear();
        
        foreach (var itemDefinition in inventory.Items)
        {
            this.inventory.Add(itemDefinition.CreateInstance(this));
        }
    }

    public void SetIntroText(string text)
    {
        this.introText = text;
    }

    private void ResetBuffers()
    {
        playerCommandBuffer = new PlayerCommandEvent[players.Length];
        fightCommandBuffer = new AnsFightQuickTimeEvent[players.Length];
        battleSequenceReadyBuffer = new bool[players.Length];
        bulletHellReadyBuffer = new bool[players.Length];
        bulletHellEndedBuffer = new bool[players.Length];
        battleEndedBuffer = new  bool[players.Length];
    }

    public void Start()
    {
        Debug.Log("[Battle] Initializing Battle...");
        InitializeBattle();
        Debug.Log("[Battle] Battle started.");
        StartTurn();
    }
    
    private void End()
    {
        Debug.Log("[Battle] Battle ended.");
    }

    public void Abort()
    {
        Debug.Log("[Battle] Battle aborted.");
    }

    private void InitializeBattle()
    {
        TurnCount = 0;
        state = BattleState.Initialized;
    }

    private void StartTurn()
    {
        ResetBuffers();
        
        for (var i = 0; i < players.Length; i++)
        {
            if (players[i].hp <= 0)
            {
                int amount = players[i].maxHp / 8 + 1;
                players[i].Heal(amount);
                EmitEvent(new HealPlayerEvent()
                {
                    Player = i,
                    HealAmount = amount,
                    CurrentHp = players[i].hp
                });
            }
        }

        GlobalStateEvent globalStateEvent = GetGlobalState();
        EmitEvent(globalStateEvent);
        EmitEvent(new UpdateInventoryEvent
        {
            items = inventory.Select(i => i.GetName()).ToArray(),
            targetType = inventory.Select(i => i.Target).ToArray(),
            selected = inventory.Select(i => i.Selected).ToArray()
        });
        
        // Send possible actions to players
        for (var i = 0; i < players.Length; i++)
        {
            PlayerStateEvent actionsEvent = GetPlayerState(i);
            EmitEvent(actionsEvent);
        }
        
        Debug.Log($"[Battle] Turn {TurnCount + 1}");
        WaitForPlayerInput();
        EmitEvent(new StartTurnEvent());
        Debug.Log("[Battle] Waiting for player Inputs");
    }

    private void WaitForPlayerInput()
    {
        state = BattleState.AwaitingForPlayerInput;
    }
    
    private void EndTurn()
    {
        TurnCount++;

        if (CheckEndBattle())
        {
            WaitForEndBattleSequence();
        }
        else
        {
            StartTurn();
        }
    }

    public void ReceivePlayerCommand(int playerId, PlayerCommandEvent command)
    {
        if (state != BattleState.AwaitingForPlayerInput) return;
        
        if (playerCommandBuffer[playerId] != null)
        {
            return;
        }
        
        // Check validity of the player command
        if (playerId < 0 || playerId > playerCommandBuffer.Length)
        {
            return;
        }
        
        // Store the player command
        playerCommandBuffer[playerId] = command;
        Debug.Log($"[Battle] Command of player {playerId} received.");
        if (command.ActionType == ActionType.Defend)
        {
            AddTp(playerId, 40);
        }
        else if (command.ActionType == ActionType.Item)
        {
            inventory[command.Index].Selected = true;
            EmitEvent(new UpdateInventoryEvent()
            {
                items = inventory.Select(i => i.GetName()).ToArray(),
                targetType = inventory.Select(i => i.Target).ToArray(),
                selected = inventory.Select(i => i.Selected).ToArray()
            });
        }
        EmitEvent(new ChooseActionEvent() {Player = playerId, ActionType = command.ActionType});
        
        if (AllActivePlayersHaveCommands())
        {
            state = BattleState.ExecutingPlayerAction;
            Debug.Log($"[Battle] All command received. Executing actions...");
            ProcessTurn();
        }
    }
    
    bool AllActivePlayersHaveCommands()
    {
        for (int i = 0; i < players.Length; i++)
        {
            // Skip dead players
            if (players[i].hp <= 0)
                continue;
            
            // Should also check for other status that can make the player inactive

            if (playerCommandBuffer[i] == null)
                return false;
        }

        return true;
    }

    public void CancelPlayerCommand(int playerId)
    {
        if (playerId < 0 || playerId > playerCommandBuffer.Length)
        {
            return;
        }

        if (playerCommandBuffer[playerId] == null)
        {
            return;
        }
        
        Debug.Log($"[Battle] Player {playerId}'s action cancelled.");
        
        if (playerCommandBuffer[playerId].ActionType == ActionType.Item)
        {
            inventory[playerCommandBuffer[playerId].Index].Selected = false;
            EmitEvent(new UpdateInventoryEvent()
            {
                items = inventory.Select(i => i.GetName()).ToArray(),
                targetType = inventory.Select(i => i.Target).ToArray(),
                selected = inventory.Select(i => i.Selected).ToArray()
            });
        }
        
        if (playerCommandBuffer[playerId].ActionType == ActionType.Defend)
        {
            RemoveTp(playerId, 40);
        }
        playerCommandBuffer[playerId] = null;
        EmitEvent(new CancelActionEvent() {Player = playerId});
    }

    private void ProcessFightQte()
    {
        var commands = playerCommandBuffer.Where(cmd => cmd != null).ToArray(); 
        
        // If at least one player is fighting
        if (commands.ToList().Any(x => x.ActionType == ActionType.Fight))
        {
            state = BattleState.AwaitingForPlayerFightQte;
            
            // For each fight action send QTE data
            for (var i = 0; i < commands.Length; i++)
            {
                int playerId = commands[i].Player;
                if (commands[i].ActionType == ActionType.Fight)
                {
                    Debug.Log($"[Battle] Player {playerId} has a Fight QTE.");
                    EmitEvent(new ReqFightQuickTimeDataEvent() {Player = playerId, Delay = 0});
                }
            }
            
            EmitEvent(new FightQteStartEvent());
        }
        else // If no one is fighting continue the turn
        {
            WaitForBulletPhaseReady();
        }
    }
    
    private void ProcessTurn()
    {
        state = BattleState.ProcessingTurn;
        Debug.Log($"[Battle] Processing turn...");
        
        // Clamp the tp value
        tp = Math.Clamp(tp, 0, 100);

        CalculateBattleSequence();
        RemoveUsedItems();
        
        WaitForBattleSequence();
        
        EmitEvent(new PlayBattleSequenceEvent()
        {
            BattleSequence = this.battleSequence
        });
    }

    private void WaitForBattleSequence()
    {
        state = BattleState.AwaitingForActionSequence;
    }

    public void ReceiveBattleSequenceEnded(int player)
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
        if (state != BattleState.AwaitingForActionSequence) return;

        if (player < 0 || player >= playerCommandBuffer.Length) return;
        
        battleSequenceReadyBuffer[player] = true;

        if (battleSequenceReadyBuffer.All(x => x))
        {
            if (CheckEndBattle())
            {
                WaitForEndBattleSequence();
            }
            else
            {
                ProcessFightQte();
            }
        }
    }
    
    public void ReceiveFightQte(AnsFightQuickTimeEvent evt)
    {
        if (state != BattleState.AwaitingForPlayerFightQte) return;
        
        int damage = -1;
        int target = 0;
        int playerId = evt.Player;
        int accuracy = evt.Accuracy;
        PlayerCommandEvent command = playerCommandBuffer[playerId];
        
        if (fightCommandBuffer[playerId] != null)
        {
            Debug.LogWarning($"[Battle] Unwanted Fight QTE from Player {playerId}, Accuracy = {accuracy}");
            return;
        }
        
        Debug.Log($"[Battle] Fight QTE received from Player {playerId}, Accuracy = {accuracy}");

        if (command == null) return;
        
        // Store command in a buffer
        fightCommandBuffer[playerId] = evt;
        // Deal Damage to targeted monster
        target = playerCommandBuffer[playerId].TargetId;
        // Attack not missed
        if (accuracy > 0)
        {
            damage = CalculateDamageOnEnemy(players[playerId], enemies[target], accuracy);
            enemies[target].DealDamage(damage);
            // Damage Event
            EmitEvent(new PlayerAttackEvent()
            {
                Player = playerId,
                Target = target,
                Damage =  damage,
                Fainted = enemies[target].IsFainted,
                Scared = !fatal // Non-fatal attacks will make the enemy scared instead of fainted
            });
        }
        else
        {
            // Miss Event
            Debug.Log("Attack Missed.");
            EmitEvent(new PlayerMissedEvent()
            {
                Player = playerId,
                Target = target
            });
        }
        
        // Check if the command buffer is full
        bool isFightBufferFull = true;
        for (var i = 0; i < playerCommandBuffer.Length; i++)
        {
            if (playerCommandBuffer[i] == null) continue;
            if (playerCommandBuffer[i].ActionType != ActionType.Fight) continue;
            if (fightCommandBuffer[i] == null)
            {
                isFightBufferFull = false;
            }
        }
        // Process the turn if it is the case
        if (isFightBufferFull)
        {
            if (CheckEndBattle())
            {
                WaitForEndBattleSequence();
            }
            else
            {
                WaitForBulletPhaseReady();
            }
        }
    }
    
    private void WaitForBulletPhaseReady()
    {
        List<string> attacks = new List<string>();
        List<int> attackers = new List<int>();
        
        Debug.Log("Waiting for clients to be ready for bullet phase.");
        state = BattleState.AwaitingForPlayersBulletPhaseReady;
        
        targetIndexes = CalculateTargets();
        
        // Send a battle sequence before the bullet hell phase
        // TODO make the enemy sequence modular
        // enemySequence.Add(new EnemyDialogSequence());
        
        for (var i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].IsActive)
            {
                attacks.Add(enemies[i].GetAttackPattern());
                attackers.Add(i);
            }
        }
        
        EmitEvent(new BulletHellWaitReady()
        {
            Attacks = attacks.ToArray(),
            Attackers = attackers.ToArray(),
            BattleSequence = enemySequence,
            Targets = targetIndexes
        });
    }

    public void ReceiveBulletPhaseReady(int playerId)
    {
        if (state != BattleState.AwaitingForPlayersBulletPhaseReady) return;

        if (playerId < 0 || playerId >= bulletHellReadyBuffer.Length) return;
        
        bulletHellReadyBuffer[playerId] = true;
        
        if (bulletHellReadyBuffer.All(x => x))
        {
            // Wait for all players to be ready for bullet phase
            StartBulletPhase();
        }
    }

    private void StartBulletPhase()
    {
        Debug.Log($"[Battle] Bullet Phase Started.");
        state = BattleState.BulletPhase;
        // Start Event
        EmitEvent(new BulletHellStartEvent());
    }

    public void ReceiveBulletPhaseEnded(int playerId)
    {
        if (state != BattleState.BulletPhase) return;
        
        bulletHellEndedBuffer[playerId] = true;

        if (bulletHellEndedBuffer.All(x => x))
        {
            EndBulletPhase();
        }
    }

    private void EndBulletPhase()
    {
        state = BattleState.EndOfTurn;
        Debug.Log($"[Battle] Bullet Phase Ended.");
        // Destroy bullet hell area
        EndTurn();
    }
    
    private bool CheckEndBattle()
    {
        // TODO Check specific end conditions for each enemies or in a scripted battle
        return enemies.All(x => x.IsFainted || x.IsSpared || x.IsPacified);
    }

    private void WaitForEndBattleSequence()
    {
        Debug.Log("Waiting for clients to finish the end battle sequence.");
        state = BattleState.WaitForEndBattleReady;
        
        EmitEvent(new PlayEndBattleSequenceEvent()
        {
            sequence = CalculateEndBattleSequence()
        });
    }
    
    public void ReceiveEndBattleReady(int playerId)
    {
        if (state != BattleState.WaitForEndBattleReady) return;

        if (playerId < 0 || playerId >= battleEndedBuffer.Length) return;

        if (battleEndedBuffer[playerId]) return;
        
        battleEndedBuffer[playerId] = true;
        
        if (battleEndedBuffer.All(x => x))
        {
            // Wait for all players to be ready to end the battle
            EndBattle();
        }
    }

    private void EndBattle()
    {
        Debug.Log("End of the battle.");
        state = BattleState.Ended;
        EmitEvent(new EndBattleEvent()
        {
            
        });
    }
    
    // Battle Event
    
    public void SubscribeEvent<T>(Action<T> callback) where T : class, IBattleEvent
    {
        battleEvents.Subscribe(callback);
    }

    public void UnsubscribeEvent<T>(Action<T> callback) where T : class,  IBattleEvent
    {
        battleEvents.Unsubscribe(callback);
    }

    public void EmitEvent<T>(T evt) where T : class, IBattleEvent
    {
        battleEvents.Emit(evt);
    }
}
