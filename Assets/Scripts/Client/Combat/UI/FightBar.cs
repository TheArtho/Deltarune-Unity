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

        private PlayerInputAction playerInputAction;
        private bool canPress;
        public bool done;

        [SerializeField] private float speed = 200;

        public Action<int> OnPress;

        private float perfectTime;

        private void Awake()
        {
            playerInputAction = new PlayerInputAction();

            playerInputAction.Battle.Select.performed += context =>
            {
                if (!canPress) return;
                Press();
            };
        }

        public void EnableInput()
        {
            playerInputAction.Enable();
        }
        
        public void DisableInput()
        {
            playerInputAction.Disable();
        }

        public void CanPress()
        {
            StartCoroutine(CanPressIE());
        }

        private IEnumerator CanPressIE()
        {
            yield return null;
            canPress = true;
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
            
            // Calculate how many seconds needed to reach the target
            float distanceToTarget = whiteBar.anchoredPosition.x - bar.anchoredPosition.x;
            float timeToTarget = distanceToTarget / speed; // speed must be in unit/sec

            perfectTime = Time.time + timeToTarget;
            
            while (true)
            {
                whiteBar.anchoredPosition += Vector2.left * (Time.deltaTime * speed);

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
            // Positionne les particules au bon endroit
            Vector3 position = whiteBarRect.rectTransform.position;
            perfectPressParticle.GetComponent<RectTransform>().position = position;
            pressParticle.GetComponent<RectTransform>().position = position;

            // Calcule l'écart de temps entre le moment d'appui et le moment "parfait"
            float timeError = Mathf.Abs(Time.time - perfectTime);
            
            const float LOGIC_FRAME_RATE = 30f;
            int framesOff = Mathf.RoundToInt(timeError * LOGIC_FRAME_RATE);

            // Calculating accuracy following Deltarune rules
            int accuracy;
            if (framesOff == 0)
            {
                accuracy = 150;
                perfectPressParticle.Emit(1);
            }
            else if (framesOff == 1)
            {
                accuracy = 120;
                pressParticle.Emit(1);
            }
            else if (framesOff == 2)
            {
                accuracy = 110;
                pressParticle.Emit(1);
            }
            else
            {
                accuracy = Mathf.Max(1, 100 - (framesOff * 2));
                pressParticle.Emit(1);
            }

            // Nettoyage
            canPress = false;
            whiteBarRect.gameObject.SetActive(false);
            StopCoroutine(nameof(MoveWhiteBar));

            // Envoie le résultat
            Debug.Log("Accuracy is " + accuracy);
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
