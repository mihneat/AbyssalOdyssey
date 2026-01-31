using System;
using System.Collections;
using Scripts.Player.VR;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Scripts.Submarine.VR
{
    public class SubmarineControllerVR : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float horizontalLookSpeed;
        [SerializeField] private float verticalLookSpeed;
        [SerializeField] [Min(0.0f)] private float lookSmoothing;
        
        [SerializeField] private CircularDrive wheelCircularDrive;
        
        private PlayerControllerVR currPlayerController;

        private Vector3 moveVec;
        private Vector2 lookVec;
        private bool shouldExit;

        private Rigidbody rb;

        private float prevWheelAngle;
        private Vector3 targetRotation;

        private Vector3 prevPosition;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            targetRotation = transform.localEulerAngles;
        }

        private void OnEnable()
        {
            SteamVR_Actions._default.SubmarineMove.AddOnChangeListener(HandleOnMove, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.SubmarineLook.AddOnChangeListener(HandleOnLook, SteamVR_Input_Sources.RightHand);
            SteamVR_Actions._default.SubmarineExit.AddOnChangeListener(HandleOnExit, SteamVR_Input_Sources.RightHand);
        }

        private void OnDisable()
        {
            SteamVR_Actions._default.SubmarineMove.RemoveOnChangeListener(HandleOnMove, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.SubmarineLook.RemoveOnChangeListener(HandleOnLook, SteamVR_Input_Sources.RightHand);
            SteamVR_Actions._default.SubmarineExit.RemoveOnChangeListener(HandleOnExit, SteamVR_Input_Sources.RightHand);
        }

        private void Update()
        {
            if (shouldExit)
            {
                ExitDrivingState();
                shouldExit = false;
            }
        }

        private void FixedUpdate()
        {
            if (currPlayerController == null)
                return;
            
            // Move the submarine
            Vector3 moveDir = transform.right * moveVec.x + transform.forward * moveVec.y;
            transform.Translate(moveDir.normalized * (moveSpeed * Time.fixedDeltaTime), Space.World);
            
            // Grab the wheel's angle
            float currWheelAngle = -wheelCircularDrive.outAngle;
            float deltaHorizontalRotation = currWheelAngle - prevWheelAngle;
            prevWheelAngle = currWheelAngle;
            
            // Rotate the submarine
            float verticalLook = -lookVec.y * verticalLookSpeed * Time.fixedDeltaTime;
            float horizontalLook = deltaHorizontalRotation * horizontalLookSpeed * Time.fixedDeltaTime;
            targetRotation += new Vector3(verticalLook, horizontalLook, 0);
            
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotation), lookSmoothing * Time.fixedDeltaTime);
        }

        public void StartDriving(PlayerControllerVR playerController, Vector3 prevPlayerLocalPosition)
        {
            currPlayerController = playerController;
            prevPosition = prevPlayerLocalPosition;
        }
        
        private void ExitDrivingState()
        {
            if (currPlayerController == null)
                return;
            
            StopDriving();
        }

        // Mirror of the function in SubmarineDriveInteractable
        private void StopDriving()
        {
            // Make the player not kinematic and detach them from the ship
            currPlayerController.GetComponent<Rigidbody>().isKinematic = false;
            currPlayerController.GetComponent<Collider>().isTrigger = false;
            
            // Teleport the player back
            currPlayerController.transform.localPosition = prevPosition;
            currPlayerController.transform.parent = null;
            
            // Might need to change the player's rotation after unparenting
            Vector3 prevPlayerRotation = currPlayerController.transform.localEulerAngles;
            currPlayerController.transform.localEulerAngles = new Vector3(0, prevPlayerRotation.y, 0);
            
            // Enable hand interactors
            HandInteractor[] handInteractors = FindObjectsByType<HandInteractor>(FindObjectsSortMode.None);
            foreach (HandInteractor handInteractor in handInteractors)
                handInteractor.ToggleComponent(true);
            
            // Unlock player input
            currPlayerController.ManuallyLockInput = false;
            
            currPlayerController = null;
        }

        private void HandleOnMove(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            moveVec = axis;
        }

        private void HandleOnLook(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            lookVec = axis;
        }

        private void HandleOnExit(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            shouldExit = newState;
        }
    }
}
