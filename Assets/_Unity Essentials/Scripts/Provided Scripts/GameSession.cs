using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public int CorrectAnswers { get; set; }
    public bool IsIntroduction { get; set; } = true;
    public bool HasWon { get; set; }
    
    public string PendingFeedback { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void ResetSession()
    {
        CorrectAnswers = 0;
        IsIntroduction = true;
        HasWon = false;
        PendingFeedback = null;
    }
}