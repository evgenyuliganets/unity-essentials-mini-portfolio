using UnityEngine;

namespace _Unity_Essentials.Scripts.Provided_Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class AnswerZone : MonoBehaviour
    {
        [SerializeField] private AnomalyGameManager gameManager;

        [Tooltip("Enabled: anomaly detected. Disabled: everything is normal.")] [SerializeField]
        private bool hasAnomaly;

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;

            if (gameManager == null)
                Debug.LogError("Game Manager is not assigned.", this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (gameManager == null || other.attachedRigidbody == null)
                return;

            gameManager.SubmitAnswer(
                hasAnomaly,
                other.attachedRigidbody
            );
        }
    }
}