using System;
using System.Collections.Generic;
using System.Linq;
using Core.Combat.Actions;
using Scriptables;

public class Player
{
    public string characterId { get; private set; }
    public string name { get; private set; }
    public int hp { get; private set; }
    public int maxHp { get; private set; }
    public int attack { get; private set; }
    public int defense { get; private set; }
    public int magic { get; private set; }

    public bool magicUser { get; private set; }

    public bool defending;

    private List<ActionDefinition> spellDefinitions = new List<ActionDefinition>();
    private List<BattleAction> spellActions = new List<BattleAction>();

    private Battle battle;

    public Player(string characterId, string name, int hp, int maxHp, int attack, int defense, int magic, bool magicUser)
    {
        this.characterId = characterId;
        this.name = name;
        this.maxHp = maxHp;
        this.hp = Math.Min(maxHp, hp);
        this.attack = attack;
        this.defense = defense;
        this.magic = magic;
        this.magicUser =  magicUser;
    }

    public Player(CharacterDefinition def)
        : this(def.characterId, def.name, def.hp, def.hp, def.attack, def.defense, def.magic, def.magicUser)
    {
        // Store action definitions for later
        foreach (var spell in def.spells)
        {
            spellDefinitions.Add(spell);
        }
    }
    
    public void Initialize(Battle battle)
    {
        this.battle = battle;
        
        spellActions.Clear();
                  
        foreach (var s in spellDefinitions)
        {
            this.spellActions.Add(s.CreateInstance(battle));
        }
    }
    
    public BattleAction GetSpellAction(int index)
    {
        return spellActions[index];
    }

    public string[] GetSpells()
    {
        return spellActions.Select(x => x.GetName()).ToArray();
    }
    
    public BattleAction.SpellTargetType[] GetSpellTargets()
    {
        return spellActions.Select(x => x.TargetTypeType).ToArray();
    }

    public void Down()
    {
        hp = -1 * maxHp / 2;
    }

    public void Heal(int amount)
    {
        hp = Math.Clamp(hp + amount, -1 * maxHp / 2, maxHp);
    }
    
    public void Damage(int amount)
    {
        hp = Math.Clamp(hp - amount, -1 * maxHp / 2, maxHp);
    }
}