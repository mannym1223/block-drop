using System.Collections.Generic;
using UnityEngine;

public class GridClearManager : MonoBehaviour
{
    
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
        BlockDropManager.Instance.OnDropped.AddListener(CheckIfFull);
    }

	private void OnDisable()
	{
		BlockDropManager.Instance.OnDropped.RemoveListener(CheckIfFull);
	}

	/// <summary>
	/// Checks if ANY row is full, then clears
	/// </summary>
	protected void CheckIfFull()
	{

		//List<int> clearedRows = new();
		for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
		{
			if (CheckIfRowFull(rowIndex))
			{
				// TODO: shift cubes above cleared row down
				//clearedRows.Add(rowIndex);
			}
		}
		/*
		int numCleared = clearedRows.Count;
		foreach (int rowIndex in clearedRows)
		{
			ShiftRowDown(rowIndex);
		}*/
	}

	protected bool CheckIfRowFull(int row)
    {
		for (int cubeIndex = 0; cubeIndex < cubeChecks[row].Length; cubeIndex++) 
		{
			var cubeChecker = cubeChecks[row][cubeIndex];
			if (!cubeChecker.hasBlock)
			{
				// row isn't full so do nothing
				Debug.Log("NOT clear" + cubeChecker.name + " has ");
				if (cubeChecker.cube != null)
				{
					Debug.Log(cubeChecker.cube.name);
				}
				return false;
			}
		}
		Debug.Log("Clear------------");

		// row is full so clear it
		for (int cubeIndex = 0; cubeIndex < cubeChecks[row].Length; cubeIndex++)
		{
			var cubeChecker = cubeChecks[row][cubeIndex];
			Destroy(cubeChecker.cube.gameObject); // TODO: Add flashing effect when destroying
		}

		return true;

		//BlockDropManager.Instance.ShiftAllBlockDown();
	}
}
