using UnityEngine;
using UnityEngine.UI;

namespace Client.Combat.UI
{
    public class BattleMenuItem : MonoBehaviour
    {
        [SerializeField] private RectTransform soulPosition;
        [SerializeField] private Text text;

        public RectTransform SoulPosition => soulPosition;
        public Text Text => text;
    }
}
