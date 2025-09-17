using System.Collections;
using Unity.VisualScripting;
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
        BlockDropManager.Instance.OnDropped.AddListener(() => isDropping = false);
	}

	private void FixedUpdate()
	{
		currentMoveValue = moveAction.ReadValue<Vector2>();
        currentDropValue = dropAction.ReadValue<float>();
		if (currentMoveValue != null && !isMoving)
		{
			bool goForward;
            bool goRight;
			// move freely while no active block
			if (activeBlock == null)
            {
                goForward = Physics.Raycast(transform.position, Vector3.forward * currentMoveValue.x, moveStep, LayerMask.GetMask("Spawn"));
                goRight = Physics.Raycast(transform.position, Vector3.right * currentMoveValue.y, moveStep, LayerMask.GetMask("Spawn"));
                Debug.Log("Moving freely");
			}
            else
            {
                goForward = activeBlock.CanMove(Vector3.forward * currentMoveValue.x, moveStep);
                goRight = activeBlock.CanMove(Vector3.right * currentMoveValue.y, moveStep);
            }
            if (goForward || goRight) 
            {
                StartCoroutine(StartMoving(goForward, goRight));
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
            stepDistance -= Time.deltaTime * moveSpeed;
            yield return new WaitForFixedUpdate();
        }
		isMoving = false;
    }
}
