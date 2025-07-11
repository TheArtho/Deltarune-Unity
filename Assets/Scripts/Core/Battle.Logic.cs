using System;
using System.Collections.Generic;
using System.Linq;
using Core.Combat;
using Core.Combat.Events;
using UnityEngine;

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
                MaxHp = players[player].maxHP,
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

    private List<IBattleSequence> CalculateBattleSequence()
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
}
