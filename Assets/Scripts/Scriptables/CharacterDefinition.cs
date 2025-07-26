using System.Collections.Generic;
using Scriptables;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacter", menuName = "Scriptable Objects/PlayerCharacter")]
public class CharacterDefinition : ScriptableObject
{
    [SerializeField]
    public string characterId;
    
    [Header("Battle Assets")]
    [SerializeField]
    public BattleSprite battlerPrefab;
    [SerializeField]
    public PlayerBattleMenu battleMenuPrefab;
    
    [Header("Battle Parameters")]
    [SerializeField]
    public string name;
    
    [Header("Battle Stats")]
    [SerializeField]
    public int hp;
    [SerializeField]
    public int attack;
    [SerializeField]
    public int defense;
    [SerializeField]
    public int magic;
    [SerializeField] 
    public bool magicUser;
    
    [Space]
    
    [SerializeField] 
    public List<ActionDefinition> spells;
}
