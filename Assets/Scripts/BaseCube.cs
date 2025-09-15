using Unity.VisualScripting;
using UnityEngine;

public class BaseCube : MonoBehaviour
{
    public bool CanMove(Vector3 direction, float distance)
    {
		return Physics.Raycast(transform.position, direction, distance, LayerMask.GetMask("Spawn"));
	}

	public bool CanDrop(Vector3 direction, float distance)
	{
		return !Physics.Raycast(transform.position, direction, distance, LayerMask.GetMask("InactiveBlock", "Platform"));
	}
}
