using System;
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
        
    }
}
