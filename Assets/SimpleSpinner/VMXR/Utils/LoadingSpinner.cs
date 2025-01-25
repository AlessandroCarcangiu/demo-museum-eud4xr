using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleSpinner
{
    [RequireComponent(typeof(Image))]
    public class LoadingSpinner : MonoBehaviour
    {
        [Header("Rotation")]
        public bool Rotation = true;
        [Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
        public float RotationSpeed = 1;
        public AnimationCurve RotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Rainbow")]
        public bool Rainbow = true;
        [Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
        public float RainbowSpeed = 0.5f;
        [Range(0, 1)]
        public float RainbowSaturation = 1f;
        public AnimationCurve RainbowAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Options")]
        public bool RandomPeriod = true;
        
        private Image _image;
        private float _period;
        
        private Color[] _avvaleColors =
        {
            new Color(0.34510f,  0.68627f, 0.23922f), // hsv(105, 65%, 67%) rgb(86, 170, 59)
            new Color(0.29412f,0.69412f,0.64314f), // hsv(172, 58%, 69%) rgb(75, 177, 164)
            new Color(0.15294f,0.54510f,0.49804f), // hsv(173, 72%, 55%) rgb(39, 139, 127)
            new Color(0.49804f,  0.72941f,  0.21961f) // hsv(87, 70%, 73%) rgb(127, 186, 56)
        };

        // private int _currentColorIndex = 0;

        // private bool colorChanged = false;


        public void Start()
        {
            _image = GetComponent<Image>();
            _period = RandomPeriod ? Random.Range(0f, 1f) : 0;
        }

        public void Update()
        {
            if (Rotation)
            {
                transform.localEulerAngles = new Vector3(0, 0, -360 * RotationAnimationCurve.Evaluate((RotationSpeed * Time.time + _period) % 1));
            }

            if (Rainbow)
            {
                int colorIndex = Mathf.FloorToInt((RainbowSpeed * Time.time + _period) % _avvaleColors.Length);
                _image.color = _avvaleColors[colorIndex];
                // colorChanged = true;
            }
        }
    }
}