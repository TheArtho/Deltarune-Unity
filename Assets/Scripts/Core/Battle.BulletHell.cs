using System;
using System.Linq;
using Client.Combat.Events;
using Core.Combat.Events;
using UnityEngine;

public partial class Battle
{
    public void Graze(GrazeEvent evt)
    {
        AddTp(evt.Player, 1);
        Math.Clamp(tp, 0, 100);
    }

    public void PlayerHurt(PlayerHurtEvent evt)
    {
        if (!targetIndexes.Contains(evt.Player))
        {
            return;
        }
        
        Player player = players[evt.Player];
        
        Debug.Log($"Player Hurt : id {evt.Player}");
        
        player.Damage(evt.Damage);

        EmitEvent(new DamagePlayerEvent()
        {
            Player = evt.Player,
            currentHp = player.hp,
            maxHp = player.maxHp,
        });
        
        // Check for downed status
        if (player.hp <= 0)
        {
            EmitEvent(new KnockOutEvent()
            {
                Player = evt.Player
            });
        }
        // Check for Game Over
        if (players.All(p => p.hp <= 0))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        EmitEvent(new GameOverEvent());
    }
}
