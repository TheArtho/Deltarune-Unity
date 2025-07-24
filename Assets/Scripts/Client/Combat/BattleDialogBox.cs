using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : DialogBox
{
    [SerializeField] private TextMeshProUGUI asterisk;
    
    public override void Clear()
    {
        base.Clear();
        asterisk.text = "";
    }

    public override IEnumerator DrawText(string text, string soundId, float timePerChar = 0.02f)
    {
        if (text.Length == 0) yield break;
            
        // Add the asterisk on the current line
        asterisk.text += "*\n";
        
        //TODO Count the number of \n to fit asterisks with the line
        
        yield return StartCoroutine(base.DrawText(text, soundId, timePerChar));
    }


    public override void DrawTextInstant(string text)
    {
        asterisk.text = "*";
        base.DrawTextInstant(text);
    }
}
