using UnityEngine;

public class CubeCheckCollider : MonoBehaviour
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

		if (other.gameObject.layer == LayerMask.NameToLayer(BlockDropManager.INACTIVE_BLOCK))
		{
			BlockDropManager.Instance.OnDropped?.Invoke();
		}
		else if (other.gameObject.layer == LayerMask.NameToLayer(BlockDropManager.SHIFTED_BLOCK))
		{
			other.gameObject.layer = LayerMask.NameToLayer(BlockDropManager.INACTIVE_BLOCK);
		}
	}
}
