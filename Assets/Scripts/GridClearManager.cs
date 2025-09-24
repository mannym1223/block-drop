using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClearManager : MonoBehaviour
{

	public float rowClearDelay; // used for flashing effect
	public float rowClearInterval;
	public List<GameObject> rows; //parent object of each row of cubes

	protected CubeCheckCollider[][] cubeChecks;

	private bool isChecking;

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
		if (isChecking)
		{
			return;
		}
		StartCoroutine(CheckIfFull());
	}

	/// <summary>
	/// Checks if ANY row is full, then clears
	/// </summary>
	protected IEnumerator CheckIfFull()
	{
		isChecking = true;
		bool[] unclearRows = new bool[rows.Count];
		int numCleared = 0;
		yield return new WaitForEndOfFrame();
		for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
		{
			if (CheckIfRowFull(rowIndex))
			{
				numCleared++;
				unclearRows[rowIndex] = false;

				// row is full so clear it
				yield return StartCoroutine(FlashBlocksInRowThenDestroy(rowIndex));
			}
			else
			{
				unclearRows[rowIndex] = true;
			}
		}

		if (numCleared > 0)
		{
			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(MoveRowsDown(unclearRows));

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

	protected bool CheckIfRowFull(int row)
    {
		for (int cubeIndex = 0; cubeIndex < cubeChecks[row].Length; cubeIndex++) 
		{
			var cubeChecker = cubeChecks[row][cubeIndex];
			if (!cubeChecker.hasBlock)
			{
				// row isn't full so do nothing
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
		var waitForInterval = new WaitForSeconds(rowClearInterval);

		while(timeElapsed < rowClearDelay)
		{
			for (int cubeIndex = 0; cubeIndex < cubeChecks[row].Length; cubeIndex++)
			{
				// alternate between visible and invisible
				if (cubeChecks[row][cubeIndex].cube != null)
				{
					MeshRenderer render = cubeChecks[row][cubeIndex].cube.GetComponent<MeshRenderer>();
					render.enabled = !render.enabled;
				}
			}
			
			yield return waitForInterval;
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

	protected IEnumerator MoveRowsDown(bool[] unclearRows)
	{
		var wait = new WaitForSeconds(BlockDropManager.Instance.dropDelay);

		int dropDistance = 0;
		// 1 2 drop
		for (int index = 0; index < unclearRows.Length; index++)
		{
			if (!unclearRows[index]) // skip cleared rows
			{
				dropDistance++;
				continue;
			}

			int distanceMoved = 0;
			// move entire row downwards
			foreach (CubeCheckCollider cubeCheck in cubeChecks[index])
			{
				BaseCube cube = cubeCheck.cube;
				if (cube == null) // no cube in this position
				{
					continue;
				}
				cube.GetComponent<Collider>().enabled = false;
				cube.gameObject.layer = LayerMask.NameToLayer(BlockDropManager.SHIFTED_BLOCK);

				cube.ShiftDown(dropDistance);
				cubeCheck.ResetCollider();
				cube.GetComponent<Collider>().enabled = true;
			}

			distanceMoved++;
			yield return wait;
		}
	}

	protected void ResetAllColliders()
	{
		foreach (CubeCheckCollider[] cubeList in cubeChecks)
		{
			foreach(CubeCheckCollider cube in cubeList)
			{
				cube.ResetCollider();
				
			}
		}
	}
}
