using UnityEngine;

namespace _Unity_Essentials.Scripts.Provided_Scripts
{
    public class SpinningPlayer : MonoBehaviour
    {
        [SerializeField, Min(0.01f)]
        private float circleRadius = 0.5f;

        [SerializeField]
        private float rotationSpeed = 100f;

        [SerializeField]
        private bool turnRight = true;

        private Vector3 circleCenter;
        private float direction;

        private void Start()
        {
            direction = turnRight ? 1f : -1f;

            // Фіксована точка біля правого або лівого колеса
            circleCenter =
                transform.position +
                transform.right * circleRadius * direction;

            circleCenter.y = transform.position.y;
        }

        private void Update()
        {
            transform.RotateAround(
                circleCenter,
                Vector3.up,
                rotationSpeed * direction * Time.deltaTime
            );
        }

        private void OnDrawGizmosSelected()
        {
            float side = turnRight ? 1f : -1f;

            Vector3 previewCenter =
                transform.position +
                transform.right * circleRadius * side;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(previewCenter, 0.08f);
        }
    }
}