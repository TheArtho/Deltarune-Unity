using System;
using System.Collections;
using Client.Combat.UI;
using Core.Combat.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BattleSprite : MonoBehaviour
{
    public int playerId;
    private Animator _animator;
    [FormerlySerializedAs("_spriteRenderer")] [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform damageTextAnchor;
    [SerializeField] private ParticleSystem healParticles;
    [SerializeField] private Color additiveColor = new Color(1,1,1, 0);
    private bool defending;
    private bool down;
    private int damageCounter = 0;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ResetAnimations()
    {
        if (!down)
        {
            Play("Idle");
        }
        damageCounter = 0;
    }

    public void Select()
    {
        StartCoroutine("SelectIE");
    }

    public void Deselect()
    {
        StopCoroutine("SelectIE");
        LeanTween.cancel(gameObject);
        additiveColor = new Color(1,1,1, 0);
    }

    private IEnumerator SelectIE()
    {
        while (true)
        {
            LeanTween.value(gameObject, additiveColor, Color.white, 0.6f).setOnUpdate(color =>
            {
                additiveColor = color;
            }).setEaseInOutQuad().setLoopPingPong(1);
            yield return new WaitForSeconds(1.2f);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    public void SetDowned(bool isDowned)
    {
        down = isDowned;
        _animator.SetBool("down", isDowned);
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

    public IEnumerator ShowPlayerDamageIE(string damage)
    {
        BattleInterface.Instance.DamageIndicators[playerId].gameObject.SetActive(false);
        BattleInterface.Instance.DamageIndicators[playerId].gameObject.SetActive(true);
        BattleInterface.Instance.DamageIndicators[playerId].transform.Find("Damage_Text").GetComponent<Text>().text =
            damage;
        // Set position
        if (Camera.main)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(damageTextAnchor.transform.position);
            BattleInterface.Instance.DamageIndicators[playerId].GetComponent<RectTransform>().position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        }
        yield return null;
    }
    
    public IEnumerator ShowPlayerHealIE(string healAmount)
    {
        LeanTween.value(gameObject, additiveColor, Color.white, 0.4f).setOnUpdate(color =>
        {
            additiveColor = color;
        }).setEaseOutQuad().setLoopPingPong(1);
        healParticles?.Play();
        BattleInterface.Instance.HealIndicators[playerId].gameObject.SetActive(false);
        BattleInterface.Instance.HealIndicators[playerId].gameObject.SetActive(true);
        BattleInterface.Instance.HealIndicators[playerId].transform.Find("Damage_Text").GetComponent<Text>().text =
            healAmount;
        // Set position
        if (Camera.main)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(damageTextAnchor.transform.position);
            BattleInterface.Instance.HealIndicators[playerId].GetComponent<RectTransform>().position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        }
        yield return null;
    }
    
    public IEnumerator ShowEnemyDamageIE(int playerId, string damage)
    {
        BattleInterface.Instance.DamageIndicators[playerId].gameObject.SetActive(false);
        BattleInterface.Instance.DamageIndicators[playerId].gameObject.SetActive(true);
        BattleInterface.Instance.DamageIndicators[playerId].transform.Find("Damage_Text").GetComponent<Text>().text =
            damage;
        // Set position
        if (Camera.main)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(damageTextAnchor.transform.position);
            BattleInterface.Instance.DamageIndicators[playerId].GetComponent<RectTransform>().position = new Vector3(screenPos.x, screenPos.y + damageCounter * 45, screenPos.z);
            damageCounter++;
        }
        yield return null;
    }

    private void Update()
    {
        try
        {
            spriteRenderer.material.SetColor("_Additive_Color", additiveColor);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}
