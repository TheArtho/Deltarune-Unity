using System;
using UnityEditor.Rendering;

public class Player
{
    public string name { get; private set; }
    public int hp { get; private set; }
    public int maxHp { get; private set; }
    public int attack { get; private set; }
    public int defense { get; private set; }
    public int magic { get; private set; }

    public bool defending;

    public Player(string name, int hp, int maxHP, int attack, int defense, int magic)
    {
        this.name = name;
        this.maxHp = maxHP;     // hard coded value
        this.hp = Math.Min(maxHP, hp);
        this.attack = attack;
        this.defense = defense;
        this.magic = magic;
    }

    public void Heal(int amount)
    {
        hp = Math.Clamp(hp + amount, -9999, maxHp);
    }
    
    public void Damage(int amount)
    {
        hp = Math.Clamp(hp - amount, -9999, maxHp);
    }
}