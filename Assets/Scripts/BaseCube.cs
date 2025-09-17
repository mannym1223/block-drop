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

	public void ShiftDown()
	{
		transform.Translate(Vector3.down);
	}

	public void LandCube()
	{
		if(transform.position.y > BlockDropManager.Instance.gameOverLimit.transform.position.y)
		{
			BlockDropManager.Instance.GameOver();
		}

		transform.SetParent(null, true);
		GetComponent<BoxCollider>().enabled = true;
		gameObject.layer = LayerMask.NameToLayer("InactiveBlock");
	}
}
