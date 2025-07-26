namespace Core.Combat
{
    public interface IBattleSequence
    {
   
        
    }

    public class BattleSequence : IBattleSequence
    {
        public bool RunInParallel = false;
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
    }

    public class PlayerAnimationSequence : BattleSequence
    {
        public int Character;
        public string Animation;
        public float Time;
    }
}