using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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
        BlockDropManager.Instance.OnDropped?.AddListener(StartCheckIfFull);
    }

	private void OnDisable()
	{
		BlockDropManager.Instance.OnDropped?.RemoveListener(StartCheckIfFull);
		StopAllCoroutines();
	}

	public void StartCheckIfFull()
	{
		StartCoroutine(CheckIfFull());
	}

	/// <summary>
	/// Checks if ANY row is full, then clears
	/// </summary>
	protected IEnumerator CheckIfFull()
	{

		List<int> unclearRows = new();
		int numCleared = 0;
		for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
		{
			if (CheckIfRowFull(rowIndex))
			{
				numCleared++;

				// row is full so clear it
				yield return StartCoroutine(FlashBlocksInRowThenDestroy(rowIndex));
			}
			else
			{
				unclearRows.Add(rowIndex);
			}
		}

		if (numCleared > 0)
		{
			if (numCleared > 1)
			{
				BlockDropManager.Instance.OnMultiRowCleared?.Invoke();
			}
			else
			{
				BlockDropManager.Instance.OnSingleRowCleared?.Invoke();
			}
			BlockDropManager.Instance.IncreaseScore(CalculateScore(numCleared));
			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(MoveRowsDown(unclearRows));
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

	protected IEnumerator MoveRowsDown(List<int> rows)
	{
		var wait = new WaitForSeconds(BlockDropManager.Instance.dropDelay);

		foreach (int row in rows)
		{
			bool rowDropped = false;
			while (!rowDropped)
			{
				foreach (CubeCheckCollider cubeCheck in cubeChecks[row])
				{
					if (cubeCheck.cube == null) // no cube in this position
					{
						continue;
					}
					else if(!cubeCheck.cube.CanDrop()) // cube has landed
					{
						rowDropped = true;
						cubeCheck.ResetCollider();
					}
					else // cube still moving down
					{
						cubeCheck.cube.ShiftDown();
					}
				}

				yield return wait;
			}
		}
	}
}
