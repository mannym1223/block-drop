using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public int numRows;
	public int numCols;
	public int height;


	public float rowClearDelay; // used for flashing effect
	public float rowClearInterval;

	protected BaseCube[,,] Cubes;

	private bool isChecking;

	private void Awake()
	{
		Cubes = new BaseCube[numRows, numCols, height];
        
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
		StartCoroutine(CheckIfAnyLevelsFull());
	}

	/// <summary>
	/// Checks if ANY level is full, then clears
	/// </summary>
	protected IEnumerator CheckIfAnyLevelsFull()
	{
		isChecking = true;
		bool[] unclearLevels = new bool[height];
		int numCleared = 0;
		yield return new WaitForEndOfFrame();
		for (int heightIndex = 0; heightIndex < height; heightIndex++)
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
