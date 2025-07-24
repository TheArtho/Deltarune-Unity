using System;
using System.Collections.Generic;
using System.Linq;
using Client.Combat.Events;
using Core.Combat;
using Core.Combat.Events;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Battle
{
    private PlayerStateEvent GetPlayerState(int player)
    {
        string[] actions = new [] {""};
        
        // TODO Get player actions depending on the enemy

        switch (player)
        {
            // Kris
            case 0:
                actions = new string[]
                {
                    "Check", "Talk"
                };
                break;
            // Susie
            case 1:
                actions = new string[]
                {
                    "Rude Buster"
                };
                break;
            // Ralsei
            case 2:
                actions = new string[]
                {
                    "Pacify", "Heal Prayer"
                };
                break;
        }
        
        return new PlayerStateEvent
        {
            Player = player,
            State = new PlayerStateEvent.PlayerState
            {
                Name = players[player].name,
                Hp = players[player].hp,
                MaxHp = players[player].maxHp,
                Attack = players[player].attack,
                Defense = players[player].defense,
                Magic = players[player].magic,
                Actions = actions
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
                    id = i + 4,
                    Name = enemies[i].name,
                    Hp = enemies[i].Hp,
                    MaxHp = enemies[i].maxHP,
                    Mercy = enemies[i].mercy,
                    Attack = enemies[i].attack,
                    Defense = enemies[i].defense
                });
        }
        
        // TODO Changed hard coded part to dynamic inventory and dialog line
        return new GlobalStateEvent
        {
            Ennemies = enemieStates.ToArray(),
            Items = new string[]
            {
                "Darkburger", "Light Candy", "Java Cookie"
            },
            Text = "It's the freaking Roaring\nKnight!!!"
        };
    }

    private List<int> CalculateTargets()
    {
        // Get a list of indices of all alive players (HP > 0)
        List<int> validTargets = Enumerable.Range(0, players.Length)
            .Where(i => players[i].hp > 0)
            .ToList();

        // Count how many enemies are still in battle
        int enemyCount = enemies.Count(e => e.IsInBattle);

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

    private List<BattleSequence> CalculateBattleSequence()
    {
        battleSequence.Clear();
        
        for (var i = 0; i < playerCommandBuffer.Length; i++)
        {
            int target = 0;
            int damage = -1;
            int damagePercentage = 0;
            players[i].defending = false;
            
            switch (playerCommandBuffer[i].ActionType)
            {
                // Act/Magic
                case ActionType.ActMagic:
                    if (i == 0) // Kris
                    {
                        battleSequence.Add(new PlayerAnimationSequence()
                        {
                            RunInParallel = true,
                            character = 0,
                            animation = "Act"
                        });
                        battleSequence.Add(new TextSequence
                        {
                            RunInParallel = false,
                            text = "Kris is acting."
                        });
                    }
                    break;
                // Use Item
                case ActionType.Item:
                    target = playerCommandBuffer[i].TargetId;
                    break;
                // Spare
                case ActionType.Spare:
                    break;
                // Defend
                case ActionType.Defend:
                    players[i].defending = true;
                    break;
            }
        }
        
        // Sort by priority order

        return battleSequence;
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
}
