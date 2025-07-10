using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Client.Combat.UI
{
    public class FightBar : MonoBehaviour
    {
        [SerializeField] private Image logo;
        [SerializeField] private Color barColor1;
        [SerializeField] private Color barColor2;
        
        [SerializeField] private Image barRectBg;
        [FormerlySerializedAs("barRect")] [SerializeField] private Image whiteBarRect;
        [SerializeField] private Image barRectGoal;
        [SerializeField] private ParticleSystem pressParticle;
        [SerializeField] private ParticleSystem perfectPressParticle;
        
        [Space]
        
        private int damageMin = 1;
        private int damageMax = 150;
        [SerializeField] private float minDistance = 5f;   // "Perfect zone" distance
        [SerializeField] private float maxDistance = 200f; // Distance to clamp to damageMin

        private PlayerInputAction _playerInputAction;
        public bool canPress;
        public bool done;

        [SerializeField] private float speed = 1;

        public Action<int> OnPress;

        private void Awake()
        {
            _playerInputAction = new PlayerInputAction();

            _playerInputAction.Battle.Select.performed += context =>
            {
                if (!canPress) return;
                Press();
            };
        }

        public void EnableInput()
        {
            _playerInputAction.Enable();
        }
        
        public void DisableInput()
        {
            _playerInputAction.Disable();
        }

        public void StartQte(int delay, bool canPress = true)
        {
            // Place white bar at the start position
            // Make it move to the left
            this.canPress = canPress;
            this.done = false;
            StartCoroutine(nameof(MoveWhiteBar), delay);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay">Delay in ms</param>
        /// <returns></returns>
        private IEnumerator MoveWhiteBar(int delay)
        {
            yield return new WaitForSeconds((float) delay / 1000);
            
            whiteBarRect.gameObject.SetActive(true);
            
            RectTransform whiteBar = whiteBarRect.rectTransform;
            RectTransform bar = barRectGoal.rectTransform;
            
            whiteBar.anchoredPosition = new Vector2(180, whiteBar.anchoredPosition.y);
            
            while (true)
            {
                whiteBar.position += Vector3.left * (Time.deltaTime * speed);

                if (whiteBar.position.x <= bar.position.x - 0.2f)
                {
                    Miss();
                    break;
                }
                yield return null;
            }
            
            whiteBarRect.gameObject.SetActive(false);
        }

        private void Press()
        {
            perfectPressParticle.GetComponent<RectTransform>().position = whiteBarRect.GetComponent<RectTransform>().position;
            pressParticle.GetComponent<RectTransform>().position = whiteBarRect.GetComponent<RectTransform>().position;
                
            float distance = Vector3.Distance(
                whiteBarRect.rectTransform.anchoredPosition,
                barRectGoal.rectTransform.anchoredPosition
            );

            int accuracy;

            if (distance <= minDistance)
            {
                accuracy = damageMax;
                perfectPressParticle.Emit(1);
            }
            else if (distance >= maxDistance)
            {
                accuracy = damageMin;
                pressParticle.Emit(1);
            }
            else
            {
                // Inverse linear interpolation
                float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
                accuracy = Mathf.RoundToInt(Mathf.Lerp(damageMin, damageMax, t));
                pressParticle.Emit(1);
            }
            
            canPress = false;
            whiteBarRect.gameObject.SetActive(false);
            // Particles
            StopCoroutine(nameof(MoveWhiteBar));
            OnPress?.Invoke(accuracy);
            Done();
        }

        private void Miss()
        {
            canPress = false;
            whiteBarRect.gameObject.SetActive(false);
            // Particles
            OnPress?.Invoke(-1);
            Done();
        }

        private void Done()
        {
            done = true;
        }
    }
}
