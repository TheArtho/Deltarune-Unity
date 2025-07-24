using System;
using Client.Combat.UI;
using Core.Combat.Events;
using UnityEngine;
using UnityEngine.Serialization;

public class BattleSprite : MonoBehaviour
{
    public int playerId;
    private Animator _animator;
    [FormerlySerializedAs("_spriteRenderer")] [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform damageTextAnchor;
    private bool defending;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ResetAnimations()
    {
        Play("Idle");
    }

    public void OnPlayerChooseAction(ChooseActionEvent evt)
    {
        if (playerId != evt.Player) return;
        defending = false;
        
        switch (evt.ActionType)
        {
            case ActionType.Fight:
                Play("PrepareFight");
                break;
            case ActionType.ActMagic:
                Play("PrepareActMagic");
                break;
            case ActionType.Item:
                Play("PrepareItem");
                break;
            case ActionType.Defend:
                Debug.Log("test2");
                Play("Defend");
                defending = true;
                break;
        }

        try
        {
            _animator.SetBool("defending", defending);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void OnPlayerCancelAction(CancelActionEvent evt)
    {
        if (playerId == evt.Player)
        {
            Play("Idle");
        }
    }

    public void OnBulletHellStart()
    {
        if (!defending)
        {
            Play("Idle");
        }
    }

    public void Darken()
    {
        LeanTween.value(gameObject, spriteRenderer.color, Color.gray, 0.8f).setOnUpdate(color =>
        {
            spriteRenderer.color = color;
        }).setEaseOutQuad();
    }
    
    public void Lighten()
    {
        LeanTween.value(gameObject, spriteRenderer.color, Color.white, 0.8f).setOnUpdate(color =>
        {
            spriteRenderer.color = color;
        }).setEaseOutQuad();
    }

    public void Play(string animation)
    {
        _animator.Play(animation);
    }
}
