using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    [SerializeField] private Text textObject;
    private string _soundId;
    private PlayerInputAction _inputAction;
    private bool _canSkip;
    private bool _waitInput;
    private bool _skip;
    private string _currentText;
    private bool _next; 
    
    private void Awake()
    {
        _inputAction = new PlayerInputAction();

        _inputAction.Battle.Select.performed += ctx =>
        {
            if (!_waitInput) return;
        };

        _inputAction.Battle.Cancel.performed += ctx =>
        {
            if (!_canSkip) return;
            
            _skip = true;
            DrawTextInstant(_currentText);
            DisableInput();
        };
        
        Clear();
    }

    public void EnableInput()
    {
        _inputAction.Enable();
    }
    
    public void DisableInput()
    {
        _inputAction.Disable();
    }

    public void SetTypingSound(string soundId = "")
    {
        this._soundId = soundId;
    }

    public virtual void Clear()
    {
        textObject.text = "";
    }
    
    public virtual IEnumerator DrawText(string text, string soundId, float timePerChar = 0.02f)
    {
        _currentText = text;
        _skip = false;
        SetTypingSound(soundId);

        foreach (char c in text)
        {
            if (_skip) break;
            
            textObject.text += c;
            SfxHandler.Play(_soundId);
            yield return new WaitForSeconds(timePerChar <= 0 ? 0.1f : timePerChar);
        }

        if (_waitInput)
        {
            yield return new WaitUntil(() => _next);
        }
    }

    public IEnumerator DrawText(string text, float timePerChar = 0.1f)
    {
        return DrawText(text, _soundId, timePerChar);
    }
    
    public virtual void DrawTextInstant(string text)
    {
        textObject.text = text;
    }
}
