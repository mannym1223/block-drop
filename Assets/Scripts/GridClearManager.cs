using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClearManager : MonoBehaviour
{

	public float rowClearDelay; // used for flashing effect
	public float rowClearInterval;
	public List<GameObject> rows; //parent object of each row of cubes

	protected CubeCheckCollider[][] cubeChecks;

	private void Awake()
	{
		// get grid size from rows
		cubeChecks = new CubeCheckCollider[rows.Count][];
		for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
		{
			var rowCubeChecks = rows[rowIndex].GetComponentsInChildren<CubeCheckCollider>();
			cubeChecks[rowIndex] = new CubeCheckCollider[rowCubeChecks.Length];

			for (int cubeIndex = 0; cubeIndex < rowCubeChecks.Length; cubeIndex++)
			{
				cubeChecks[rowIndex][cubeIndex] = rowCubeChecks[cubeIndex];
			}
		}

        //gridCubes.AddRange(GetComponentsInChildren<GridCheckCollider>());
        
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        BlockDropManager.Instance.OnDropped?.AddListener(CheckIfFull);
    }

	private void OnDisable()
	{
		BlockDropManager.Instance.OnDropped?.RemoveListener(CheckIfFull);
	}

	/// <summary>
	/// Checks if ANY row is full, then clears
	/// </summary>
	protected void CheckIfFull()
	{

		//List<int> clearedRows = new();
		int numCleared = 0;
		for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
		{
			if (CheckIfRowFull(rowIndex))
			{
				numCleared++;
			}
		}

		if (numCleared > 0)
		{
			BlockDropManager.Instance.IncreaseScore(CalculateScore(numCleared));
		}
	}

	protected bool CheckIfRowFull(int row)
    {
		for (int cubeIndex = 0; cubeIndex < cubeChecks[row].Length; cubeIndex++) 
		{
			var cubeChecker = cubeChecks[row][cubeIndex];
			if (!cubeChecker.hasBlock)
			{
				// row isn't full so do nothing
				if (cubeChecker.cube != null)
				{
					Debug.Log(cubeChecker.cube.name);
				}
				return false;
			}
		}

		// row is full so clear it
		StartCoroutine(FlashBlocksInRowThenDestroy(row));
		BlockDropManager.Instance.OnRowCleared?.Invoke();
		return true;
	}

	protected int CalculateScore(int numRows)
	{
		return 1000 + (numRows * numRows * 1000);
	}

	protected IEnumerator FlashBlocksInRowThenDestroy(int row)
	{
		float timeElapsed = 0f;
		while(timeElapsed < rowClearDelay)
		{
			for (int cubeIndex = 0; cubeIndex < cubeChecks[row].Length; cubeIndex++)
			{
				// alternate between visible and invisible
				MeshRenderer render = cubeChecks[row][cubeIndex].cube.GetComponent<MeshRenderer>();
				render.enabled = !render.enabled;
			}
			
			yield return new WaitForSeconds(rowClearInterval);
			timeElapsed += rowClearInterval;
		}

		for (int cubeIndex = 0; cubeIndex < cubeChecks[row].Length; cubeIndex++)
		{
			var cubeChecker = cubeChecks[row][cubeIndex];
			if (cubeChecker.cube != null)
			{
				Destroy(cubeChecker.cube.gameObject); // TODO: Add flashing effect when destroying
			}
			cubeChecker.ResetCollider();
		}
	}
}
