using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    // UI Elements
    public TextMeshProUGUI taskText;
    public Image taskTextBackground; // Reference to the background image
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI taskDisplay;
    public Button startButton;
    public Button restartButton;

    // Player and components
    public GameObject player;
    private PlayerInventory playerInventory;
    private PlayerController playerController;

    // Snake(s) in the scene
    public List<SnakeController> snakes;

    // Player's initial position
    private Vector3 playerInitialPosition;

    // Music and sound effects
    public AudioClip backgroundMusic;
    public AudioClip successMusic;
    public AudioClip failureMusic;
    public AudioClip loseLifeSound;

    // Timer and game state
    private float timeLimit = 120f; // 2 minutes
    private bool gameStarted = false;
    private bool gameEnded = false;

    // Plant collection
    private int requiredPlant1ID;
    private int requiredPlant2ID;
    private string[] plantNames = {
        "Rafflesia Arnoldii", "Ginger", "Orchid", "Protea",
        "Agapanthus", "Mustard", "Nightshade", "Pimpernel"
    };

    // Lives and hearts
    public Image[] hearts;
    public int maxLives = 3;
    private int currentLives;

    // Audio source
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerInventory = player.GetComponent<PlayerInventory>();
        playerController = player.GetComponent<PlayerController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        playerInitialPosition = player.transform.position;

        foreach (var snake in snakes)
        {
            snake.SaveInitialPosition();
        }

        if (playerController != null)
            playerController.canMove = false;

        currentLives = maxLives;
        UpdateHeartsUI();

        SelectRandomPlants();
        InitializeUI();
    }

    void InitializeUI()
    {
        taskText.text = $"Find the {plantNames[requiredPlant1ID]} and {plantNames[requiredPlant2ID]} in 2 minutes to finish the potion!";
        timerText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        taskDisplay.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        taskTextBackground.gameObject.SetActive(true); // Ensure the background is initially visible

        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    void StartGame()
    {
        startButton.gameObject.SetActive(false);
        taskText.gameObject.SetActive(false);
        taskTextBackground.gameObject.SetActive(false); // Hide the background image
        timerText.gameObject.SetActive(true);
        taskDisplay.gameObject.SetActive(true);
        gameStarted = true;

        if (playerController != null)
            playerController.canMove = true;

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (gameStarted && !gameEnded)
        {
            UpdateTaskDisplay();
            UpdateTimer();

            if (CheckForRequiredPlants())
            {
                EndGame(true);
            }
        }
    }

    void UpdateTimer()
    {
        timeLimit -= Time.deltaTime;

        if (timeLimit <= 0)
        {
            timeLimit = 0;
            EndGame(false, "time");
        }

        int minutes = Mathf.FloorToInt(timeLimit / 60);
        int seconds = Mathf.FloorToInt(timeLimit % 60);
        timerText.text = $"Time Left: {minutes:00}:{seconds:00}";
    }

    void UpdateTaskDisplay()
    {
        if (playerInventory != null)
        {
            int collectedPlant1 = playerInventory.GetPlantCount(requiredPlant1ID);
            int collectedPlant2 = playerInventory.GetPlantCount(requiredPlant2ID);

            taskDisplay.text = $"{plantNames[requiredPlant1ID]} × {collectedPlant1}    {plantNames[requiredPlant2ID]} × {collectedPlant2}";
        }
    }

    bool CheckForRequiredPlants()
    {
        int collectedPlant1 = playerInventory.GetPlantCount(requiredPlant1ID);
        int collectedPlant2 = playerInventory.GetPlantCount(requiredPlant2ID);

        return collectedPlant1 >= 1 && collectedPlant2 >= 1;
    }

    void SelectRandomPlants()
    {
        do
        {
            requiredPlant1ID = Random.Range(0, plantNames.Length);
            requiredPlant2ID = Random.Range(0, plantNames.Length);
        } while (requiredPlant1ID == requiredPlant2ID);
    }

    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            PlaySound(loseLifeSound);
            UpdateHeartsUI();

            if (currentLives <= 0)
            {
                EndGame(false, "lives");
            }
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentLives;
        }
    }

    void EndGame(bool success, string failureReason = "")
    {
        gameEnded = true;

        audioSource.Stop();
        if (playerController != null)
            playerController.canMove = false;

        resultText.gameObject.SetActive(true);

        if (success)
        {
            resultText.text = "Congratulations!\nYou have successfully gathered the plants!";
            PlaySound(successMusic);
        }
        else
        {
            if (failureReason == "time")
            {
                resultText.text = "Time's up!\nYou failed to gather the necessary plants.";
            }
            else if (failureReason == "lives")
            {
                resultText.text = "You ran out of lives!\nYou failed to gather the necessary plants.";
            }
            PlaySound(failureMusic);
        }

        restartButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        timeLimit = 120f;
        gameStarted = false;
        gameEnded = false;

        resultText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        taskText.gameObject.SetActive(true);
        taskTextBackground.gameObject.SetActive(true); // Show the background image again
        startButton.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        taskDisplay.gameObject.SetActive(false);

        player.transform.position = playerInitialPosition;

        playerInventory.ClearInventory();

        currentLives = maxLives;
        UpdateHeartsUI();

        foreach (var snake in snakes)
        {
            snake.ResetToInitialPosition();
        }

        SelectRandomPlants();
        InitializeUI();
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
