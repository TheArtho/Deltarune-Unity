using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacter", menuName = "Scriptable Objects/PlayerCharacter")]
public class PlayerCharacterSO : ScriptableObject
{
    [Header("Battle Assets")]
    [SerializeField]
    private GameObject battlerPrefab;
    [SerializeField]
    private Sprite battleLogo;
    
    [Header("Battle Parameters")]
    [SerializeField]
    private string battleName;
    [SerializeField]
    private bool hasMagic;
    [SerializeField] 
    private Color color1;
    [SerializeField] 
    private Color color2;
    [SerializeField] 
    private Color color3;
    
    [Space]
    
    [Header("Battle Stats")]
    [SerializeField]
    private int hp;
    [SerializeField]
    private int attack;
    [SerializeField]
    private int defense;
    [SerializeField]
    private int magic;
}
