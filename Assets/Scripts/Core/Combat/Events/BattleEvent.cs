using System;
using System.Collections.Generic;
using Core.Combat.Actions;
using UnityEngine.InputSystem.Utilities;

namespace Core.Combat.Events
{
    public interface IBattleEvent { }

    public class PlayAnimationEvent : IBattleEvent
    {
        public string AnimationName;
        public int TargetId;
    }

    public class ChooseActionEvent : IBattleEvent
    {
        public int Player;
        public ActionType ActionType;
    }
    
    public class CancelActionEvent : IBattleEvent
    {
        public int Player;
    }
    
    public class GlobalStateEvent : IBattleEvent
    {
        public struct EnemyState
        {
            public int id;
            public string Name;
            public int Hp;
            public int MaxHp;
            public int Mercy;
            public int Attack;
            public int Defense;
            public bool Fainted;
            public bool Spared;
            public bool Pacified;
        }

        public EnemyState[] Ennemies;
        public string[] Items;
        public string Text;

        public int[] ActivePlayers;
    }
    
    public class PlayerStateEvent : IBattleEvent
    {
        public struct PlayerState
        {
            public string Name;
            public int Hp;
            public int MaxHp;
            public int Attack;
            public int Defense;
            public int Magic;
            public string[][] Actions;
        }
        
        public int Player;
        public PlayerState State;
    }
    
    public class UpdateInventoryEvent : IBattleEvent
    {
        public string[] items;
        public BattleItem.TargetType[] targetType;
        public bool[] selected;
    }

    public class StartTurnEvent : IBattleEvent {}
     
    public class ReqFightQuickTimeDataEvent : IBattleEvent
    {
        public int Player;
        public int Delay;
    }
    
    public class FightQteStartEvent : IBattleEvent {}

    public class PlayerAttackEvent : IBattleEvent
    {
        public int Player;
        public int Damage;
        public int Target;
        public bool Fainted;
    }
    
    public class PlayerMissedEvent : IBattleEvent
    {
        public int Player;
        public int Target;
    }

    public class PlayBattleSequenceEvent : IBattleEvent
    {
        public List<BattleSequence> BattleSequence;
    }
    
    public class PlayBattleSequenceOneShotEvent : IBattleEvent
    {
        public List<BattleSequence> BattleSequence;
    }

    public class BulletHellWaitReady : IBattleEvent
    {
        public List<int> Targets;
        public List<BattleSequence> BattleSequence;
        public string BattleMode;
        public string[] Attacks;
    }
    
    public class BulletHellStartEvent : IBattleEvent {}

    public class AddTpEvent : IBattleEvent
    {
        public int Player;
        public int Amount;
        public int PreviousValue;
    }
    
    public class RemoveTpEvent : IBattleEvent
    {
        public int Player;
        public int Amount;
        public int PreviousValue;
    }

    public class DamagePlayerEvent : IBattleEvent
    {
        public int Player;
        public int CurrentHp;
        public int MaxHp;
        public string Damage;
    }
    
    public class HealPlayerEvent : IBattleEvent
    {
        public int Player;
        public int CurrentHp;
        public int HealAmount;
    }

    public class KnockOutEvent : IBattleEvent
    {
        public int Player;
        public List<int> NewTargets;
    }
    
    public class GameOverEvent : IBattleEvent
    {
        public string Message = "Stay determined.";
        public string SoundText = "text";
        public string Music;
    }

    public class PlayEndBattleSequenceEvent : IBattleEvent
    {
        public List<BattleSequence> sequence;
    }
    
    public class EndBattleEvent : IBattleEvent
    {
        public List<BattleSequence> sequence;
    }
}