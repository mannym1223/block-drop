
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int width;
    public int length;
    public int height;

    public List<BaseCube> cubes;

    /*
	private void Awake()
	{
		cubes = new List<BaseCube>();

        BaseCube[] children = GetComponentsInChildren<BaseCube>();
        foreach (BaseCube child in children)
        {
            cubes.Add(child);
        }
	}*/

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
}
