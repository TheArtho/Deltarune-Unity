using System;
using System.Collections;
using Core.Combat;
using UnityEngine;

/// <summary>
/// Most of the logic can be overriden in a extending class to match with the behaviour of the new enemy
/// </summary>
public class Enemy
{
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
      
      public bool isSpared = false;
      public bool IsFainted => (Hp <= 0);
      public Status status = Status.None;

      protected int[] attacks;

      public Battle battle { get; private set; }
      public int battlerIndex { get; private set; }
      public int turnCount { get; private set; }
      
      #endregion
      
      #region Hardcoded Variables

      public string[] battleText { get; protected set; }
      
      #endregion

      public Enemy(string name, int hp, int attack, int defense)
      {
            this.name = name;
            maxHP = hp;
            this.Hp = hp;
            // attacks = new EnemyAttack[] {new EnemyAttack()};
            this.attack = attack;
            this.defense = defense;
      }

      public void Initialize(Battle battle, int battlerIndex)
      {
            this.battle = battle;
            this.battlerIndex = battlerIndex;
            InitializeAttacks(battle);
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
      
      /// <summary>
      /// Custom Behavior depending on the situation
      /// </summary>
      /// <returns></returns>
      public virtual IEnumerator StartTurnBehaviour()
      {
            // Custom Behavior depending on the situation
            yield return null;
      }

      /// <summary>
      /// Additional behavior at the end of the turn
      /// </summary>
      /// <returns></returns>
      public virtual IEnumerator EndTurnBehavior()
      {
            // Additional effects for the Enemy at the end of the turn
            yield return null;
      }

      /// <summary>
      /// Faint animation
      /// </summary>
      /// <returns></returns>
      public virtual IEnumerator Faint()
      {
            yield return null;
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