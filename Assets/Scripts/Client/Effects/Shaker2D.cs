using UnityEngine;

namespace Client.Effects
{
    public class Shaker2D : MonoBehaviour
    {
        [Range(0,1)] public float verticalStrength = 0;
        [Range(0,1)] public float horizontalStrength = 0;
    
        [Range(0.1f,10)] public float speed = 1;

        [SerializeField] private float maxStrength = 1;
    
        private Transform objectTransform;
        private Vector3 startPosition;
        private float startNoiseX, startNoiseY;
        private float time;

        private const float NoiseBuffer = 100000;

        // Start is called before the first frame update
        void Start()
        {
            objectTransform = this.transform;
            startPosition = objectTransform.localPosition;
            time = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (maxStrength > 10) maxStrength = 10;
            if (maxStrength < 0) maxStrength = 0;
        
            time += Time.deltaTime*speed;

            var currentHorizontalStrength = maxStrength * horizontalStrength;
            var currentVerticalStrength = maxStrength * verticalStrength;

            var noiseX = Mathf.PerlinNoise((time % NoiseBuffer), 0);
            var noiseY = Mathf.PerlinNoise(0, (time % NoiseBuffer));

            if ((horizontalStrength > 0 || verticalStrength > 0) && (maxStrength > 0)) // Shakes the object position depending on the noise
            {
                objectTransform.localPosition = startPosition + new Vector3(noiseX*currentHorizontalStrength - (currentHorizontalStrength/2), noiseY*currentVerticalStrength - (currentVerticalStrength/2), startPosition.z);
            }
            else // Recenter the object position
            {
                time = 0;
                objectTransform.localPosition = new Vector3(Mathf.MoveTowards(startPosition.x, 0, Time.deltaTime), Mathf.MoveTowards(startPosition.y, 0, Time.deltaTime), startPosition.z);
            }
        }
    }
}
