using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Client.Combat.Events;
using Core.Combat.Events;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Combat.UI
{
    public class BattleChoice : MonoBehaviour
    {
        public int playerId;
        
        private int _index;
        private PlayerInputAction _playerInputAction;
        private bool _moveLockedX, _moveLockedY;
        
        [SerializeField] private List<Button> buttons;

        public Action<PlayerCommandEvent> OnSelect;
        public Action OnCancel;

        public bool canCancel;

        private void Awake()
        {
            _playerInputAction = new PlayerInputAction();
            
            _playerInputAction.Battle.Move.performed += context =>
            {
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
                    _moveLockedY = true;
                    _moveLockedX = false;
                }
            };
            
            _playerInputAction.Battle.Move.canceled += context =>
            {
                _moveLockedX = false;
                _moveLockedY = false;
            };
            
            _playerInputAction.Battle.Select.performed += context =>
            {
                switch (_index)
                {
                    case 0: // Fight
                        DisableInput();
                        SfxHandler.Play("menu_select");
                        StartCoroutine(Fight());
                        break;
                    case 1:  // Act / Magic
                        DisableInput();
                        SfxHandler.Play("menu_select");
                        StartCoroutine(ActMagic());
                        break;
                    case 2: // Item
                        if (BattleInterface.Instance.InventoryEvent.items.Length > 0)
                        {
                            DisableInput();
                            SfxHandler.Play("menu_select");
                            StartCoroutine(Items());
                        }
                        break;
                    case 3: // Spare
                        DisableInput();
                        SfxHandler.Play("menu_select");
                        StartCoroutine(Spare());
                        break;
                    case 4: // Defend
                        DisableInput();
                        SfxHandler.Play("menu_select");
                        StartCoroutine(Defend());
                        break;
                }
            };
            
            _playerInputAction.Battle.Cancel.performed += context =>
            {
                if (!canCancel) return;
                
                DisableInput();
                SfxHandler.Play("menu_select");
                OnCancel?.Invoke();
            };
        }
        
        private IEnumerator Fight()
        {
            int target = 0;
            int index = -1;
            
            var enemies = new List<BattleEnemyMenu.EnemyData>();
            
            foreach (var e in BattleInterface.Instance.GlobalStateEvent.Ennemies)
            {
                enemies.Add(new BattleEnemyMenu.EnemyData
                {
                    name = e.Name,
                    hp = Mathf.CeilToInt((float) e.Hp / e.MaxHp * 100),
                    mercy = e.Mercy
                });
            }
            
            yield return StartCoroutine(BattleInterface.Instance.EnemySelect(enemies.ToArray(),
                value =>
                {
                    target = value;
                })
            );

            // Cancel
            if (target == -1)
            {
                EnableInput();
                BattleScene.Instance.PlayerBattleSprites[playerId].Play("Idle");
            }
            else // Index chosen
            {
                OnSelect?.Invoke(new  PlayerCommandEvent
                {
                    Player = -1,
                    ActionType = ActionType.Fight,
                    TargetId = target,
                    Index = index
                });
            }
        }

        private IEnumerator ActMagic()
        {
            int target = 0;
            int index = -1;
            
            while (true)
            {
                var enemies = new List<BattleEnemyMenu.EnemyData>();
            
                foreach (var e in BattleInterface.Instance.GlobalStateEvent.Ennemies)
                {
                    enemies.Add(new BattleEnemyMenu.EnemyData
                    {
                        name = e.Name,
                        hp = Mathf.CeilToInt((float) e.Hp / e.MaxHp * 100),
                        mercy = e.Mercy
                    });
                }
            
                yield return StartCoroutine(BattleInterface.Instance.EnemySelect(enemies.ToArray(),
                    value =>
                    {
                        target = value;
                    })
                );
                
                // Cancel
                if (target == -1)
                {
                    EnableInput();
                    break;
                }
                
                yield return StartCoroutine(BattleInterface.Instance.SubMenuSelect(
                    BattleInterface.Instance.PlayerStateEvents[playerId].State.Actions[target],
                    value =>
                    {
                        index = value;
                    }));

                // Cancel
                if (index == -1)
                {
                    // Loop
                }
                else // Index chosen
                {
                    OnSelect?.Invoke(new  PlayerCommandEvent
                    {
                        Player = -1,
                        ActionType = ActionType.ActMagic,
                        TargetId = target,
                        Index = index
                    });
                    break;
                }
            }
        }
        
        private IEnumerator Items()
        {
            int target = 0;
            int index = -1;

            while (true)
            {
                Action<UpdateInventoryEvent> onUpdateInventory = context =>
                {
                    BattleInterface.Instance.SubMenu.options = context.items.ToList();
                    BattleInterface.Instance.SubMenu.availableOptions = context.selected.Select(b => !b).ToList();
                };

                BattleInterface.Instance.OnUpdateInventory += onUpdateInventory;
                
                yield return StartCoroutine(BattleInterface.Instance.SubMenuSelect(
                    BattleInterface.Instance.InventoryEvent.items,
                    BattleInterface.Instance.InventoryEvent.selected.Select(b => !b).ToArray(),
                    value =>
                    {
                        index = value;
                    }));
                
                // Cancel
                if (index == -1)
                {
                    EnableInput();
                    break;
                }
                
                BattleInterface.Instance.OnUpdateInventory -= onUpdateInventory;
                
                var enemies = new List<BattleEnemyMenu.EnemyData>();
            
                foreach (var e in BattleInterface.Instance.PlayerStateEvents)
                {
                    enemies.Add(new BattleEnemyMenu.EnemyData
                    {
                        name = e.State.Name,
                        hp = Mathf.CeilToInt((float) e.State.Hp / e.State.MaxHp * 100),
                        mercy = -1
                    });
                }
                
                yield return StartCoroutine(BattleInterface.Instance.PlayerSelect(enemies.ToArray(),
                    value =>
                    {
                        target = value;
                    })
                );
                
                // Cancel
                if (target == -1)
                {
                    // Loop
                }
                else // Index chosen
                {
                    OnSelect?.Invoke(new  PlayerCommandEvent
                    {
                        Player = -1,
                        ActionType = ActionType.Item,
                        TargetId = target,
                        Index = index
                    });
                    break;
                }
            }
        }
        
        private IEnumerator Spare()
        {
            int target = 0;
            int index = -1;

            var enemies = new List<BattleEnemyMenu.EnemyData>();
            
            foreach (var e in BattleInterface.Instance.GlobalStateEvent.Ennemies)
            {
                enemies.Add(new BattleEnemyMenu.EnemyData
                {
                    name = e.Name,
                    hp = Mathf.CeilToInt((float) e.Hp / e.MaxHp * 100),
                    mercy = e.Mercy
                });
            }
            
            yield return StartCoroutine(BattleInterface.Instance.EnemySelect(enemies.ToArray(),
                value =>
                {
                    target = value;
                })
            );

            // Cancel
            if (target == -1)
            {
                EnableInput();
            }
            else // Index chosen
            {
                OnSelect?.Invoke(new  PlayerCommandEvent
                {
                    Player = -1,
                    ActionType = ActionType.Spare,
                    TargetId = target,
                    Index = index
                });
            }
        }
        
        private IEnumerator Defend()
        {
            OnSelect?.Invoke(new  PlayerCommandEvent
            {
                Player = -1,
                ActionType = ActionType.Defend,
                TargetId = _index,
                Index = -1
            });
            yield return null;
        }
        
        public void EnableInput()
        {
            _playerInputAction?.Enable();
        }
    
        public void DisableInput()
        {
            _playerInputAction?.Disable();
        }

        private void MoveRight()
        {
            _index++;

            if (_index >= buttons.Count)
            {
                _index = 0;
            }

            UpdateButtons();
        }

        private void MoveLeft()
        {
            _index--;

            if (_index < 0)
            {
                _index = buttons.Count - 1;
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].selected = (i == _index);
            }
        }
    }
}
