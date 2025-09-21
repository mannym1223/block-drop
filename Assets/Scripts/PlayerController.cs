using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform rope;
    [HideInInspector]
    public Block activeBlock;
    public float moveStep = 1f;
    public float moveSpeed = 1.0f;
    [HideInInspector]
    public Camera mainCam;
    public Transform joystick;

    InputAction moveAction;
    private Vector2 currentMoveValue;
    private bool isMoving = false;

    InputAction dropAction;
    private float currentDropValue;
    private bool isDropping = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = FindFirstObjectByType<Camera>();
		moveAction = InputSystem.actions.FindAction("Move");
        dropAction = InputSystem.actions.FindAction("Drop");
        BlockDropManager.Instance.OnDropped.AddListener(() => isDropping = false);
	}

	private void FixedUpdate()
	{
		currentMoveValue = moveAction.ReadValue<Vector2>();
        currentDropValue = dropAction.ReadValue<float>();
		if (currentMoveValue != null && !isMoving && currentMoveValue.magnitude != 0f)
		{
			bool goForward;
            bool goRight;

			Vector3 mainCamForward = mainCam.transform.forward * currentMoveValue.y;
            mainCamForward.y = 0f;
            mainCamForward.Normalize();
			Vector3 mainCamRight = mainCam.transform.right * currentMoveValue.x;

			if (activeBlock == null) // move freely while no active block
			{
				goForward = Physics.Raycast(transform.position, mainCamForward, moveStep, LayerMask.GetMask("Spawn"));
                goRight = Physics.Raycast(transform.position, mainCamRight, moveStep, LayerMask.GetMask("Spawn"));
			}
            else
            {
                goForward = activeBlock.CanMove(mainCamForward, moveStep);
				goRight = activeBlock.CanMove(mainCamRight, moveStep);
			}
            if (goForward || goRight) 
            {
                Vector3 direction = Vector3.zero;
                if (goForward) 
                {
                    direction += mainCamForward;
                }
                if (goRight)
                {
                    direction += mainCamRight;
                }
                direction.Normalize();
				joystick.localRotation = Quaternion.Euler(currentMoveValue.y * -35, 0f, currentMoveValue.x * -35);
				StartCoroutine(StartMoving(direction));
            }
		}

        if (currentDropValue > 0f && !isMoving && activeBlock != null && !isDropping)
        {
            activeBlock.transform.SetParent(null, true);
            activeBlock.Drop();
            activeBlock = null;
            isDropping = true;
        }
	}

	IEnumerator StartMoving(Vector3 direction)
    {
        isMoving = true;

        float stepDistance = 0f;

        if(direction.z > 0f && direction.x > 0f)
        {
            stepDistance = Mathf.Sqrt(moveStep * 2); // get value of diagonal
        }
        else
        {
            stepDistance = moveStep;
        }

        Vector3 newPosition = transform.position + (direction * moveStep);
        newPosition.x = Mathf.Round(newPosition.x);
        newPosition.z = Mathf.Round(newPosition.z);
        while (Vector3.Distance(newPosition, transform.position) > 0.05f)
        {
            // transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
            transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed * Time.deltaTime);
            //stepDistance -= Time.deltaTime * moveSpeed;
            yield return new WaitForFixedUpdate();
        }
		joystick.localRotation = Quaternion.Euler(0f, 0f, 0f);
		isMoving = false;
    }

    IEnumerator StartMovingJoystick()
    {
        Quaternion newRotation = Quaternion.Euler(currentMoveValue.y * 35, 0f, 0f);
        while (Quaternion.Angle(joystick.rotation, newRotation) > 0.05f)
        {
            joystick.Rotate(Vector3.forward * 35 * currentMoveValue.y, 0f, 0f);
            yield return new WaitForFixedUpdate();
        }
	}
}
