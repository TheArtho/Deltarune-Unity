using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Combat.UI
{
    public class BattleMenuItem : MonoBehaviour
    {
        [SerializeField] private RectTransform soulPosition;
        [SerializeField] private TextMeshProUGUI text;

        public RectTransform SoulPosition => soulPosition;
        public TextMeshProUGUI Text => text;
    }
}
