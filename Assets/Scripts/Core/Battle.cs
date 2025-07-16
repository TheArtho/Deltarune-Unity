using System;
using System.Collections.Generic;
using System.Linq;
using Client.Combat.Events;
using Core.Combat;
using Core.Combat.Events;
using UnityEngine;

public partial class Battle
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
        Won,
        Lost
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
    
    private List<BattleSequence> battleSequence = new List<BattleSequence>();
    
    private EventBus bulletHellEvents = new EventBus();
    private EventBus battleEvents = new EventBus();
    
    // Public parameters
    public int TurnCount { get; protected set; }

    public Battle(Player[] players, Enemy[] enemies, bool canLoose = false, bool canSpare = true)
    {
        this.players = players;
        this.enemies = enemies;
        this.canLoose = canLoose;
        this.canSpare = canSpare;

        ResetBuffers();
    }

    private void ResetBuffers()
    {
        playerCommandBuffer = new PlayerCommandEvent[players.Length];
        fightCommandBuffer = new AnsFightQuickTimeEvent[players.Length];
        battleSequenceReadyBuffer = new bool[players.Length];
        bulletHellReadyBuffer = new bool[players.Length];
        bulletHellEndedBuffer = new bool[players.Length];
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

        GlobalStateEvent globalStateEvent = GetGlobalState();
        EmitEvent(globalStateEvent);
        
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

        StartTurn();
    }

    public void ReceivePlayerCommand(int playerId, PlayerCommandEvent command)
    {
        if (state != BattleState.AwaitingForPlayerInput) return;
        
        // Check validity of the player command
        if (playerId < 0 || playerId > playerCommandBuffer.Length)
        {
            EmitEvent(new CancelActionEvent() {Player = playerId});
            return;
        }
        
        // Store the player command
        playerCommandBuffer[playerId] = command;
        Debug.Log($"[Battle] Command of player {playerId} received.");
        EmitEvent(new ChooseActionEvent() {Player = playerId, ActionType = command.ActionType});
        
        if (playerCommandBuffer.All(playerCommand => playerCommand != null))
        {
            state = BattleState.ExecutingPlayerAction;
            Debug.Log($"[Battle] All command received. Executing actions...");
            ProcessTurn();
        }
    }

    public void CancelPlayerCommand(int playerId)
    {
        if (playerId < 0 || playerId > playerCommandBuffer.Length)
        {
            return;
        }
        
        Debug.Log($"[Battle] Player {playerId}'s action cancelled.");
        
        playerCommandBuffer[playerId] = null;
        EmitEvent(new CancelActionEvent() {Player = playerId});
    }

    private void ProcessFightQte()
    {
        // If at least one player is fighting
        if (playerCommandBuffer.ToList().Any(x => x.ActionType == ActionType.Fight))
        {
            state = BattleState.AwaitingForPlayerFightQte;
            
            // For each fight action send QTE data
            for (var i = 0; i < playerCommandBuffer.Length; i++)
            {
                if (playerCommandBuffer[i].ActionType == ActionType.Fight)
                {
                    Debug.Log($"[Battle] Player {i} has a Fight QTE.");
                    EmitEvent(new ReqFightQuickTimeDataEvent() {Player = i, Delay = 0});
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

        CalculateBattleSequence();
        
        WaitForBattleSequence();
        
        EmitEvent(new PlayBattleSequenceEvent()
        {
            battleSequence = this.battleSequence
        });
    }

    private void WaitForBattleSequence()
    {
        state = BattleState.AwaitingForActionSequence;
    }

    public void ReceiveBattleSequenceEnded(int player)
    {
        if (state != BattleState.AwaitingForActionSequence) return;
        
        battleSequenceReadyBuffer[player] = true;

        if (battleSequenceReadyBuffer.All(x => x))
        {
            ProcessFightQte();
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
            if (playerCommandBuffer[i].ActionType != ActionType.Fight) continue;
            if (fightCommandBuffer[i] == null)
            {
                isFightBufferFull = false;
            }
        }
        // Process the turn if it is the case
        if (isFightBufferFull)
        {
            WaitForBulletPhaseReady();
        }
    }
    private void WaitForBulletPhaseReady()
    {
        Debug.Log("Waiting for clients to be ready for bullet phase.");
        state = BattleState.AwaitingForPlayersBulletPhaseReady;
        // Send a battle sequence before the bullet hell phase
        // TODO change hard coded values
        EmitEvent(new BulletHellWaitReady()
        {
            battleMode = "base",
            attacks = new string[] {"Test Attack"},
            battleSequence = new List<BattleSequence>()
        });
    }

    public void ReceiveBulletPhaseReady(int playerId)
    {
        // if (state != BattleState.AwaitingForPlayersBulletPhaseReady) return;
        
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
    
    // Bullet Hell Events

    public void SubscribeBulletHellEvent<T>(Action<T> callback) where T : class, IBulletHellEvent
    {
        bulletHellEvents.Subscribe(callback);
    }

    public void UnsubscribeBulletHellEvent<T>(Action<T> callback) where T : class, IBulletHellEvent
    {
        bulletHellEvents.Unsubscribe(callback);
    }

    private void EmitBulletHellEvent<T>(T evt) where T : class, IBulletHellEvent
    {
        bulletHellEvents.Emit(evt);
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

    private void EmitEvent<T>(T evt) where T : class, IBattleEvent
    {
        battleEvents.Emit(evt);
    }
}
