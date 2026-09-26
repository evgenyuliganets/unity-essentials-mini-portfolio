using UnityEngine;

[DisallowMultipleComponent]
public class AutomaticDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField, Range(-180f, 180f)]
    private float openAngle = 90f;

    [SerializeField, Min(1f)]
    private float rotationSpeed = 120f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool shouldOpen;

    private void Awake()
    {
        closedRotation = transform.localRotation;

        openRotation =
            closedRotation *
            Quaternion.Euler(0f, openAngle, 0f);
    }
    
    public void OpenAwayFrom(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = playerPosition - transform.position;

        float playerSide = Vector3.Dot(transform.forward, directionToPlayer);
        float angleDirection = playerSide >= 0f ? 1f : -1f;

        openRotation =
            closedRotation *
            Quaternion.Euler(0f, openAngle * angleDirection, 0f);

        shouldOpen = true;
    }

    private void Update()
    {
        Quaternion targetRotation =
            shouldOpen ? openRotation : closedRotation;

        transform.localRotation =
            Quaternion.RotateTowards(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    public void SetOpen(bool open)
    {
        shouldOpen = open;
    }
}