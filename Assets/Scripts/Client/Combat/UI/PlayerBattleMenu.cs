using Client.Combat.UI;
using Core.Combat.Events;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBattleMenu : MonoBehaviour
{
    [SerializeField] private CharacterBattleInfo playerInfo;
    [SerializeField] private BattleChoice battleChoice;

    public CharacterBattleInfo PlayerInfo => playerInfo;
    public BattleChoice BattleChoice => battleChoice;
    
    public void UpdateInfo(PlayerStateEvent evt)
    {
        playerInfo.UpdateHp(evt.State.Hp, evt.State.MaxHp);
        playerInfo.SetName(evt.State.Name);
    }
}
