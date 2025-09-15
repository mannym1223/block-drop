using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform rope;
    public Block activeBlock;
    public float moveStep = 1f;
    public float moveSpeed = 1.0f;

    InputAction moveAction;
    private Vector2 currentMoveValue;
    private bool isMoving = false;

    InputAction dropAction;
    private float currentDropValue;
    private bool isDropping = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		moveAction = InputSystem.actions.FindAction("Move");
        dropAction = InputSystem.actions.FindAction("Drop");
	}

	private void FixedUpdate()
	{
		currentMoveValue = moveAction.ReadValue<Vector2>();
        currentDropValue = dropAction.ReadValue<float>();
        Debug.Log(currentDropValue);
		if (currentMoveValue != null && !isMoving && activeBlock != null)
		{
            bool goForward = activeBlock.CanMove(Vector3.forward * currentMoveValue.x, moveStep);
            bool goRight = activeBlock.CanMove(Vector3.right * currentMoveValue.y, moveStep);

            if (goForward || goRight) 
            {
                StartCoroutine(StartMoving(goForward, goRight));
            }
			//Debug.Log(currentMoveValue);

            //Debug.Log(goForward);
		}

        if (currentDropValue > 0f && !isMoving && activeBlock != null && !isDropping)
        {
            activeBlock.transform.SetParent(null, true);
            activeBlock.Drop();
            isDropping = true;
            Debug.Log("Call drop on block");
        }
	}

	IEnumerator StartMoving(bool goForward, bool goRight)
    {
        isMoving = true;

        Vector3 direction = Vector3.zero;
        float stepDistance = 0f;

        if (goForward)
        {
            direction.z += currentMoveValue.x;
        }
        if (goRight)
        {
            direction.x += currentMoveValue.y;
		}
        direction.Normalize();

        if(goForward && goRight)
        {
            stepDistance = Mathf.Sqrt(moveStep * 2); // get value of diagonal
        }
        else
        {
            stepDistance = moveStep;
        }

        while (stepDistance > 0f)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            stepDistance -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
		isMoving = false;
    }
}
