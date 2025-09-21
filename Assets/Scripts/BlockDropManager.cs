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

    public BlockTypeList blockTypes;
    public Transform spawnPoint;
    public PlayerController player;
    public Transform gameOverLimit;
    public GameObject gameOverText;
    public GameObject gameOverScoreText;

	public UnityEvent OnDropped;
    public UnityEvent OnPlayerMoved;
    public UnityEvent OnRowCleared;
    public UnityEvent<int> OnScoreChanged;
    public float dropDelay = 0.2f; // used by blocks

    protected int score;
	protected List<BaseCube> allCubes = new();
    protected bool isGameOver;

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
        OnDropped?.RemoveAllListeners();
	}

	// Update is called once per frame
	void Update()
    {
        if (isGameOver && restartAction?.ReadValue<float>() > 0f)
        {
            SceneManager.LoadScene(0); // restart the scene
        }
    }

    public void SpawnBlock()
    {
        if (player.activeBlock != null)
        {
            Debug.Log("Cannot spawn new block. Active block exists.");
            return;
        }

        int randomIndex = (int)(Random.value * (blockTypes.BlockList.Count));
        var newBlock = blockTypes.BlockList[randomIndex];
		player.activeBlock = Instantiate(newBlock, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        player.activeBlock.Spawned();
        allCubes.AddRange(player.activeBlock.cubes);
	}

    public void IncreaseScore(int scoreGained)
    {
        score += scoreGained;
        OnScoreChanged?.Invoke(score);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        Debug.Log("Score: " + score);

        gameOverText.SetActive(true);
        gameOverScoreText.GetComponent<TextMeshPro>().text = score.ToString();
        isGameOver = true;
    }
}
