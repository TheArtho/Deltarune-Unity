using System.Collections.Generic;
using Client.Combat;
using Client.Combat.UI;
using Core.Combat;
using Scriptables;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    [SerializeField] BattleDefinition battleDefinition;
    
    [Space]
    
    [SerializeField] BattleProxy proxy;
    
    private Battle battle;
    
    void Start()
    {
        StartBattle(battleDefinition);
    }

    public void StartBattle(BattleDefinition battleDef)
    {
        List<Player> players = new List<Player>();
        
        foreach (var def in battleDef.players)
        {
            players.Add(new Player(def));
        }

        List<Enemy> enemies = new List<Enemy>();
            
        foreach (var def in battleDef.enemies)
        {
            enemies.Add(new Enemy(def));
        }

        battle = new Battle(players.ToArray(), enemies.ToArray());
        battle.SetIntroText(battleDef.introText);
            
        proxy.Init(battle);
        battle.Start();
        
        BgmHandler.PlayMain(battleDef.music);
    }
}
