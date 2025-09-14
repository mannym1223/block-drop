using UnityEngine;

public class BlockDropManager : MonoBehaviour
{
    public static BlockDropManager Instance 
    { 
        get 
        { 
            if (Instance == null) Instance = new();
            return Instance;
        }
      private set => Instance = value; 
    }

    public GridManager gridManager;
    public Transform spawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(gridManager.BlockList[0], spawnPoint.position, spawnPoint.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public static void GameOver()
    {
        Debug.Log("Game Over");
    }
}
