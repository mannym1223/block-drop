using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

    public GridManager gridManager;
    public Transform spawnPoint;
    public PlayerController player;
    public Transform gameOverLimit;

	public UnityEvent OnDropped;
    public float dropDelay = 0.2f; // used by blocks

	protected List<BaseCube> allCubes = new();

	private InputAction spawnAction;

	private void Awake()
	{
		instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        spawnAction = InputSystem.actions.FindAction("SpawnBlock");
		SpawnBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnAction.ReadValue<float>() > 0f)
        {
            SpawnBlock();
        }
    }

    public void SpawnBlock()
    {
        if (player.activeBlock != null)
        {
            Debug.Log("Cannot spawn new block. Active block exists.");
            return;
        }
        int randomIndex = (int)(Random.value * (gridManager.BlockList.Count));
        player.activeBlock = Instantiate(gridManager.BlockList[randomIndex], spawnPoint.position, spawnPoint.rotation, spawnPoint);
        //CheckIfGameOver();
        allCubes.AddRange(player.activeBlock.cubes);
	}

    public void CheckIfGameOver()
    {
        foreach (BaseCube cube in player.activeBlock.cubes)
        {
            
        }
    }

    public void ShiftAllBlockDown()
    {
        foreach (BaseCube cube in allCubes)
        {
            cube.ShiftDown();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
