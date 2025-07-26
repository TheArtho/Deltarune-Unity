namespace Core.Combat
{
    public interface IBattleSequence
    {
   
        
    }

    public class BattleSequence : IBattleSequence
    {
        public bool RunInParallel = false;
    }

    public class DialogSequence : BattleSequence
    {
        public int Character;
        public string Text;
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