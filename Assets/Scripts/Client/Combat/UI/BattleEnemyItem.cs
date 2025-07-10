using UnityEngine;
using UnityEngine.UI;

namespace Client.Combat.UI
{
    public class BattleEnemyItem : MonoBehaviour
    {
        [SerializeField] private RectTransform soulPosition;
        [SerializeField] private Text name;
        [SerializeField] private Text hp;
        [SerializeField] private Text mercy;
        [SerializeField] private Image hpBar;
        [SerializeField] private Image mercyBar;
        [SerializeField] private Image hpBarBg;
        [SerializeField] private Image mercyBarBg;
        
        public RectTransform SoulPosition => soulPosition;

        public Text Name => name;

        public Text Hp => hp;

        public Text Mercy => mercy;

        public Image HpBar => hpBar;

        public Image MercyBar => mercyBar;
        
        public Image HpBarBg => hpBarBg;
        
        public Image MercyBarBg => mercyBarBg;
    }
}
