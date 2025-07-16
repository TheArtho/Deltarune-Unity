using System;
using System.Collections.Generic;
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
        }

        public EnemyState[] Ennemies;
        public string[] Items;
        public string Text;
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
            public string[] Actions;
        }
        
        public int Player;
        public PlayerState State;
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
    }
    
    public class PlayerMissedEvent : IBattleEvent
    {
        public int Player;
        public int Target;
    }

    public class PlayBattleSequenceEvent : IBattleEvent
    {
        public List<IBattleSequence> battleSequence;
    }

    public class BulletHellWaitReady : IBattleEvent
    {
        public List<IBattleSequence> battleSequence;
        public string battleMode;
        public string[] attacks;
    }
    
    public class BulletHellStartEvent : IBattleEvent {}
}