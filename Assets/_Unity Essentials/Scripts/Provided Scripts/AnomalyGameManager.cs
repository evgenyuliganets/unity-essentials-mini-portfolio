using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _Unity_Essentials.Scripts.Provided_Scripts
{
    internal enum AnomalyType
    {
        MissingObject,
        FloatingObject,
        LampColor
    }

    public class AnomalyGameManager : MonoBehaviour
    {
        [Header("Player")] [SerializeField] private Rigidbody playerBody;
        [SerializeField] private Transform spawnPoint;

        [Header("Missing Objects Pool")] [SerializeField]
        private List<GameObject> missingObjectsPool;

        [Header("Floating Objects Pool")] [SerializeField]
        private List<GameObject> floatingObjectsPool;

        [Header("Lamp")] [SerializeField] private Light lampLight;

        [Header("Rules")] [SerializeField, Min(1)]
        private int requiredCorrectAnswers = 3;

        [Header("UI")] [SerializeField] private TMP_Text progressText;
        [SerializeField] private TMP_Text messageText;

        private bool _hasAnomaly;
        private AnomalyType _currentAnomalyType;
        private float _nextAllowedAnswerTime;

        private bool IsIntroduction => GameSession.Instance.IsIntroduction;
        private bool HasWon => GameSession.Instance.HasWon;
        private int CorrectAnswers => GameSession.Instance.CorrectAnswers;

        private void Start()
        {
            LockCursor();
            
            if (!ValidateReferences())
                return;

            _nextAllowedAnswerTime = Time.time + 1f;

            TeleportPlayer();

            if (HasWon)
            {
                UpdateProgressUI();
                ShowMessage("SHIFT COMPLETE!\nPress R to play again.");
                return;
            }

            if (IsIntroduction)
            {
                _hasAnomaly = false;

                UpdateProgressUI();
                ShowMessage(
                    "Memorize the room.\nEverything is normal. Choose the NORMAL exit."
                );

                return;
            }

            StartRound();
        }

        private void Update()
        {
            if (HasWon &&
                Keyboard.current != null &&
                Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartGame();
            }
        }

        public void SubmitAnswer(bool answeredAnomaly, Rigidbody enteringBody)
        {
            if (!enabled ||
                HasWon ||
                enteringBody != playerBody)
            {
                return;
            }

            // Prevent duplicate submissions.
            if (Time.time < _nextAllowedAnswerTime)
                return;

            _nextAllowedAnswerTime = Time.time + 1f;

            bool isCorrect = answeredAnomaly == _hasAnomaly;

            if (IsIntroduction)
            {
                HandleIntroductionAnswer(isCorrect);
                return;
            }

            HandleRoundAnswer(isCorrect);
        }

        private void HandleIntroductionAnswer(bool isCorrect)
        {
            if (!isCorrect)
            {
                ShowMessage(
                    "This room is normal.\nChoose the NORMAL exit."
                );

                TeleportPlayer();
                return;
            }

            // Introduction does not count toward the streak.
            GameSession.Instance.IsIntroduction = false;

            ReloadScene();
        }

        private void HandleRoundAnswer(bool isCorrect)
        {
            if (isCorrect)
            {
                GameSession.Instance.CorrectAnswers++;
            }
            else
            {
                GameSession.Instance.CorrectAnswers = 0;
            }

            if (GameSession.Instance.CorrectAnswers >= requiredCorrectAnswers)
            {
                GameSession.Instance.HasWon = true;
            }

            // Reloading restores the entire room to its original scene state.
            ReloadScene();
        }

        private void StartRound()
        {
            // Scene was just loaded, so the room is already in its normal state.

            _hasAnomaly = Random.value < 0.5f;

            if (_hasAnomaly)
            {
                ApplyRandomAnomaly();
            }

            UpdateProgressUI();

            ShowMessage(
                "Inspect the room.\nLook for anything unusual."
            );
        }

        private void ApplyRandomAnomaly()
        {
            _currentAnomalyType = (AnomalyType)Random.Range(
                0,
                Enum.GetValues(typeof(AnomalyType)).Length
            );

            switch (_currentAnomalyType)
            {
                case AnomalyType.MissingObject:
                    ApplyMissingObjectAnomaly();
                    break;

                case AnomalyType.FloatingObject:
                    ApplyFloatingObjectAnomaly();
                    break;

                case AnomalyType.LampColor:
                    ApplyLampColorAnomaly();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyMissingObjectAnomaly()
        {
            if (missingObjectsPool == null ||
                missingObjectsPool.Count == 0)
            {
                Debug.LogWarning(
                    "Missing Objects Pool is empty.",
                    this
                );

                return;
            }

            int index = Random.Range(
                0,
                missingObjectsPool.Count
            );

            missingObjectsPool[index].SetActive(false);
        }

        private void ApplyFloatingObjectAnomaly()
        {
            if (floatingObjectsPool == null ||
                floatingObjectsPool.Count == 0)
            {
                Debug.LogWarning(
                    "Floating Objects Pool is empty.",
                    this
                );

                return;
            }

            int index = Random.Range(
                0,
                floatingObjectsPool.Count
            );

            SetFloatObject(
                floatingObjectsPool[index]
            );
        }

        private void ApplyLampColorAnomaly()
        {
            if (lampLight != null)
            {
                lampLight.color = Color.red;
            }
        }

        private void SetFloatObject(GameObject obj)
        {
            obj.transform.position += Vector3.up * 1.5f;

            Rigidbody rb = obj.GetComponent<Rigidbody>();

            if (rb == null)
                return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
        }

        public void RestartGame()
        {
            GameSession.Instance.ResetSession();

            ReloadScene();
        }

        private void ReloadScene()
        {
            Scene currentScene =
                SceneManager.GetActiveScene();

            SceneManager.LoadScene(
                currentScene.buildIndex
            );
        }

        private void TeleportPlayer()
        {
            playerBody.linearVelocity = Vector3.zero;
            playerBody.angularVelocity = Vector3.zero;

            playerBody.position =
                spawnPoint.position;

            playerBody.rotation =
                spawnPoint.rotation;
        }

        private void UpdateProgressUI()
        {
            if (progressText == null)
                return;

            if (HasWon)
            {
                progressText.text =
                    $"CORRECT ANSWERS: {CorrectAnswers}/{requiredCorrectAnswers}";

                return;
            }

            progressText.text = IsIntroduction
                ? "OBSERVATION"
                : $"CORRECT ANSWERS: {CorrectAnswers}/{requiredCorrectAnswers}";
        }

        private void ShowMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }

            Debug.Log(message, this);
        }

        private bool ValidateReferences()
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError(
                    "GameSession was not found.",
                    this
                );

                enabled = false;
                return false;
            }

            if (playerBody == null ||
                spawnPoint == null)
            {
                Debug.LogError(
                    "Assign Player Body and Spawn Point.",
                    this
                );

                enabled = false;
                return false;
            }

            return true;
        }
        
        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}