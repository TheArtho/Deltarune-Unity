using System;

public class Player
{
    public string characterId { get; private set; }
    public string name { get; private set; }
    public int hp { get; private set; }
    public int maxHp { get; private set; }
    public int attack { get; private set; }
    public int defense { get; private set; }
    public int magic { get; private set; }

    public bool defending;

    public Player(string characterId, string name, int hp, int maxHp, int attack, int defense, int magic)
    {
        this.characterId = characterId;
        this.name = name;
        this.maxHp = maxHp;
        this.hp = Math.Min(maxHp, hp);
        this.attack = attack;
        this.defense = defense;
        this.magic = magic;
    }

    public Player(CharacterDefinition def)
    : this(def.characterId, def.name, def.hp, def.hp, def.attack, def.defense, def.magic)
    { } 

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