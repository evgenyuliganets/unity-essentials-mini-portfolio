using UnityEngine;

namespace _Unity_Essentials.Scripts.Provided_Scripts
{
    [RequireComponent(typeof(Light))]
    public class DayNightCycle : MonoBehaviour
    {
        [Tooltip("Real seconds required for one complete day.")] [SerializeField, Min(0.1f)]
        private float secondsPerDay = 120f;

        [Tooltip("World axis around which the sun rotates.")] [SerializeField]
        private Vector3 rotationAxis = Vector3.right;

        private void Update()
        {
            if (rotationAxis == Vector3.zero)
                return;

            float degreesPerSecond = 360f / secondsPerDay;
            float rotationThisFrame =
                degreesPerSecond * Time.deltaTime;

            transform.Rotate(
                rotationAxis.normalized,
                rotationThisFrame,
                Space.World
            );
        }
    }
}