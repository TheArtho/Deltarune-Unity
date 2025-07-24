using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterBattleInfo : MonoBehaviour
{
    [SerializeField]
    private bool selected;
    [SerializeField]
    private Image logo;
    [SerializeField] 
    private Color logoColor = Color.white;
    [SerializeField] 
    private Sprite charaLogo;
    [SerializeField] 
    private Sprite[] logoSprites;
    [Space]
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

    public void SetLogo(int logoIndex)
    {
        if (logoIndex < 0)
        {
            this.logo.sprite = charaLogo;
            this.logo.color = Color.white;
        }
        else
        {
            this.logo.sprite = logoSprites[logoIndex];
            this.logo.color = logoColor;
        }
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