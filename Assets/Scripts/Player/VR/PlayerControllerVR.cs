using System;
using System.Numerics;
using UnityEngine;
using Valve.VR;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Scripts.Player.VR
{
    public class PlayerControllerVR : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private PlayerEnvironmentDetector environmentDetector;
        
        [Tooltip("Only used to be activated after the player timeline ends")]
        [SerializeField] private Collider bodySubmergedCollider;
        
        [SerializeField] private float groundMoveSpeed = 3.0f;
        [SerializeField] private float waterMoveSpeed = 2.0f;
        [SerializeField] private float jumpOutOfWaterMagnitude = 10.0f;

        private Rigidbody rb;
        
        private Vector2 moveVector;
        private float swimValue;
        private bool shouldJumpNextFrame;
        
        public bool ManuallyLockInput { get => manuallyLockInput; set => manuallyLockInput = value; }
        private bool manuallyLockInput;

        private Environment currEnvironment = Environment.Underwater;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            environmentDetector.OnEnvironmentChanged += HandleOnEnvironmentChanged;
            
            SteamVR_Actions._default.Move.AddOnChangeListener(HandleOnMove, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.SwimVertical.AddOnChangeListener(HandleOnSwimVertical, SteamVR_Input_Sources.RightHand);
            SteamVR_Actions._default.Jump.AddOnChangeListener(HandleOnJump, SteamVR_Input_Sources.RightHand);
        }

        private void OnDisable()
        {
            environmentDetector.OnEnvironmentChanged -= HandleOnEnvironmentChanged;
            
            SteamVR_Actions._default.Move.RemoveOnChangeListener(HandleOnMove, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.SwimVertical.RemoveOnChangeListener(HandleOnSwimVertical, SteamVR_Input_Sources.RightHand);
            SteamVR_Actions._default.Jump.RemoveOnChangeListener(HandleOnJump, SteamVR_Input_Sources.RightHand);
        }

        private void Update()
        {
            PlayerJump();
        }

        private void FixedUpdate()
        {
            PlayerMove();
        }

        private void PlayerMove()
        {
            if (manuallyLockInput)
                return;

            Vector3 finalMoveVector = Vector3.zero;

            // Move relative to the camera
            Vector3 camRight = Vector3.ProjectOnPlane(transform.InverseTransformDirection(playerCamera.transform.right), Vector3.up);
            if (camRight.magnitude > 0.01f)
                camRight.Normalize();
            
            Vector3 camForward = transform.InverseTransformDirection(playerCamera.transform.forward);
            if (currEnvironment == Environment.Air)
            {
                camForward = Vector3.ProjectOnPlane(camForward, Vector3.up);
                if (camForward.magnitude > 0.01f)
                    camForward.Normalize();
            }

            Vector3 moveVector = this.moveVector.x * camRight + this.moveVector.y * camForward;
            finalMoveVector += moveVector;
            
            // Add the swim
            if (currEnvironment != Environment.Air)
            {
                Vector3 swimVector = swimValue * Vector3.up;
                finalMoveVector += swimVector;
            }
            
            // Clamp the vector to a length of 1
            if (finalMoveVector.sqrMagnitude > 1.0f)
                finalMoveVector.Normalize();
            
            // Complete the translation
            float moveSpeed = currEnvironment == Environment.Air ? groundMoveSpeed : waterMoveSpeed;
                transform.Translate(finalMoveVector * moveSpeed * 0.02f);
        }

        private void PlayerJump()
        {
            if (manuallyLockInput)
                return;
            
            if (!shouldJumpNextFrame)
                return;

            shouldJumpNextFrame = false;

            Vector3 linearVelocity = rb.linearVelocity;
            linearVelocity.y = jumpOutOfWaterMagnitude;
            rb.linearVelocity = linearVelocity;
        }

        private void HandleOnEnvironmentChanged(Environment newEnvironment)
        {
            switch (newEnvironment)
            {
                case Environment.Air:
                    rb.linearDamping = 0.0f;
                    rb.useGravity = true;
                    break;
                
                case Environment.HalfUnderwater:
                case Environment.Underwater:
                    rb.linearDamping = 10.0f;
                    rb.useGravity = false;
                    break;
            }

            currEnvironment = newEnvironment;
        }
        
        // Used by animation events
        public void LockInput()
        {
            if (!Application.isPlaying)
                return;

            manuallyLockInput = true;
        }

        // Used by animation events
        public void UnlockInput()
        {
            if (!Application.isPlaying)
                return;

            manuallyLockInput = false;
            
            // Hack: Reactivate body submerged collider
            bodySubmergedCollider.enabled = true;
        }

        private void HandleOnMove(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            moveVector = axis;
        }

        private void HandleOnSwimVertical(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            swimValue = axis.y;
        }

        private void HandleOnJump(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool shouldJump)
        {
            if (!shouldJump)
                return;
            
            if (currEnvironment != Environment.HalfUnderwater)
                return;

            shouldJumpNextFrame = true;
        }
    }
}
