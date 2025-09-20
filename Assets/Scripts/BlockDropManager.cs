using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
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
    public UnityEvent<int> OnScoreChanged;
    public float dropDelay = 0.2f; // used by blocks

    protected int score;
	protected List<BaseCube> allCubes = new();
    protected bool isGameOver;

	private InputAction spawnAction;
    private InputAction restartAction;

	private void Awake()
	{
		instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        spawnAction = InputSystem.actions.FindAction("SpawnBlock");
        restartAction = InputSystem.actions.FindAction("Restart");
		SpawnBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnAction.ReadValue<float>() > 0f)
        {
            SpawnBlock();
        }
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
        player.activeBlock = Instantiate(blockTypes.BlockList[randomIndex], spawnPoint.position, spawnPoint.rotation, spawnPoint);
        //CheckIfGameOver();
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
