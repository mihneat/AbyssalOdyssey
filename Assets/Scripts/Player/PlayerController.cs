using System;
using Scripts.Harpoon;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public CinemachineCamera cam;
        public PlayerInput playerInput;
        public PlayerEnvironmentDetector environmentDetector;
        [SerializeField] private HarpoonLauncherController harpoonLauncherController;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float lookSpeed;
        
        public event Action OnInteract;
        public event Action OnExitDrivingState;
        public event Action<Vector3> OnMoveSubmarine;
        public event Action<Vector2> OnRotateSubmarine;

        private Vector2 moveVec;
        private Vector2 lookVec;
        private float swimVec;

        private bool isSprinting;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            environmentDetector.OnEnvironmentChanged += HandleOnEnvironmentChanged;
        }

        private void OnDisable()
        {
            environmentDetector.OnEnvironmentChanged -= HandleOnEnvironmentChanged;
        }

        private void HandleOnEnvironmentChanged(Environment newEnvironment)
        {
            switch (newEnvironment)
            {
                case Environment.Air:
                    rb.linearDamping = 0.0f;
                    rb.useGravity = true;
                    break;
                
                case Environment.Underwater:
                    rb.linearDamping = 10.0f;
                    rb.useGravity = false;
                    break;
            }
        }

        private void FixedUpdate()
        {
            // Move the player
            float speed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 moveDir = transform.forward * moveVec.y + transform.right * moveVec.x;
            if (environmentDetector.Environment == Environment.Underwater)
                moveDir += transform.up * swimVec;
            
            transform.Translate(moveDir.normalized * (speed * Time.fixedDeltaTime), Space.World);
            
            // Rotate the camera
            transform.localEulerAngles += new Vector3(0, lookVec.x, 0) * (lookSpeed * Time.fixedDeltaTime);
            Vector3 currCamRotation = cam.transform.localEulerAngles;
            currCamRotation += new Vector3(-lookVec.y, 0, 0) * (lookSpeed * Time.fixedDeltaTime);

            if (currCamRotation.x > 89.0f && currCamRotation.x < 180.0f)
                currCamRotation.x = 89.0f;
            if (currCamRotation.x > 180.0f && currCamRotation.x < 271.0f)
                currCamRotation.x = 271.0f;

            cam.transform.localEulerAngles = currCamRotation;
        }

        public void HandleOnMove(InputAction.CallbackContext ctx)
        {
            moveVec = ctx.ReadValue<Vector2>();
        }
        
        public void HandleOnLook(InputAction.CallbackContext ctx)
        {
            lookVec = ctx.ReadValue<Vector2>();
        }
        
        public void HandleOnSwimVertical(InputAction.CallbackContext ctx)
        {
            swimVec = ctx.ReadValue<float>();
        }
        
        public void HandleOnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton() == false)
                return;
            
            OnInteract?.Invoke();
        }
        
        public void HandleOnSprint(InputAction.CallbackContext ctx)
        {
            isSprinting = ctx.ReadValueAsButton();
        }
        
        public void HandleOnHarpoonLaunch(InputAction.CallbackContext ctx)
        {
            harpoonLauncherController.HandleHarpoonLaunchButton(ctx.ReadValueAsButton());
        }
        
        public void HandleOnHarpoonReset(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton() == false)
                return;
            
            harpoonLauncherController.HandleResetButtonClicked();
        }
        
        public void HandleOnExitDrivingState(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton() == false)
                return;
            
            OnExitDrivingState?.Invoke();
        }
        
        public void HandleOnMoveSubmarine(InputAction.CallbackContext ctx)
        {
            OnMoveSubmarine?.Invoke(ctx.ReadValue<Vector3>());
        }
        
        public void HandleOnRotateSubmarine(InputAction.CallbackContext ctx)
        {
            OnRotateSubmarine?.Invoke(ctx.ReadValue<Vector2>());
        }
    }
}
