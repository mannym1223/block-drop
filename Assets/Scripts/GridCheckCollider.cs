using UnityEngine;

public class GridCheckCollider : MonoBehaviour
{
	public bool hasBlock;
	public BaseCube cube;

	private Collider checkCollider;

	public void ResetCollider()
	{
		checkCollider.enabled = true;
		hasBlock = false;
		cube = null;
	}

	private void Awake()
	{
		checkCollider = GetComponent<Collider>();
	}

	private void OnTriggerStay(Collider other)
	{
		hasBlock = true;
		cube = other.gameObject.GetComponent<BaseCube>();
		checkCollider.enabled = false;

		BlockDropManager.Instance.OnDropped?.Invoke();
	}
}
