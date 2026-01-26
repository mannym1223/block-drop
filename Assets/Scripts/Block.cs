using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public List<float> rotations = new List<float>(); // used when spawning
    public BlockTypeList blockTypes; // used for block colors
    public GameObject outlineBox;

    [Tooltip("Specifies where the cubes will spawn in the grid. Must have the same number of indices as cubes.")]
    public List<Vector3Int> cubeIndices = new List<Vector3Int>();

    public int height;

    [HideInInspector]
    public List<BaseCube> cubes = new();

	private void Awake()
	{
        cubes.AddRange(GetComponentsInChildren<BaseCube>());
		if (blockTypes.materials.Count > 0)
		{
            Material newMaterial = blockTypes.materials[(int)(Random.value * (blockTypes.materials.Count))];

			foreach (BaseCube cube in cubes)
            {
                cube.GetComponent<MeshRenderer>().material = newMaterial;
            }
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Spawned()
    {
		if (rotations.Count > 1)
		{
			int randomIndex = (int)(Random.value * (rotations.Count));
			transform.rotation = Quaternion.Euler(0f, rotations[randomIndex], 0f);
		}
        
	}

	public virtual void Drop()
    {
        Destroy(outlineBox);
        StartCoroutine(StartDropping());
	}

	public virtual bool CanMove(Vector3 direction, float distance)
    {
        foreach(BaseCube cube in cubes)
        {
            if(!cube.CanMove(direction, distance))
            {
                return false; 
            }
        }
        return true;
    }

    public virtual bool CanDrop (float distance)
    {
		/*
		foreach (BaseCube cube in cubes)
		{
			if (!cube.CanDrop())
			{
				return false;
			}
		}*/


		return true;
	}

    public void SeparateCubes()
    {
        foreach(BaseCube cube in cubes)
        {
            cube.LandCube();
		}
    }

    IEnumerator StartDropping()
    {
        while(CanDrop(1f))
        {
            transform.Translate(Vector3.down);
            yield return new WaitForSeconds(BlockDropManager.Instance.dropDelay);
        }
        SeparateCubes();
        //BlockDropManager.Instance.OnDropped?.Invoke();

		Destroy(gameObject);
	}
}
