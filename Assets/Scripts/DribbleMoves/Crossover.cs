using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Dribbling))]
public class Crossover : MonoBehaviour
{
    [Header("Hand Positions")] 
    [SerializeField] private Transform leftHandPosition;
    [SerializeField] private Transform rightHandPosition;

    [Header("Crossover Settings")] 
    [SerializeField] private float crossoverSpeed = 5f;
    [SerializeField] private float crossoverArcHeight = 0.5f;
    [SerializeField] private float stickFlickThreshold = 0.7f;

    private int currentHand = 1; //-1 = left, 1 = right
    
    private Dribbling dribbling;
    private float crossoverProgress = 0f;
    private Vector3 crossoverStartPos;
    private Vector3 crossoverEndPos;
    private bool stickWasNeutral = true;
    private Vector2 rightStickInput;
    private InputSystem_Actions _inputActions;
    
    public bool isCrossingOver = false;

    void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }
    
    void Start()
    {
        dribbling = GetComponent<Dribbling>();
    }
    
    void Update()
    {
        HandleCrossoverInput();

        if (isCrossingOver)
        {
            UpdateCrossover();
        }
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Player.RightStick.performed += OnRightStick;
        _inputActions.Player.RightStick.canceled += OnRightStick;
    }

    private void OnDisable()
    {
        _inputActions.Player.RightStick.performed -= OnRightStick;
        _inputActions.Player.RightStick.canceled -= OnRightStick;
        _inputActions.Disable();
    }
    
    private void OnRightStick(InputAction.CallbackContext context)
    {
        rightStickInput = context.ReadValue<Vector2>();
    }
    
    private void HandleCrossoverInput()
    {
        if (!dribbling.isDribbling || isCrossingOver) return;

        float stickX = rightStickInput.x;

        if (stickWasNeutral)
        {
            if (stickX > stickFlickThreshold && currentHand == -1)
            {
                StartCrossover(targetHand: 1);
                stickWasNeutral = false;
            }
            else if (stickX < -stickFlickThreshold && currentHand == 1)
            {
                StartCrossover(targetHand: -1);
                stickWasNeutral = false;
            }
        }

        if (Mathf.Abs(stickX) < 0.2f)
        {
            stickWasNeutral = true;
        }
    }
    
    private void StartCrossover(int targetHand)
    {
        isCrossingOver = true;
        crossoverProgress = 0f;
        crossoverStartPos = dribbling.ballRB.position;

        Transform targetTransform = targetHand == 1 ? rightHandPosition : leftHandPosition;
        crossoverEndPos = new Vector3(
            targetTransform.position.x,
            dribbling.ballRB.position.y,
            targetTransform.position.z
        );

        currentHand = targetHand;
    }
    
    private void UpdateCrossover()
    {
        crossoverProgress += Time.deltaTime * crossoverSpeed;
        float t = Mathf.Clamp01(crossoverProgress);

        Vector3 flatPos = Vector3.Lerp(crossoverStartPos, crossoverEndPos, t);
        float arc = Mathf.Sin(t * Mathf.PI) * crossoverArcHeight;
        Vector3 arcedPos = new Vector3(flatPos.x, flatPos.y + arc, flatPos.z);

        dribbling.ballRB.isKinematic = true;
        dribbling.ballRB.MovePosition(arcedPos);

        if (t >= 1f)
        {
            isCrossingOver = false;
            dribbling.ballRB.isKinematic = false;
            dribbling.ResumeKeepBallToPlayer();
            dribbling.ApplyDribbleForce();
        }
    }
    
}
