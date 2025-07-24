using System;
using System.Collections;
using System.Collections.Generic;
using Client.Combat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSubMenu : MonoBehaviour
{
    [SerializeField] private Image soul;
    
    [SerializeField] private List<BattleMenuItem> menuItems;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private int columns = 2; // 2x2 grid
    public List<string> options;
    
    private int _index;
    private PlayerInputAction _playerInputAction;
    private bool _moveLockedX, _moveLockedY;

    public Action<int> OnSelect;
    public Action OnCancel;

    public bool canCancel;

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
                if (move.x > 0)
                    MoveRight();
                else
                    MoveLeft();
                _moveLockedX = true;
                _moveLockedY = false;
                
                SfxHandler.Play("menu_move");
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
    
    private Vector2Int IndexToGrid(int index)
    {
        return new Vector2Int(index % columns, index / columns);
    }

    private int GridToIndex(int x, int y)
    {
        return y * columns + x;
    }

    private void MoveRight()
    {
        var coord = IndexToGrid(_index);
        coord.x++;

        if (GridToIndex(coord.x, coord.y) >= options.Count || coord.x >= columns)
            coord.x = 0;

        _index = GridToIndex(coord.x, coord.y);
        UpdateButtons();
    }

    private void MoveLeft()
    {
        var coord = IndexToGrid(_index);
        coord.x--;

        if (coord.x < 0 || GridToIndex(coord.x, coord.y) >= options.Count)
            coord.x = columns - 1;

        _index = GridToIndex(coord.x, coord.y);
        UpdateButtons();
    }

    private void MoveUp()
    {
        var coord = IndexToGrid(_index);
        coord.y--;

        if (coord.y < 0 || GridToIndex(coord.x, coord.y) >= options.Count)
            coord.y = (options.Count - 1) / columns;

        _index = GridToIndex(coord.x, coord.y);
        UpdateButtons();
    }

    private void MoveDown()
    {
        var coord = IndexToGrid(_index);
        coord.y++;

        if (GridToIndex(coord.x, coord.y) >= options.Count)
            coord.y = 0;

        _index = GridToIndex(coord.x, coord.y);
        UpdateButtons();
    }


    public void UpdateButtons()
    {
        for (var i = 0; i < menuItems.Count; i++)
        {
            if (i < options.Count)
            {
                menuItems[i].Text.text = options[i];
            }
            else
            {
                menuItems[i].Text.text = "";
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
