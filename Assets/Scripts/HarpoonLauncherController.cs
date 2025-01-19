using System;
using NaughtyAttributes;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Scripts
{
    public enum HarpoonState
    {
        Loaded,
        Shooting,
        Stationary,
        Retracting
    }
    
    public class HarpoonLauncherController : MonoBehaviour
    {
        [BoxGroup("Harpoon References")]
        [SerializeField] private Transform harpoonSpot;
        [BoxGroup("Harpoon References")]
        [SerializeField] private HarpoonController harpoonController;
        [BoxGroup("Harpoon References")]
        [SerializeField] private HarpoonLauncherTriggerController triggerController;
        
        [BoxGroup("Harpoon Config")]
        [SerializeField] private float shootForce = 3.0f;
        [BoxGroup("Harpoon Config")]
        [SerializeField] private float retractSpeed = 15.0f;
        [BoxGroup("Harpoon Config")]
        [SerializeField] private float stopFactor = 10.0f;
        
        private Interactable interactable;

        private Hand harpoonLauncherHand;

        private Transform harpoon;
        private Rigidbody harpoonRb;
        private HarpoonState harpoonState = HarpoonState.Loaded;

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            
            harpoon = harpoonController.transform;
            harpoonRb = harpoon.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            interactable.onAttachedToHand += HandleOnAttachedToHand;
            interactable.onDetachedFromHand += HandleOnDetachedFromHand;
            
            SteamVR_Actions._default.ShootHarpoon.AddOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.ShootHarpoon.AddOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.RightHand);
            
            SteamVR_Actions._default.HarpoonTriggerAmount.AddOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.HarpoonTriggerAmount.AddOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.RightHand);
        }

        private void OnDisable()
        {
            interactable.onAttachedToHand -= HandleOnAttachedToHand;
            interactable.onDetachedFromHand -= HandleOnDetachedFromHand;
            
            SteamVR_Actions._default.ShootHarpoon.RemoveOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.ShootHarpoon.RemoveOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.RightHand);
            
            SteamVR_Actions._default.HarpoonTriggerAmount.RemoveOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.HarpoonTriggerAmount.RemoveOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.RightHand);
        }

        private void FixedUpdate()
        {
            if (harpoonState == HarpoonState.Retracting)
            {
                // Translate the harpoon towards the launcher
                harpoonRb.linearVelocity = (harpoonSpot.position - harpoon.transform.position).normalized * retractSpeed;
                
                // Snap the harpoon when it's close
                if (Vector3.Distance(harpoonSpot.position, harpoon.transform.position) < 0.15f)
                {
                    AttachHarpoonToLauncher();
                    harpoonState = HarpoonState.Loaded;
                }
            }
        }

        private void HandleOnAttachedToHand(Hand hand)
        {
            Debug.Log($"[HarpoonLauncherController] Harpoon launcher attached to hand: {hand.handType}");

            harpoonLauncherHand = hand;
        }

        private void HandleOnDetachedFromHand(Hand hand)
        {
            Debug.Log($"[HarpoonLauncherController] Harpoon launcher detached from hand: {hand.handType}");

            if (harpoonLauncherHand != null)
                HandleTriggerButtonReleased();
            
            harpoonLauncherHand = null;
            
            // Reset the trigger rotation
            triggerController.SetInterpolationValue(0.0f);
        }

        private void HandleOnShootHarpoon(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool isClicked)
        {
            if (harpoonLauncherHand == null)
                return;

            if (harpoonLauncherHand.handType != fromSource)
                return;

            if (isClicked)
                HandleTriggerButtonClicked();
            else
                HandleTriggerButtonReleased();
        }

        private void HandleHarpoonTriggerAmount(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float triggerAmount, float triggerAmountDelta)
        {
            if (harpoonLauncherHand == null)
                return;

            if (harpoonLauncherHand.handType != fromSource)
                return;

            Debug.Log($"[HarpoonLauncherController] Trigger amount changed to: {triggerAmount}");
            
            triggerController.SetInterpolationValue(triggerAmount);
        }

        private void HandleTriggerButtonClicked()
        {
            // Use haptics to indicate the action is ongoing
            SteamVR_Actions.default_Haptic[harpoonLauncherHand.handType].Execute(0, 5.0f, 4, 0.9f);

            switch (harpoonState)
            {
                case HarpoonState.Loaded:
                    ShootHarpoon();
                    break;
                
                case HarpoonState.Stationary:
                    RetractHarpoon();
                    break;
                
                case HarpoonState.Shooting:
                case HarpoonState.Retracting:
                default:
                    break;
            }
        }

        private void HandleTriggerButtonReleased()
        {
            // Stop the haptics
            SteamVR_Actions.default_Haptic[harpoonLauncherHand.handType].Execute(0, 0.1f, 0, 0);
            
            switch (harpoonState)
            {
                case HarpoonState.Shooting:
                case HarpoonState.Retracting:
                    StopHarpoon();
                    break;
                
                case HarpoonState.Loaded:
                case HarpoonState.Stationary:
                default:
                    break;
            }
        }

        private void ShootHarpoon()
        {
            // Add velocity to harpoon
            DetachHarpoonFromLauncher();
            harpoonRb.AddForce(-harpoon.forward * 1000.0f * shootForce);

            harpoonState = HarpoonState.Shooting;
        }

        private void StopHarpoon()
        {
            // Reduce the velocity of the harpoon
            harpoonRb.linearVelocity /= stopFactor;

            harpoonState = HarpoonState.Stationary;
        }

        private void RetractHarpoon()
        {
            harpoonState = HarpoonState.Retracting;
        }

        private void AttachHarpoonToLauncher()
        {
            harpoon.parent = harpoonSpot;
            
            harpoonRb.isKinematic = true;
            harpoon.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        private void DetachHarpoonFromLauncher()
        {
            harpoonRb.isKinematic = false;
            harpoon.parent = null;
        }
    }
}
