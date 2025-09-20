using System.Collections;
using UnityEngine;

public class FlashingText : MonoBehaviour
{
    MeshRenderer textRender;
    public bool activateOnStart;
	public float flashDelay;

	private bool isFlashing;
	private float flashTime = 0;

	private void Awake()
	{
		textRender = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		if (activateOnStart)
		{
			ActivateFlashing();
		}
	}

	private void Update()
	{
		if (isFlashing && flashTime > flashDelay)
		{
			textRender.enabled = !textRender.enabled;
			flashTime = 0f;
		}
		flashTime += Time.deltaTime;
	}

	public void ActivateFlashing()
	{
        isFlashing = true;
	}

	public void DeactivateFlashing()
	{
		isFlashing = false;
	}
}
