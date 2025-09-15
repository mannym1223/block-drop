using System.Collections.Generic;
using UnityEngine;

public class GridClearManager : MonoBehaviour
{
    public List<GridCheckCollider> gridCubes = new();

	private void Awake()
	{
        gridCubes.AddRange(GetComponentsInChildren<GridCheckCollider>());
        
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

	protected void CheckIfFull()
    {
		foreach (GridCheckCollider gridChecker in gridCubes) 
		{
			if (!gridChecker.hasBlock)
			{
				// row isn't full so do nothing
				Debug.Log("NOT clear" + gridChecker.name + " has ");
				if (gridChecker.cube != null)
				{
					Debug.Log(gridChecker.cube.name);
				}
				return;
			}
		}
		Debug.Log("Clear------------");

		// row is full so clear it
		foreach (GridCheckCollider gridChecker in gridCubes)
		{
			Destroy(gridChecker.cube.gameObject); // TODO: Add flashing effect when destroying
		}

		BlockDropManager.Instance.ShiftAllBlockDown();
	}
}
