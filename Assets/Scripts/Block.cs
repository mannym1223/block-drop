
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    public int width;
    public int length;
    public int height;

    public List<BaseCube> cubes;

    public virtual void Drop()
    {
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

    public virtual bool CanDrop (Vector3 direction, float distance)
    {
		foreach (BaseCube cube in cubes)
		{
			if (!cube.CanDrop(direction, distance))
			{
				return false;
			}
		}
		return true;
	}

    protected void SeparateCubes()
    {
        foreach(BaseCube cube in cubes)
        {
			cube.transform.SetParent(null, true);
            cube.GetComponent<BoxCollider>().enabled = true;
			cube.gameObject.layer = LayerMask.NameToLayer("InactiveBlock");
		}

        Destroy(gameObject);
    }

    IEnumerator StartDropping()
    {
        while(CanDrop(Vector3.down, 1f))
        {
            transform.Translate(Vector3.down);
            yield return new WaitForSeconds(BlockDropManager.Instance.dropDelay);
        }
        SeparateCubes();
        BlockDropManager.Instance.OnDropped?.Invoke();
	}
}
