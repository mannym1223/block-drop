using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	public float rotateSpeed = 20f;

    InputAction previousCamAction;
    InputAction nextCamAction;

	bool previousCamValue;
	bool nextCamValue;
	bool isRotating;

	float[] rotations = new float[4];
	int currentRotation = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		previousCamAction = InputSystem.actions.FindAction("CameraPrevious");
		nextCamAction = InputSystem.actions.FindAction("CameraNext");
		for (int index = 0; index < rotations.Length; index++)
		{
			rotations[index] = transform.rotation.y + index * 90;
		}
	}

	private void Update()
	{
		if (previousCamAction != null)
		{
			previousCamValue = previousCamAction.ReadValue<float>() > 0f;
		}
		if (nextCamAction != null)
		{
			nextCamValue = nextCamAction.ReadValue<float>() > 0f;
		}
	}

	private void LateUpdate()
	{
        if (previousCamValue && !isRotating)
		{
			StartCoroutine(RotateCamera(1));
		}
		else if (nextCamValue && !isRotating)
		{
			StartCoroutine(RotateCamera(-1));
		}
	}

	IEnumerator RotateCamera(int direction)
	{
		int nextRotation;
		if (direction < 0) nextRotation = rotations.Length - 1;
		else
		{
			nextRotation = (currentRotation + direction) % rotations.Length;
		}
		isRotating = true;
		Debug.Log("Rotating" + direction);
		float distanceRotated = 0f;
		while (Mathf.Abs(distanceRotated) < 90f)
		{
			float delta = rotateSpeed * Time.deltaTime * direction * 10;
			distanceRotated += delta;
			transform.Rotate(0f, delta, 0f);
			yield return new WaitForEndOfFrame();
		}
		isRotating = false;
	}
}
