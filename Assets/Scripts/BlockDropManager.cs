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

	public UnityEvent OnDropped;
    public float dropDelay = 0.2f; // used by blocks

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
        Debug.Log("Hello");
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnAction.ReadValue<float>() > 0f)
        {
            SpawnBlock();
            Debug.Log("Spawning");
        }
    }

    public void SpawnBlock()
    {
        if (player.activeBlock != null)
        {
            Debug.Log("Cannot spawn new block. Active block exists.");
            return;
        }
		player.activeBlock = Instantiate(gridManager.BlockList[0], spawnPoint.position, spawnPoint.rotation, spawnPoint);
	}

    /// <summary>
    /// 
    /// </summary>
    public static void GameOver()
    {
        Debug.Log("Game Over");
    }
}
