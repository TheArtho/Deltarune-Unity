using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : DialogBox
{
    [SerializeField] private Text asterisk;
    
    public override void Clear()
    {
        base.Clear();
        asterisk.text = "";
    }

    public override IEnumerator DrawText(string text, string soundId, float timePerChar = 0.02f)
    {
        asterisk.text = "*";
        return base.DrawText(text, soundId, timePerChar);
    }

    public override void DrawTextInstant(string text)
    {
        asterisk.text = "*";
        base.DrawTextInstant(text);
    }
}
