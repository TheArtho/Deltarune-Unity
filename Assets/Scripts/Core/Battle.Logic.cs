using System;
using System.Collections.Generic;
using System.Linq;
using Core.Combat;
using Core.Combat.Events;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Battle
{
    private PlayerStateEvent GetPlayerState(int playerId)
    {
        return new PlayerStateEvent
        {
            Player = playerId,
            State = new PlayerStateEvent.PlayerState
            {
                Name = players[playerId].name,
                Hp = players[playerId].hp,
                MaxHp = players[playerId].maxHp,
                Attack = players[playerId].attack,
                Defense = players[playerId].defense,
                Magic = players[playerId].magic,
                Actions = enemies.Select(enemy => enemy.GetActionChoice(playerId)).ToArray(),
                Spells = players[playerId].GetSpells(),
                ActionTargets = players[playerId].GetSpellTargets(),
                MagicUser = players[playerId].magicUser
            }
        };
    }

    private GlobalStateEvent GetGlobalState()
    {
        List<GlobalStateEvent.EnemyState> enemieStates = new List<GlobalStateEvent.EnemyState>();
        
        for (var i = 0; i < enemies.Length; i++)
        {
            enemieStates.Add(
                new GlobalStateEvent.EnemyState
                {
                    id = i,
                    Name = enemies[i].NameColored(),
                    Hp = enemies[i].Hp,
                    MaxHp = enemies[i].maxHP,
                    Mercy = enemies[i].mercy,
                    Attack = enemies[i].attack,
                    Defense = enemies[i].defense,
                    Fainted = enemies[i].IsFainted,
                    Spared = enemies[i].IsSpared,
                    Pacified = enemies[i].IsPacified
                });
        }
        
        // TODO Changed hard coded part to dynamic dialog line
        return new GlobalStateEvent
        {
            Ennemies = enemieStates.ToArray(),
            Items = inventory.Select(x => x.GetName()).ToArray(),
            Text = introText,
            ActivePlayers = players
                .Select((p, index) => new { p, index })   // Associate each player with their index
                .Where(x => x.p.hp > 0)                 // Keep players with hp > 0
                .Select(x => x.index).ToArray()
        };
    }

    private List<int> CalculateTargets()
    {
        // Get a list of indices of all alive players (HP > 0)
        List<int> validTargets = Enumerable.Range(0, players.Length)
            .Where(i => players[i].hp > 0)
            .ToList();

        // Count how many enemies are still in battle
        int enemyCount = enemies.Count(e => e.IsActive);

        // If no valid players or no enemies, return an empty target list
        if (validTargets.Count == 0 || enemyCount == 0)
            return new List<int>();

        // If there are more enemies than alive players,
        // all living players become targets
        if (enemyCount >= validTargets.Count)
            return new List<int>(validTargets);

        // Otherwise, randomly select `enemyCount` players from the alive pool
        List<int> selected = new List<int>();
        List<int> pool = new List<int>(validTargets);

        for (int i = 0; i < enemyCount; i++)
        {
            int index = Random.Range(0, pool.Count); // Pick a random index
            selected.Add(pool[index]);               // Add it to the result
            pool.RemoveAt(index);                    // Remove to avoid duplicates
        }

        // selected = new List<int>() { 0, 1, 2 };

        return selected;
    }



    private int CalculateDamageOnEnemy(Player attacker, Enemy target, int accuracy)
    {
        int damage = Mathf.RoundToInt(attacker.attack * ((float)accuracy / 20) - 3 * target.defense);
        
        return Math.Max(1, damage);
    }
    
    private int CalculateDamageOnPlayer(Enemy attacker, Player target)
    {
        int damage = Math.Max(1, 5 * attacker.attack - 3 * target.defense);

        if (target.defending)
        {
            damage = Mathf.RoundToInt(damage * (2f / 3f));
        }

        return damage;
    }
    
    private int GetActionPriority(ActionType type)
    {
        return type switch
        {
            ActionType.Defend => 0,
            ActionType.Spare => 1,
            ActionType.ActMagic => 2,
            ActionType.Item => 3,
            ActionType.Fight => 4,
            _ => 999
        };
    }

    private void CalculateBattleSequence()
    {
        battleSequence.Clear();
        enemySequence.Clear();

        var commands = playerCommandBuffer.Where(cmd => cmd != null).ToArray(); 

        // Step 1 : create a list of indices
        List<int> actionOrder = Enumerable.Range(0, commands.Length).ToList();

        // Step 2 : sort by priority
        actionOrder.Sort((a, b) =>
        {
            int prioA = GetActionPriority(commands[a].ActionType);
            int prioB = GetActionPriority(commands[b].ActionType);
            if (prioA == prioB) return a.CompareTo(b); // tie-breaker = ordre d'entrée
            return prioA.CompareTo(prioB);
        });

        // Step 3 : create a sequence in the sorted order
        foreach (int i in actionOrder)
        {
            players[i].defending = false;

            var action = commands[i];

            switch (action.ActionType)
            {
                case ActionType.ActMagic:
                    if (players[i].magicUser)
                    {
                        players[i].GetSpellAction(action.Index).Execute(action.Player, action.TargetId);
                    }
                    else
                    {
                        enemies[action.TargetId].GetAction(action.Player, action.Index).Execute(action.Player, action.TargetId);
                    }
                    break;
                
                case ActionType.Item:
                    inventory[action.Index].Use(action.Player, action.TargetId);
                    RemoveUsedItems();
                    break;
                
                case ActionType.Spare:
                    if (enemies[action.TargetId].CanSpare())
                    {
                        enemies[action.TargetId].IsSpared = true;
                        
                        battleSequence.Add(new PlayerAnimationSequence
                        {
                            RunInParallel = true,
                            Player = i,
                            Animation = "Act"
                        });
                        battleSequence.Add(new TextSequence
                        {
                            Text = $"{players[action.Player].name} spared {enemies[action.TargetId].name}",
                            Delay = 0.5f
                        });
                        battleSequence.Add(new SpareEnemySequence
                        {
                            RunInParallel = true,
                            Enemy = action.TargetId,
                        });
                    }
                    else
                    {
                        battleSequence.Add(new PlayerAnimationSequence
                        {
                            RunInParallel = true,
                            Player = i,
                            Animation = "Act"
                        });
                        battleSequence.Add(new TextSequence
                        {
                            Text = $"{players[action.Player].name} spared {enemies[action.TargetId].name}",
                            Delay = 0.5f
                        });
                        battleSequence.Add(new TextSequence
                        {
                            Text = $"\nBut its name wasn't <gradient=spare>YELLOW</gradient>...",
                            ClearText = false
                        });
                    }
                    break;
                
                case ActionType.Defend:
                    players[i].defending = true;
                    break;
                
                case ActionType.Fight:
                    // Will likely be handled elsewhere (e.g., QTE)
                    break;
            }

            if (CheckEndBattle())
            {
                break;
            }
        }
    }

    private void RemoveUsedItems()
    {
        inventory.RemoveAll(x => x.Selected);
    }


    private void AddTp(int playerId, int amount)
    {
        int previous = this.tp;
        this.tp = this.tp + amount;
        EmitEvent(new AddTpEvent()
        {
            Player = playerId,
            Amount = amount,
            PreviousValue = previous
        });
    }
    
    private void RemoveTp(int playerId, int amount)
    {
        int previous = this.tp;
        this.tp = this.tp - amount;
        EmitEvent(new RemoveTpEvent()
        {
            Player = playerId,
            Amount = amount,
            PreviousValue = previous
        });
    }

    private List<BattleSequence> CalculateEndBattleSequence()
    {
        List<BattleSequence> sequence = new List<BattleSequence>();
        
        for (var i = 0; i < players.Length; i++)
        {
            sequence.Add(new PlayerAnimationSequence()
            {
                RunInParallel = true,
                Player = i,
                Animation = "EndBattle"
            });
        }
        sequence.Add(new TextSequence()
        {
            Text = "You won!"
        });
        sequence.Add(new TextSequence()
        {
            ClearText = false,
            Text = "\nGot 0 EXP and 0 D$."
        });

        return sequence;
    }
}
