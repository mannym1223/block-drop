using System.Collections;
using UnityEngine;

public class FlashingText : MonoBehaviour
{
    MeshRenderer textRender;
    public bool activateOnStart;
	public float flashDelay;

	private bool isFlashing;

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

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void ActivateFlashing()
	{
        if (isFlashing)
        {
			return;
        }
        StartCoroutine(StartFlashing());
	}

	private IEnumerator StartFlashing()
	{
		isFlashing = true;

		while (true)
		{
			textRender.enabled = !textRender.enabled;
			yield return new WaitForSeconds(flashDelay);
		}
	}
}
