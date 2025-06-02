using System;
using System.Collections;
using Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Submarine
{
    public class SubmarineController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float lookSpeed;
        
        private PlayerController currPlayerController;

        private Vector3 moveVec;
        private Vector2 lookVec;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // Move the submarine
            Vector3 moveDir = transform.forward * moveVec.z + transform.right * moveVec.x + transform.up * moveVec.y;
            transform.Translate(moveDir.normalized * (moveSpeed * Time.fixedDeltaTime), Space.World);
            
            // Rotate the submarine
            Vector3 currRotation = transform.localEulerAngles;
            currRotation += new Vector3(-lookVec.y, lookVec.x, 0) * (lookSpeed * Time.fixedDeltaTime);

            if (currRotation.x > 89.0f && currRotation.x < 180.0f)
                currRotation.x = 89.0f;
            if (currRotation.x > 180.0f && currRotation.x < 271.0f)
                currRotation.x = 271.0f;

            transform.localEulerAngles = currRotation;
        }

        public void StartDriving(PlayerController playerController)
        {
            currPlayerController = playerController;

            StartCoroutine(SubscribeInputEvents());
        }

        private IEnumerator SubscribeInputEvents()
        {
            yield return null;
            
            // Bind the input actions
            currPlayerController.OnExitDrivingState += ExitDrivingState;
            currPlayerController.OnMoveSubmarine += HandleOnMoveSubmarine;
            currPlayerController.OnRotateSubmarine += HandleOnRotateSubmarine;
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
            currPlayerController.transform.parent = null;
            
            // Might need to change the player's rotation after unparenting
            Vector3 prevPlayerRotation = currPlayerController.transform.localEulerAngles;
            currPlayerController.transform.localEulerAngles = new Vector3(0, prevPlayerRotation.y, 0);
            
            // Reenable player interactor
            currPlayerController.GetComponent<PlayerInteractor>().ToggleComponent(true);
            
            // Reenable the player camera
            currPlayerController.cam.enabled = true;
            
            // Switch input from Submarine to Player
            currPlayerController.playerInput.SwitchCurrentActionMap("Player");
            
            // Unbind all actions
            currPlayerController.OnExitDrivingState -= ExitDrivingState;
            currPlayerController.OnMoveSubmarine -= HandleOnMoveSubmarine;
            currPlayerController.OnRotateSubmarine -= HandleOnRotateSubmarine;
            
            currPlayerController = null;
        }

        private void HandleOnMoveSubmarine(Vector3 moveDir)
        {
            moveVec = moveDir;
        }

        private void HandleOnRotateSubmarine(Vector2 rotateDir)
        {
            lookVec = rotateDir;
        }
    }
}
