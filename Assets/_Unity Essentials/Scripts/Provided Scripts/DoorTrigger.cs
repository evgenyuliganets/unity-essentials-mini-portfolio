using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    private AutomaticDoor door;

    private readonly HashSet<Collider> playerColliders = new();

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null || door == null)
            return;

        // Напрямок вибираємо лише для першого колайдера гравця
        if (playerColliders.Count == 0)
        {
            door.OpenAwayFrom(player.transform.position);
        }

        playerColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null || door == null)
            return;

        playerColliders.Remove(other);

        if (playerColliders.Count == 0)
        {
            door.SetOpen(false);
        }
    }
}