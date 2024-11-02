using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // Set start time for timer
    public float startTime = 180f;
    private float currentTime;
    private bool timerRunning = false;

    // Time displayed 
    public TextMeshProUGUI timerText;
    public Board board; 

    private void Start()
    {
        // Initialize current time to start time
        currentTime = startTime;
        UpdateTimerDisplay();

        // Assign Board component
        if (board == null)
        {
            board = FindObjectOfType<Board>();
        }
    }

    private void Update()
    {
        if (timerRunning)
        {
            // Decrement time by time elapsed
            currentTime -= Time.deltaTime;

            // If time runs out stop timer and end game
            if (currentTime <= 0)
            {
                currentTime = 0;
                timerRunning = false;
                TimerEnded();
            }

            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        // Start/restart timer
        currentTime = startTime;
        timerRunning = true;
        UpdateTimerDisplay();
    }

    public void StopTimer()
    {
        // Stop timer
        timerRunning = false;
    }

    public void DeductTime(float seconds)
    {
        // Subtract seconds from current time
        currentTime -= seconds;

        // If time runs out stop timer and end game
        if (currentTime <= 0)
        {
            currentTime = 0;
            timerRunning = false;
            TimerEnded();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        // Update timer text to display time
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void TimerEnded()
    {
        // Method for timer ends
        if (board != null)
        {
            board.GameOverDueToTime();
        }
    }
}
