using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BlockDropManager : MonoBehaviour
{
    private static BlockDropManager instance;

    public static BlockDropManager Instance 
    { 
        get 
        {
            return instance;
        }
    }

    public GridManager grid;
    public BlockTypeList blockTypes;
    public Transform spawnPoint;
    public PlayerController player;
    public Transform gameOverLimit;
    public GameObject gameOverText;
    public GameObject gameOverScoreText;
    public GameObject gameStartText;

    public UnityEvent OnBlockSpawned;
    public UnityEvent OnStartDropping;
	public UnityEvent OnDropped;
    public UnityEvent<Vector3> OnPlayerTryMove;
    public UnityEvent OnPlayerMoved;
    public UnityEvent OnSingleRowCleared;
    public UnityEvent OnMultiRowCleared;
    public UnityEvent OnGameOver;
    public UnityEvent OnGameStarted;
    public UnityEvent<int> OnScoreChanged;
    public float dropDelay = 0.2f; // used by blocks

	public static readonly string INACTIVE_BLOCK = "InactiveBlock";
	public static readonly string SHIFTED_BLOCK = "ShiftedBlock";

    protected int score;
    protected bool isGameOver;
    protected bool isGameStarted;

    private InputAction restartAction;

	private void Awake()
	{
		instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        restartAction = InputSystem.actions.FindAction("Restart");
        OnDropped?.AddListener(SpawnBlock);
		SpawnBlock();
    }

	private void OnDisable()
	{
        OnBlockSpawned?.RemoveAllListeners();
        OnDropped?.RemoveAllListeners();
        OnPlayerMoved?.RemoveAllListeners();
        OnSingleRowCleared?.RemoveAllListeners();
        OnMultiRowCleared?.RemoveAllListeners();
        OnGameOver?.RemoveAllListeners();
        OnGameStarted?.RemoveAllListeners();
	}

	// Update is called once per frame
	void Update()
    {
        if (!isGameStarted && restartAction?.ReadValue<float>() > 0f)
        {
            isGameStarted = true;
            StartGame();
            OnGameStarted?.Invoke();
        }
        if (isGameOver && restartAction?.ReadValue<float>() > 0f)
        {
            SceneManager.LoadScene(0); // restart the scene
        }
    }

    public void SpawnBlock()
    {
        if (grid.HasCurrentBlock())
        {
            Debug.Log("Cannot spawn new block. Active block exists.");
            return;
        }

        int randomIndex = (int)(Random.value * (blockTypes.BlockTypes.Count));
        var newBlock = blockTypes.BlockTypes[randomIndex];
		//player.activeBlock = Instantiate(newBlock, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        //player.activeBlock.Spawned(); // TODO: replace with listener in player controller
        //grid.SpawnBlock(newBlock);

        //OnBlockSpawned?.Invoke(newBlock);
	}

    public void IncreaseScore(int scoreGained)
    {
        score += scoreGained;
        OnScoreChanged?.Invoke(score);
    }

    public void StartGame()
    {
        Destroy(gameStartText);
        //SpawnBlock();
    }

    public void GameOver()
    {
        gameOverText.SetActive(true);
        gameOverScoreText.GetComponent<TextMeshPro>().text = score.ToString();
        isGameOver = true;

        OnGameOver?.Invoke();
    }
}
