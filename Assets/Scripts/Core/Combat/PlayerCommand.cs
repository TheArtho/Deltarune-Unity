using UnityEngine;

public class PlayerCommand
{
    public int PlayerId;
    public ActionType ActionType;
    public int TargetId;
    public int Index;   // The index of the action for act, magic and item (example: index of the action)
}

public enum ActionType
{
    None = 0,
    Fight = 1,
    ActMagic = 2,
    Item = 3,
    Spare = 4,
    Defend = 5
}