using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Combat.Actions;
using Core.Combat.Events;
using Scriptables;
using UnityEngine;

namespace Core.Combat
{
      /// <summary>
      /// Most of the logic can be overriden in a extending class to match with the behaviour of the new enemy
      /// </summary>
      ///
      [EnemyClass(nameof(Enemy))]
      public class Enemy
      {
            public class ActionList
            {
                  public string characterId;
                  public List<ActionDefinition> actions;
            }
            
            #region Parameters
      
            public string name { get; private set; }

            public int maxHP { get; protected set; }
            public int attack { get; private set; }
            public int defense { get; private set; }
      
            /// <summary>
            /// Value between 0 and 100
            /// </summary>
            public int mercy { get; protected set; }

            private int _hp;
            public int Hp
            {
                  get => _hp;
                  set
                  {
                        if (value >= 0 && value <= maxHP)
                        {
                              _hp = value;
                        }
                        else
                        {
                              _hp = value < 0 ? 0 : maxHP;
                        }
                  }
            }
      
            /// <summary>
            /// Determines if the enemy is already spared
            /// </summary>
            public bool IsSpared = false;
            /// <summary>
            /// Determines if the enemy is already pacified
            /// </summary>
            public bool IsPacified = false;
            /// <summary>
            /// Determines if you can have mercy for the enemy (example: asgore)
            /// </summary>
            private bool haveMercy = true;
            public bool IsFainted => (Hp <= 0);
            public Status status = Status.None;

            protected int[] attacks;

            public Battle battle { get; private set; }
            public int battlerIndex { get; private set; }
            public int turnCount { get; private set; }
            public bool IsInBattle => !IsFainted && !IsSpared;
      
            #endregion
      
            #region Hardcoded Variables

            public string description = "This is a test description.";
            public string[] battleText { get; protected set; }

            private List<ActionList> actionDefinitions = new List<ActionList>();
            private List<List<BattleAction>> actions;
      
            #endregion

            private Enemy(string name, int hp, int attack, int defense, bool haveMercy = true)
            {
                  this.battle = battle;
                  this.name = name;
                  this.maxHP = hp;
                  this.Hp = hp;
                  this.attack = attack;
                  this.defense = defense;
                  this.haveMercy = haveMercy;
            }

            public Enemy(EnemyDefinition def)
                  : this(
                        def.name,
                        def.hp,
                        def.attack,
                        def.defense,
                        def.haveMercy
                  )
            {
                  // Store action definitions for later
                  foreach (var a in def.playerActions)
                  {
                        actionDefinitions.Add(new ActionList()
                        {
                              characterId = a.characterId,
                              actions = a.actions
                        });
                  }
            }

            public void Initialize(Battle battle)
            {
                  this.actions = new List<List<BattleAction>>();
                  this.battle = battle;
                  
                  foreach (var p in battle.Players)
                  {
                        var defList = actionDefinitions.Find(a => a.characterId == p.characterId);
                        List<BattleAction> instanceList = defList.actions.Select(definition => definition.CreateInstance(battle)).ToList();
                        this.actions.Add(instanceList);
                  }
            }

            public void DealDamage(int damage)
            {
                  Hp -=  damage;
                  Debug.Log($"{name} took {damage} damage. HP: {Hp}/{maxHP}.");
            }
 
            private void InitializeAttacks(Battle battle)
            {
                  for (int i = 0; i < attacks.Length; ++i)
                  {
                        // attacks[i].Initialize(battle, this);
                  }
            }

            /// <summary>
            /// Returns the list of action names for a specific player
            /// </summary>
            /// <param name="playerId"></param>
            public string[] GetActionChoice(int playerId)
            {
                  return actions[playerId].Select(a => a.GetName()).ToArray();
            }

            public BattleAction GetAction(int playerId, int index)
            {
                  return actions[playerId][index];
            }

            /// <summary>
            /// Chooses an attack depending on the situation
            /// </summary>
            /// <returns></returns>
            public virtual int AttackChoice()
            {
                  // Default attack choice is randomly picking from the attacks array
                  if (attacks.Length == 0) return -1;
                  return UnityEngine.Random.Range(0, attacks.Length);
            }

            /// <summary>
            /// Checks if the conditions to be spared are complete
            /// </summary>
            /// <returns></returns>
            public virtual bool CanSpare()
            {
                  // Checks if the conditions to be spared are complete
                  return false;
            }

            public virtual string GetStartText()
            {
                  // Custom text showing in the dialog box at the start of each turn
                  return battleText[0] ?? "Please insert text";
            }

            public void UpdateTurn()
            {
                  turnCount = battle.TurnCount;
            }
      }
}