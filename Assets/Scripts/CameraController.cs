using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	public float rotateSpeed = 10f;

    InputAction previousCamAction;
    InputAction nextCamAction;

	bool previousCamValue;
	bool nextCamValue;
	bool isRotating;

	float[] yawRotations = new float[4];
	int currentYaw = 0;

	MeshRenderer glass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		previousCamAction = InputSystem.actions.FindAction("CameraPrevious");
		nextCamAction = InputSystem.actions.FindAction("CameraNext");
		for (int index = 0; index < yawRotations.Length; index++)
		{
			yawRotations[index] = transform.rotation.y + index * 90;
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

	private void FixedUpdate()
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
		int nextYaw = FindNextYawIndex(direction);

		isRotating = true;
		Vector3 currentRotation = transform.rotation.eulerAngles;
		Quaternion newRotation = Quaternion.Euler(currentRotation.x, yawRotations[nextYaw], currentRotation.z);

		while (Quaternion.Angle(transform.rotation, newRotation) > 1f)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
			currentRotation = transform.rotation.eulerAngles;
			yield return new WaitForEndOfFrame();
		}

		currentYaw = nextYaw;
		isRotating = false;
	}

	private int FindNextYawIndex(int direction)
	{
		int nextYaw = currentYaw;
		if (nextYaw + direction < 0) nextYaw = yawRotations.Length - 1;
		else
		{
			nextYaw = (currentYaw + direction) % yawRotations.Length;
		}

		return nextYaw;
	}
}
