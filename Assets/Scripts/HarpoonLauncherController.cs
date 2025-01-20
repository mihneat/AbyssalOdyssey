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
        
        [BoxGroup("Resistance Meter")]
        [SerializeField] private ResistanceMeter resistanceMeter;
        [BoxGroup("Resistance Meter")]
        [SerializeField] private float tensionIncreaseRate = 0.2f;
        [BoxGroup("Resistance Meter")]
        [SerializeField] private float tensionDecayRate = 0.4f;

        public Action<Harpoonable> OnSurfaceHit;
        public Action<bool> OnHarpoonReset;
        
        private Interactable interactable;

        private Hand harpoonLauncherHand;

        private Transform harpoon;
        private Rigidbody harpoonRb;
        private BoxCollider harpoonCollider;
        private HarpoonState harpoonState = HarpoonState.Loaded;

        private Harpoonable stuckObject;
        private Rigidbody stuckObjectRb;

        private float tension;

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            
            harpoon = harpoonController.transform;
            harpoonRb = harpoon.GetComponent<Rigidbody>();
            harpoonCollider = harpoon.GetComponent<BoxCollider>();

            harpoonCollider.enabled = false;

            harpoonController.OnHitSurface += HandleOnHitSurface;
        }

        private void OnEnable()
        {
            interactable.onAttachedToHand += HandleOnAttachedToHand;
            interactable.onDetachedFromHand += HandleOnDetachedFromHand;
            
            SteamVR_Actions._default.ShootHarpoon.AddOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.ShootHarpoon.AddOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.RightHand);
            
            SteamVR_Actions._default.HarpoonTriggerAmount.AddOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.HarpoonTriggerAmount.AddOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.RightHand);
            
            SteamVR_Actions._default.ResetHarpoon.AddOnChangeListener(HandleOnResetHarpoon, SteamVR_Input_Sources.Any);
        }

        private void OnDisable()
        {
            interactable.onAttachedToHand -= HandleOnAttachedToHand;
            interactable.onDetachedFromHand -= HandleOnDetachedFromHand;
            
            SteamVR_Actions._default.ShootHarpoon.RemoveOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.ShootHarpoon.RemoveOnChangeListener(HandleOnShootHarpoon, SteamVR_Input_Sources.RightHand);
            
            SteamVR_Actions._default.HarpoonTriggerAmount.RemoveOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.HarpoonTriggerAmount.RemoveOnChangeListener(HandleHarpoonTriggerAmount, SteamVR_Input_Sources.RightHand);
            
            SteamVR_Actions._default.ResetHarpoon.RemoveOnChangeListener(HandleOnResetHarpoon, SteamVR_Input_Sources.Any);
        }

        private void Update()
        {
            resistanceMeter.UpdateTension(tension);
        }

        private void FixedUpdate()
        {
            switch (harpoonState)
            {
                case HarpoonState.Retracting:
                    if (stuckObject == null)
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
                    else
                    {
                        // Translate the harpoon towards the launcher
                        stuckObjectRb.linearVelocity = (harpoonSpot.position - harpoon.transform.position).normalized * retractSpeed / (1.0f + stuckObject.resistance);
                
                        // Snap the harpoon when it's close
                        if (Vector3.Distance(harpoonSpot.position, harpoon.transform.position) < 0.15f)
                        {
                            ResetHarpoon(true);
                            break;
                        }
                        
                        tension = Mathf.Clamp01(tension + tensionIncreaseRate * Time.fixedDeltaTime);
                    }

                    break;
            }
            
            // Constantly decrease tension in any other state
            if (harpoonState != HarpoonState.Retracting)
                tension = Mathf.Clamp01(tension - tensionDecayRate * Time.fixedDeltaTime);
        }

        private void HandleOnAttachedToHand(Hand hand)
        {
            // Debug.Log($"[HarpoonLauncherController] Harpoon launcher attached to hand: {hand.handType}");

            harpoonLauncherHand = hand;
        }

        private void HandleOnDetachedFromHand(Hand hand)
        {
            // Debug.Log($"[HarpoonLauncherController] Harpoon launcher detached from hand: {hand.handType}");

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

            // Debug.Log($"[HarpoonLauncherController] Trigger amount changed to: {triggerAmount}");
            
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

            harpoonCollider.enabled = true;

            harpoonState = HarpoonState.Shooting;
        }

        private void StopHarpoon()
        {
            // Reduce the velocity of the harpoon
            if (stuckObject == null)
            {
                harpoonRb.linearVelocity /= stopFactor;
            }
            else
            {
                stuckObjectRb.linearVelocity /= stopFactor;
                stuckObject.OnStopReeling();
            }

            harpoonState = HarpoonState.Stationary;
        }

        private void RetractHarpoon()
        {
            if (stuckObject != null)
                stuckObject.OnStartReeling();
            
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

        private void HandleOnHitSurface(Harpoonable harpoonableSurface)
        {
            // Stick the harpoon to the surface
            stuckObject = harpoonableSurface;
            stuckObjectRb = stuckObject.GetComponent<Rigidbody>();
            
            harpoon.parent = harpoonableSurface.transform;
            harpoonRb.isKinematic = true;
            
            harpoonCollider.enabled = false;
            
            resistanceMeter.ToggleVisibility(true);
            
            OnSurfaceHit?.Invoke(harpoonableSurface);
        }

        private void HandleOnResetHarpoon(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool isClicked)
        {
            ResetHarpoon(false);
        }

        private void ResetHarpoon(bool isCatchSuccessful)
        {
            AttachHarpoonToLauncher();
            harpoonController.ReleaseHarpoon();
            
            if (stuckObject != null)
            {
                stuckObject.OnReelResult(isCatchSuccessful);
                stuckObject.OnStopReeling();
            }
            
            stuckObject = null;
            stuckObjectRb = null;

            // TOO BROKEN no thanks
            // tension = 0.0f;
            
            resistanceMeter.ToggleVisibility(false);
                        
            harpoonState = HarpoonState.Loaded;
            
            OnHarpoonReset?.Invoke(isCatchSuccessful);
        }
    }
}
