using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;

    InputAction moveFwdAction;
    InputAction moveBwdAction;
    InputAction moveLeftAction;
    InputAction moveRightAction;
    float amountForward = 0.0f;
    float amountBackward = 0.0f;
    float amountLeft = 0.0f;
    float amountRight = 0.0f;
    CharacterController controller;

    void Start()
    {
        moveFwdAction = InputSystem.actions.FindAction("MoveFWD");
        moveBwdAction = InputSystem.actions.FindAction("MoveBWD");
        moveLeftAction = InputSystem.actions.FindAction("MoveLWD");
        moveRightAction = InputSystem.actions.FindAction("MoveRWD");
        controller = GetComponent<CharacterController>();
    }

    void ProcessInputs()
    {
        amountForward = moveFwdAction.IsPressed() ? 1.0f : 0.0f;
        amountBackward = moveBwdAction.IsPressed() ? 1.0f : 0.0f;
        amountLeft = moveLeftAction.IsPressed() ? 1.0f : 0.0f;
        amountRight = moveRightAction.IsPressed() ? 1.0f : 0.0f;
    }

    void ProcessMovement()
    {
        var dir = new Vector3(amountRight - amountLeft, 0.0f, amountForward - amountBackward);
        dir.Normalize();
        dir *= speed * Time.deltaTime;

        controller.Move(dir);
    }

    void Update()
    {
        ProcessInputs();
        ProcessMovement();
    }
}
