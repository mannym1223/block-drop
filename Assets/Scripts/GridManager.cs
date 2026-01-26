using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public int numRows;
	public int numCols;
	public int gridHeight;

	public float rowClearDelay; // used for flashing effect
	public float rowClearInterval;

	public BaseCube cubePrefab;

	[SerializeField]
	protected BaseCube[,,] Cubes;

	[SerializeField]
	protected HashSet<BaseCube> currentCubes = new();

	private bool isChecking;

	private BlockDropManager dropManager;

	private void Awake()
	{
		Cubes = new BaseCube[numRows, gridHeight, numCols];
		dropManager = BlockDropManager.Instance;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		dropManager.OnDropped?.AddListener(StartCheckIfFull);
		dropManager.OnStartDropping?.AddListener(DropBlock);
	}
	
	private void OnDisable()
	{
		dropManager.OnDropped?.RemoveListener(StartCheckIfFull);
		dropManager.OnStartDropping?.RemoveListener(DropBlock);
		StopAllCoroutines();
	}
	
	/// <summary>
	/// Adds the spawned block's cubes to grid
	/// </summary>
	/// <param name="blockPrefab"></param>
	public void SpawnBlock(BlockScriptable blockPrefab)
	{
		Debug.Log("Spawning block...");

		Material cubeMat = dropManager.blockTypes.materials[(int)(Random.value * (dropManager.blockTypes.materials.Count))];

		var cubeSpawns = blockPrefab.GetRandomRotationSpawns();
		for (int index = 0; index < cubeSpawns.Count; index++)
		{
			Transform spawnPoint = dropManager.spawnPoint;
			Vector3Int spawnIndex = cubeSpawns[index];
			BaseCube cube = Instantiate(cubePrefab, spawnPoint.position - spawnIndex, spawnPoint.rotation, spawnPoint);
			cube.GetComponent<Renderer>().material = cubeMat;

			Cubes[spawnIndex.x, spawnIndex.y, spawnIndex.z] = cube;
			currentCubes.Add(cube);
		}

		dropManager.OnBlockSpawned?.Invoke();
		Debug.Log("Spawned " + blockPrefab);
	}

	public void DropBlock()
	{
		Debug.Log("Drop block-------");
		StartCoroutine(StartDroppingBlock());
	}

	public void StartCheckIfFull()
	{
		if (isChecking)
		{
			return;
		}
		StartCoroutine(CheckIfAnyLevelsFull());
	}

	protected IEnumerator StartDroppingBlock()
	{
		while (CanDropBlock(1))
		{
			foreach (var cube in currentCubes)
			{
				// move cube down and update grid
				cube.transform.Translate(Vector3.down);
				Debug.Log(cube.gridCell);
				Cubes[cube.gridCell.x, cube.gridCell.y, cube.gridCell.z] = null;
				cube.gridCell.y++;
				Cubes[cube.gridCell.x, cube.gridCell.y, cube.gridCell.z] = cube;
			}
			yield return new WaitForSeconds(dropManager.dropDelay);
		}
		
		currentCubes.Clear();
	}

	protected bool CanDropBlock(int distance)
	{
		foreach(BaseCube cube in currentCubes)
		{
			Vector3Int currentCell = cube.gridCell;
			if(cube.gridCell.y >= gridHeight)
			{
				Debug.Log("Cannot drop: reached last cell");
				return false;
			}
			BaseCube cubeBelow = Cubes[currentCell.x, currentCell.y + 1, currentCell.z];

			// Check if any are INVALID and return false if they are
			if (currentCell != null) // null check
			{
				if(cubeBelow == null) // cell below is empty so cube can move
				{
					continue;
				}
				else if (!currentCubes.Contains(cubeBelow)) // cell below exists and isn't part of current block
				{
					Debug.Log("Cannot drop");
					return false;
				}
			}
		}
		Debug.Log("Can drop");
		return true;
	}

	/// <summary>
	/// Checks if ANY level is full, then clears
	/// </summary>
	protected IEnumerator CheckIfAnyLevelsFull()
	{
		isChecking = true;
		bool[] unclearLevels = new bool[gridHeight];
		int numCleared = 0;
		yield return new WaitForEndOfFrame();
		for (int heightIndex = 0; heightIndex < gridHeight; heightIndex++)
		{
			if (CheckIfHeightLevelFull(heightIndex))
			{
				numCleared++;
				unclearLevels[heightIndex] = false;

				// row is full so clear it
				yield return StartCoroutine(FlashBlocksInHeightLevelThenDestroy(heightIndex));
			}
			else
			{
				unclearLevels[heightIndex] = true;
			}
		}

		if (numCleared > 0)
		{
			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(MoveLevelsDown(unclearLevels));

			if (numCleared > 1)
			{
				BlockDropManager.Instance.OnMultiRowCleared?.Invoke();
			}
			else
			{
				BlockDropManager.Instance.OnSingleRowCleared?.Invoke();
			}
			Debug.Log("Num cleared: " + numCleared);
			
			BlockDropManager.Instance.IncreaseScore(CalculateScore(numCleared));
		}
		isChecking = false;
	}

	/// <summary>
	/// Checks every space at the specified height. Returns false if any are empty
	/// </summary>
	/// <param name="heightLevel"></param>
	/// <returns></returns>
	protected bool CheckIfHeightLevelFull(int heightLevel)
    {
		for (int rowIndex =  0; rowIndex < numRows; rowIndex++) 
		{
			for (int colIndex = 0; colIndex < numCols; colIndex++)
			{
				if (Cubes[rowIndex, colIndex, heightLevel] != null)
				{
					// level isn't full so do nothing
					return false;
				}
			}
		}

		return true;
	}

	protected int CalculateScore(int numRows)
	{
		return 1000 + (numRows * numRows * 1000);
	}

	protected IEnumerator FlashBlocksInHeightLevelThenDestroy(int heightLevel)
	{
		float timeElapsed = 0f;
		var waitForInterval = new WaitForSeconds(rowClearInterval);

		while(timeElapsed < rowClearDelay)
		{
			for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
			{
				for (int colIndex = 0; colIndex < numCols; colIndex++)
				{
					// alternate between visible and invisible
					if (Cubes[rowIndex, colIndex, heightLevel] != null)
					{
						MeshRenderer render = Cubes[rowIndex, colIndex, heightLevel].GetComponent<MeshRenderer>();
						render.enabled = !render.enabled;
					}
				}
			}
			
			yield return waitForInterval;
			timeElapsed += rowClearInterval;
		}


		for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
		{
			for (int colIndex = 0; colIndex < numCols; colIndex++)
			{
				Destroy(Cubes[rowIndex, colIndex, heightLevel].gameObject);
			}
		}
	}

	protected IEnumerator MoveLevelsDown(bool[] unclearLevels)
	{
		var wait = new WaitForSeconds(BlockDropManager.Instance.dropDelay);

		int dropDistance = 0;
		// 1 2 drop
		for (int heightIndex = 0; heightIndex < unclearLevels.Length; heightIndex++)
		{
			if (!unclearLevels[heightIndex]) // skip cleared rows
			{
				dropDistance++;
				continue;
			}

			int distanceMoved = 0;
			// move entire level downwards
			for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
			{
				for (int colIndex = 0; colIndex < numCols; colIndex++)
				{
					BaseCube cube = Cubes[rowIndex, colIndex, heightIndex];
					if (cube == null) // no cube in this position
					{
						continue;
					}
					//cube.GetComponent<Collider>().enabled = false;
					//cube.gameObject.layer = LayerMask.NameToLayer(BlockDropManager.SHIFTED_BLOCK);

					ShiftCubeDown(cube, dropDistance);
					Cubes[rowIndex, colIndex, heightIndex] = null;
					Cubes[rowIndex, colIndex, heightIndex - 1] = cube;
					//cubeCheck.ResetCollider();
					//cube.GetComponent<Collider>().enabled = true;
				}
			}

			distanceMoved++;
			yield return wait;
		}
	}

	protected void ShiftCubeDown(BaseCube cube, int dropDistance)
	{
		cube.transform.Translate(new Vector3(0f, (float)dropDistance, 0f));
	}
}
