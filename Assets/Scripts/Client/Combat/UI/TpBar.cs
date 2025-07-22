using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Client.Combat.UI
{
    public class TpBar : MonoBehaviour
    {
        [SerializeField] private Image barWhite;
        [SerializeField] private Image barOrange;
        [FormerlySerializedAs("TpText")] [SerializeField] private Text tpText;
        [FormerlySerializedAs("TpTextObject")] [SerializeField] private GameObject tpTextObject;
        [FormerlySerializedAs("TpTextMaxObject")] [SerializeField] private GameObject tpTextMaxObject;
        
        [SerializeField] Color maxColor = Color.yellow;
        
        [SerializeField] private int value;
        private float currentFill;
        private const float fillLerpSpeed = 5f;

        public void AddTp(int amount)
        {
            SetTp(value + amount);
        }

        public void RemoveTp(int amount)
        {
            SetTp(value - amount);
        }

        public void SetTp(int newValue)
        {
            int clampedValue = Mathf.Clamp(newValue, 0, 100);
            value = newValue;
            barWhite.fillAmount = clampedValue / 100f;
            
            tpText.text = clampedValue.ToString();
            
            if (clampedValue < 100)
            {
                tpTextObject.SetActive(true);
                tpTextMaxObject.SetActive(false);
                barWhite.color = Color.white;
                barOrange.color = new Color(barOrange.color.r, barOrange.color.g, barOrange.color.b, 1);
            }
            else
            {
                tpTextMaxObject.SetActive(true);
                tpTextObject.SetActive(false);
                barWhite.color = maxColor;
                barOrange.color = new Color(barOrange.color.r, barOrange.color.g, barOrange.color.b, 0);
            }
        }

        private void Update()
        {
            SetTp(value);
            float targetFill = barWhite.fillAmount;
            float current = barOrange.fillAmount;
            float lerped = Mathf.Lerp(current, targetFill - 0.01f, Time.deltaTime * fillLerpSpeed);
            barOrange.fillAmount = lerped;
        }
    }
}