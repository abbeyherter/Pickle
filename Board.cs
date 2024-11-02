using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    // Array alphabet for user input
    private static readonly KeyCode[] SUPPORTED_KEYS = {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z
    };

    // Reference to inital and guessing rows
    private Row initialRow;
    private Row guessingRow;

    // Array for valid words
    private string[] solutions;
    private string[] validWords;

    // Current word
    private string word;

    // Track input in row
    private int columnIndex;

    // Check inital guess
    private bool isInitialGuessMade = false;

    // Timer
    private Timer timer;

    // Check game status
    private bool gameOver = false; // Flag to stop the game

    // Types of tile states
    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    // UI text and buttons
    [Header("UI")]
    public TextMeshProUGUI invalidWordText; // Message for invalid words
    public TextMeshProUGUI youWinText;      // "You Win" message
    public TextMeshProUGUI youLoseText;     // "You Lose" message
    public GameObject newWordButton;        // "New Word" button
    public GameObject restartButton;        // "Start Over" button

    private void Awake()
    {
        // Initialize rows from child
        Row[] rows = GetComponentsInChildren<Row>();
        initialRow = rows[0];
        guessingRow = rows[1];

        // Find timer
        timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.board = this;
        }
    }

    private void Start()
    {   
        // Load and start game
        LoadData();
        NewGame();
    }

    public void GameOverDueToTime()
    {
        // If timer expires, display "better luck next time" text
        gameOver = true; 
        youLoseText.gameObject.SetActive(true); 
        invalidWordText.gameObject.SetActive(false); 
    }

    public void NewGame()
    {
        // Method for new game
        SetRandomWord();
        ResetGameState();
        youWinText.gameObject.SetActive(false);
        youLoseText.gameObject.SetActive(false);
        invalidWordText.gameObject.SetActive(false);
        gameOver = false;
        timer.StartTimer();
    }

    public void RestartGame()
    {
        // Method for restarting game
        ResetGameState();
        youWinText.gameObject.SetActive(false);
        youLoseText.gameObject.SetActive(false);
        invalidWordText.gameObject.SetActive(false);
        gameOver = false;
        timer.StartTimer();
    }

    private void ResetGameState()
    {
        // Reset game by clearing rows and reset variables
        ClearRow(initialRow);
        ClearRow(guessingRow);
        columnIndex = 0;
        isInitialGuessMade = false;
        invalidWordText.gameObject.SetActive(false);
        enabled = true;
    }

    private void LoadData()
    {
        // Load valid words 
        validWords = Resources.Load<TextAsset>("official_wordle_all").text.Split('\n');
        solutions = Resources.Load<TextAsset>("official_wordle_common").text.Split('\n');
    }

    private void SetRandomWord()
    {
        // Select random word from word list
        word = solutions[Random.Range(0, solutions.Length)].Trim().ToLower();
    }

    private void Update()
    {
        if (gameOver) return; 

        // Check user input
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            HandleBackspace();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isInitialGuessMade && columnIndex >= initialRow.tiles.Length)
            {
                SubmitInitialGuess();
            }
            else if (isInitialGuessMade && columnIndex >= guessingRow.tiles.Length)
            {
                SubmitSubsequentGuess();
            }
        }
        else
        {
            HandleKeyPress();
        }
    }

    private void HandleBackspace()
    {
        if (gameOver) return;

        // Handles removing letters by backspacing
        columnIndex = Mathf.Max(columnIndex - 1, 0);
        Row activeRow = isInitialGuessMade ? guessingRow : initialRow;
        activeRow.tiles[columnIndex].SetLetter('\0');
        activeRow.tiles[columnIndex].SetState(emptyState);
        invalidWordText.gameObject.SetActive(false);
    }

    private void HandleKeyPress()
    {
        if (gameOver) return;

        // Choose active row
        Row activeRow = isInitialGuessMade ? guessingRow : initialRow;

        // Handle letter key input for board placement
        if (columnIndex >= activeRow.tiles.Length) return;

        foreach (KeyCode key in SUPPORTED_KEYS)
        {
            if (Input.GetKeyDown(key))
            {
                char letter = (char)key;
                activeRow.tiles[columnIndex].SetLetter(letter);
                activeRow.tiles[columnIndex].SetState(occupiedState);
                columnIndex++;
                break;
            }
        }
    }

    private void SubmitInitialGuess()
    {
        if (gameOver) return;

        // Check if word is invalid
        if (!IsValidWord(initialRow.word))
        {
            invalidWordText.gameObject.SetActive(true);
            return;
        }

        // Sumbit inital guess
        ProcessGuess(initialRow);
        isInitialGuessMade = true;
        timer.StartTimer();
        columnIndex = 0; 
        ClearRow(guessingRow);

        // Check if word entered matches the random word
        if (HasWon(initialRow))
        {
            youWinText.gameObject.SetActive(true);
            timer.StopTimer();
            gameOver = true;
        }
    }

    private void SubmitSubsequentGuess()
    {
        if (gameOver) return;

        // Check if word is invalid
        if (!IsValidWord(guessingRow.word))
        {
            invalidWordText.gameObject.SetActive(true);
            timer.DeductTime(10f);
            return;
        }

        // Process guesses
        ProcessGuess(guessingRow);

        // Check if word entered matches the random word 
        if (HasWon(guessingRow))
        {
            youWinText.gameObject.SetActive(true);
            timer.StopTimer();
            gameOver = true;
        }
        else
        // Reset for next guess
        {
            columnIndex = 0;
            ClearRow(guessingRow);
        }
    }

    private void ProcessGuess(Row row)
    {
        string remaining = word;

        // Check for correct letters in correct position
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];
            char letter = tile.letter;

            if (letter == word[i])
            {
                tile.SetState(correctState);
                remaining = remaining.Remove(i, 1).Insert(i, " ");
            }
        }

        // Check for correct letters in wrong position
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];
            if (tile.state == correctState) continue;

            char letter = tile.letter;
            if (remaining.Contains(letter))
            {
                tile.SetState(wrongSpotState);
                int index = remaining.IndexOf(letter);
                remaining = remaining.Remove(index, 1).Insert(index, " ");
            }
            else
            {
                tile.SetState(incorrectState);
            }
        }
    }

    private void ClearRow(Row row)
    {
        // Clear tiles in row and set to empty state
        foreach (Tile tile in row.tiles)
        {
            tile.SetLetter('\0');
            tile.SetState(emptyState);
        }
    }

    private bool IsValidWord(string word)
    {
        // Method to check if the word is valid
        word = word.Trim().ToLower();
        foreach (string validWord in validWords)
        {
            if (validWord.Trim().ToLower() == word) return true;
        }
        return false;
    }

    private bool HasWon(Row row)
    {
        // Check if all tiles in row are in their correct positions
        foreach (Tile tile in row.tiles)
        {
            if (tile.state != correctState) return false;
        }
        return true;
    }
}
