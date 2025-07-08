using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] Transform cameraTransform;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] Transform groundControl;

    InputAction moveFwdAction;
    InputAction moveBwdAction;
    InputAction moveLeftAction;
    InputAction moveRightAction;
    InputAction turnLeftAction;
    InputAction turnRightAction;
    InputAction turnUpAction;
    InputAction turnDownAction;
    float amountForward = 0.0f;
    float amountBackward = 0.0f;
    float amountLeft = 0.0f;
    float amountRight = 0.0f;
    float amountTurnLeft = 0.0f;
    float amountTurnRight = 0.0f;
    float amountTurnUp = 0.0f;
    float amountTurnDown = 0.0f;
    CharacterController controller;
    float cameraEulerX = 0.0f;
    Vector3 potentialEnergy = Vector3.zero;
    AudioSource audioSource;
    Animator animator;
    bool isFalling = false;

    void Start()
    {
        moveFwdAction = InputSystem.actions.FindAction("MoveFWD");
        moveBwdAction = InputSystem.actions.FindAction("MoveBWD");
        moveLeftAction = InputSystem.actions.FindAction("MoveLWD");
        moveRightAction = InputSystem.actions.FindAction("MoveRWD");
        turnLeftAction = InputSystem.actions.FindAction("TurnLeft");
        turnRightAction = InputSystem.actions.FindAction("TurnRight");
        turnUpAction = InputSystem.actions.FindAction("TurnUp");
        turnDownAction = InputSystem.actions.FindAction("TurnDown");
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void PlayFootstepSound()
    {
        audioSource.clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.Play();
    }

    void ProcessInputs()
    {
        amountForward = moveFwdAction.IsPressed() ? 1.0f : 0.0f;
        amountBackward = moveBwdAction.IsPressed() ? 1.0f : 0.0f;
        amountLeft = moveLeftAction.IsPressed() ? 1.0f : 0.0f;
        amountRight = moveRightAction.IsPressed() ? 1.0f : 0.0f;
        amountTurnLeft = turnLeftAction.IsPressed() ? 1.0f : 0.0f;
        amountTurnRight = turnRightAction.IsPressed() ? 1.0f : 0.0f;
        amountTurnUp = turnUpAction.IsPressed() ? 1.0f : 0.0f;
        amountTurnDown = turnDownAction.IsPressed() ? 1.0f : 0.0f;
    }

    bool CheckGround()
    {
        return Physics.Raycast(groundControl.position, -groundControl.up, 0.5f);
    }

    void ProcessMovement()
    {
        var dir = Vector3.zero;
        dir += (amountForward - amountBackward) * transform.forward;
        dir += (amountRight - amountLeft) * transform.right;
        dir.Normalize();
        dir *= speed * Time.fixedDeltaTime;

        // Gravity
        if (controller.isGrounded)
        {
            potentialEnergy = Vector3.zero;
            animator.SetBool("IsWalking", dir.magnitude > 0.0f);
            isFalling = false;
        }
        else if (CheckGround() && !isFalling)
        {
            controller.Move(Vector3.down * 1000.0f);
            animator.SetBool("IsWalking", dir.magnitude > 0.0f);
        }
        else
        {
            potentialEnergy += 0.5f * Mathf.Pow(Time.fixedDeltaTime, 2.0f) * Physics.gravity;
            animator.SetBool("IsWalking", false);
            isFalling = true;
        }
        dir += potentialEnergy;

        controller.Move(dir);
    }

    void ProcessRotation()
    {
        var dir = new Vector3(0.0f, amountTurnRight - amountTurnLeft, 0.0f);
        dir *= rotationSpeed * Time.fixedDeltaTime;

        transform.Rotate(dir);
    }

    void ProcessCameraRotation()
    {
        var dir = (amountTurnDown - amountTurnUp) * rotationSpeed * Time.fixedDeltaTime;
        var cameraEulers = cameraTransform.localEulerAngles;
        cameraEulerX = Mathf.Clamp(cameraEulerX + dir, -80.0f, 80.0f);
        cameraTransform.localEulerAngles = new Vector3(cameraEulerX, cameraEulers.y, cameraEulers.z);
    }

    void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        ProcessMovement();
        ProcessRotation();
        ProcessCameraRotation();
    }
}
