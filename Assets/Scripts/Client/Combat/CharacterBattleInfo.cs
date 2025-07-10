using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBattleInfo : MonoBehaviour
{
    [SerializeField]
    private bool selected;
    [SerializeField]
    private Image characterLogo;
    [SerializeField]
    private Text name;
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private Text hpText;
    [SerializeField]
    private Text hpMaxText;

    [Space] 
    
    [SerializeField] 
    private Image infoBackground;
    [SerializeField] 
    private GameObject choiceButtons;

    public bool Selected => selected;

    public void Select()
    {
        selected = true;
        UpdateSelected();
    }

    public void Unselect()
    {
        selected = false;
        UpdateSelected();
    }

    private void Update()
    {
        UpdateSelected();
    }

    private void UpdateSelected()
    {
        if (selected)
        {
            infoBackground.enabled = true;
            choiceButtons.SetActive(true);
        }
        else
        {
            infoBackground.enabled = false;
            choiceButtons.SetActive(false);
        }
    }

    public void SetLogo(Sprite logo)
    {
        this.characterLogo.sprite = logo;
    }

    public void SetName(string name)
    {
        this.name.text = name.ToUpper();
    }

    public void UpdateHp(int currentHp, int maxHp)
    {
        hpText.text = currentHp.ToString();
        hpMaxText.text = maxHp.ToString();
        
        hpBar.fillAmount = Mathf.Clamp01(currentHp / Mathf.Max(maxHp, 1f));
    }
}