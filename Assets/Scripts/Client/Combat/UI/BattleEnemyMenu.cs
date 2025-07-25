using System;
using System.Collections;
using System.Collections.Generic;
using Client.Combat;
using Client.Combat.UI;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnemyMenu : MonoBehaviour
{
    [Serializable]
    public struct EnemyData
    {
        public string name;
        public int hp;
        public int mercy;
    }
    
    [SerializeField] private Image soul;
    
    [SerializeField] private GameObject mercyLabel;
    
    [SerializeField] private List<BattleEnemyItem> menuItems;
    public List<EnemyData> options;
    
    private int _index;
    private PlayerInputAction _playerInputAction;
    private bool _moveLockedX, _moveLockedY;

    public Action<int> OnSelect;
    public Action OnCancel;
    public Action<int, int> OnChangeSelection;

    public bool canCancel;

    public int Index => _index;

    private void Awake()
    {
        _playerInputAction = new PlayerInputAction();
        
        _playerInputAction.Battle.Move.performed += context =>
        {
            if (options.Count <= 1) return;

            Vector2 move = context.ReadValue<Vector2>();
            if (move.magnitude < 0.25f) // Deadzone
            {
                _moveLockedX = false;
                _moveLockedY = false;
                return; 
            }

            // Strongest axis priority
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            {
                if (_moveLockedX) return;
                _moveLockedX = true;
                _moveLockedY = false;
            }
            else
            {
                if (_moveLockedY) return;
                if (move.y > 0)
                    MoveUp();
                else
                    MoveDown();
                _moveLockedY = true;
                _moveLockedX = false;
                
                SfxHandler.Play("menu_move");
            }
        };

        _playerInputAction.Battle.Move.canceled += context =>
        {
            _moveLockedX = false;
            _moveLockedY = false;
        };
        
        _playerInputAction.Battle.Select.performed += context =>
        {

            DisableInput();
            SfxHandler.Play("menu_select");
            OnSelect?.Invoke(_index);
        };
        
        _playerInputAction.Battle.Cancel.performed += context =>
        {
            if (!canCancel) return;
            
            DisableInput();
            SfxHandler.Play("menu_select");
            OnCancel?.Invoke();
        };
    }

    public void EnableInput()
    {
        _playerInputAction?.Enable();
    }
    
    public void DisableInput()
    {
        _playerInputAction?.Disable();
    }

    private void MoveUp()
    {
        _index--;

        if (_index < 0)
        {
            _index = Math.Min(menuItems.Count - 1, options.Count - 1);
        }
        else
        {
            OnChangeSelection(_index + 1, _index);
        }
        
        UpdateButtons();
    }

    private void MoveDown()
    {
        _index++;

        if (_index >= menuItems.Count || _index >= options.Count)
        {
            _index = 0;
        }
        else
        {
            OnChangeSelection(_index - 1, _index);
        }
        
        UpdateButtons();
    }


    public void UpdateButtons()
    {
        mercyLabel.gameObject.SetActive(options.Exists(x => x.mercy >= 0));
        
        for (var i = 0; i < menuItems.Count; i++)
        {
            if (i < options.Count)
            {
                menuItems[i].HpBar.enabled = true;
                menuItems[i].MercyBar.enabled = options[i].mercy >= 0;
                menuItems[i].HpBarBg.enabled = true;
                menuItems[i].MercyBarBg.enabled = options[i].mercy >= 0;
                
                menuItems[i].HpBar.fillAmount = (float) options[i].hp / 100;
                menuItems[i].MercyBar.fillAmount = (float) options[i].mercy / 100;
                
                menuItems[i].Name.text = options[i].name;
                menuItems[i].Hp.text = $"{options[i].hp}%";
                menuItems[i].Mercy.text = (options[i].mercy >= 0) ? $"{options[i].mercy}%" : "";
            }
            else
            {
                menuItems[i].HpBar.enabled = false;
                menuItems[i].MercyBar.enabled = false;
                menuItems[i].HpBarBg.enabled = false;
                menuItems[i].MercyBarBg.enabled = false;
                
                menuItems[i].Name.text = "";
                menuItems[i].Hp.text = "";
                menuItems[i].Mercy.text = "";
            }
        }
        
        soul.rectTransform.position = menuItems[_index].SoulPosition.position;
    }

    private void OnEnable()
    {
        soul.gameObject.SetActive(true);
        UpdateButtons();
        _index = 0;
    }
    
    private void OnDisable()
    {
        soul.gameObject.SetActive(false);
    }
}
