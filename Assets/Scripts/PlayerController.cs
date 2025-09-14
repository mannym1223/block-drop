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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		moveAction = InputSystem.actions.FindAction("Move");
	}

	private void FixedUpdate()
	{
		currentMoveValue = moveAction.ReadValue<Vector2>();

		if (currentMoveValue != null && !isMoving && activeBlock != null)
		{
            bool goForward = Physics.Raycast(activeBlock.transform.position, Vector3.forward * currentMoveValue.x, moveStep, LayerMask.GetMask("Spawn"));
			bool goRight = Physics.Raycast(activeBlock.transform.position, Vector3.right * currentMoveValue.y, moveStep, LayerMask.GetMask("Spawn"));

            if (goForward || goRight) 
            {
                StartCoroutine(StartMoving(goForward, goRight));
            }
			//Debug.Log(currentMoveValue);

            Debug.Log(goForward);
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
            Debug.Log(direction * moveSpeed * Time.deltaTime);
            stepDistance -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
		Debug.Log("Exiting move" + goForward);
		isMoving = false;
    }
}
