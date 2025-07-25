namespace Core.Combat
{
    public interface IBattleSequence
    {
   
        
    }

    public class BattleSequence : IBattleSequence
    {
        public bool RunInParallel = false;
    }
    
    public class WaitSequence : BattleSequence
    {
        public float Time = 0.3f;
    }
    
    public class PlayerDialogSequence : BattleSequence
    {
        public int PlayerId = 0;
        public string Text = "Test.";
        public float Time = 1f;
    }

    public class EnemyDialogSequence : BattleSequence
    {
        public int EnemyId = 0;
        public string Text = "Test.";
        public bool ClearText = true;
        public float TimePerChar = 0.05f;
        public string Sound = "text";
        public bool CanSkip = false;
        public float Time = 1f;
    }

    public class TextSequence : BattleSequence
    {
        public string Text;
        public string Sound = "text";
        public bool ClearText = true;
        public bool CanSkip = false;
        public float Delay = 1f;
        public float timePerChar = 0.04f;
    }
    
    public class ClearTextSequence : BattleSequence
    { }

    public class PlayerAnimationSequence : BattleSequence
    {
        public int Player;
        public string Animation;
        public float Time;
        public bool LockAnimation;
    }

    public class HealPlayerSequence : BattleSequence
    {
        public int Player;
        public int CurrentHp;
        public int HealAmount;
    }

    public class HealEnemySequence : BattleSequence
    {
        public int Enemy;
        public int CurrentHp;
        public int HealAmount;
    }

    public class SpareEnemySequence : BattleSequence
    {
        public int Enemy;
    }
    
    public class PacifyEnemySequence : BattleSequence
    {
        public int Enemy;
    }
    
    public class ScareEnemySequence : BattleSequence
    {
        public int Enemy;
    }
    
    public class KillEnemySequence : BattleSequence
    {
        public int Enemy;
    }
}