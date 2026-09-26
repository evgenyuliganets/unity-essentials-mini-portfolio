using UnityEngine;
using UnityEngine.InputSystem;

namespace _Unity_Essentials.Scripts.Provided_Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class MousePlayerController : MonoBehaviour
    {
        [Header("Movement")] [SerializeField, Min(0f)]
        private float moveSpeed = 3f;

        [Header("Look")] [SerializeField] private Transform cameraTransform;
        [SerializeField, Min(0.01f)] private float mouseSensitivity = 0.1f;
        [SerializeField, Range(1f, 89f)] private float pitchLimit = 80f;

        private Rigidbody _body;
        private Vector2 _moveInput;
        private float _pitch;
        private float _pendingYaw;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();

            if (cameraTransform == null)
            {
                Debug.LogError("Camera Transform is not assigned.", this);
                enabled = false;
                return;
            }

            cameraTransform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                ReleaseCursor();
                return;
            }

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _moveInput = Vector2.zero;
                _pendingYaw = 0f;

                if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                return;
            }

            ReadMovement(keyboard);
            ReadLook(mouse);
        }

        private void ReadMovement(Keyboard keyboard)
        {
            _moveInput = Vector2.zero;

            if (keyboard == null)
                return;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                _moveInput.y += 1f;

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                _moveInput.y -= 1f;

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                _moveInput.x += 1f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                _moveInput.x -= 1f;

            // Prevent faster diagonal movement.
            _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);
        }

        private void ReadLook(Mouse mouse)
        {
            if (mouse == null)
                return;

            Vector2 delta = mouse.delta.ReadValue() * mouseSensitivity;

            _pendingYaw += delta.x;
            _pitch = Mathf.Clamp(_pitch - delta.y, -pitchLimit, pitchLimit);

            // Only the camera tilts up and down.
            cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void FixedUpdate()
        {
            bool canMove = Cursor.lockState == CursorLockMode.Locked;

            // Use the current body rotation so teleports remain authoritative.
            float yaw = _body.rotation.eulerAngles.y;
            yaw += canMove ? _pendingYaw : 0f;
            _pendingYaw = 0f;

            Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
            _body.MoveRotation(targetRotation);

            Vector2 input = canMove ? _moveInput : Vector2.zero;

            Vector3 direction =
                targetRotation * new Vector3(input.x, 0f, input.y);

            Vector3 velocity = direction * moveSpeed;

            // Preserve gravity and vertical movement.
            velocity.y = _body.linearVelocity.y;
            _body.linearVelocity = velocity;
        }

        private void ReleaseCursor()
        {
            _moveInput = Vector2.zero;
            _pendingYaw = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnDisable()
        {
            ReleaseCursor();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                ReleaseCursor();
        }
    }
}