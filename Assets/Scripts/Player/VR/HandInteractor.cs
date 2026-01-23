using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Helper;
using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Scripts.Player.VR
{
    [RequireComponent(typeof(Hand))]
    public class HandInteractor : MonoBehaviour
    {
        [SerializeField] private PlayerControllerVR playerController;
        [SerializeField] private TMP_Text interactionText;
        
        [SerializeField] private SteamVR_Input_Sources handType;
        
        private readonly Dictionary<IPlayerInteractable, int> currentInteractables = new();
        private readonly Stack<IPlayerInteractable> mostRecentInteractables = new();

        private SortedSet<IPlayerInteractable> interactables = new();

        private void OnEnable()
        {
            SteamVR_Actions._default.Interact.AddOnChangeListener(HandleOnInteract, handType);
        }

        private void OnDisable()
        {
            SteamVR_Actions._default.Interact.RemoveOnChangeListener(HandleOnInteract, handType);
        }

        public void AcceptInteractables(SortedSet<IPlayerInteractable> newInteractables)
        {
            // TODO: Maybe refuse interactables if already holding something in the hand
            
            interactables = newInteractables;
            UpdateInteractionText();
        }

        [Obsolete]
        public void AddInteractable(IPlayerInteractable interactable)
        {
            currentInteractables.TryAdd(interactable, 0);
            currentInteractables[interactable]++;

            mostRecentInteractables.Push(interactable);

            UpdateInteractionText();
        }

        [Obsolete]
        public void RemoveInteractable(IPlayerInteractable interactable)
        {
            if (!currentInteractables.ContainsKey(interactable) || currentInteractables[interactable] == 0)
                return;
            
            currentInteractables[interactable]--;
            if (currentInteractables[interactable] == 0)
                currentInteractables.Remove(interactable);
            
            CleanStack();

            UpdateInteractionText();
        }

        private void HandleOnInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool hasInteracted)
        {
            if (!hasInteracted)
                return;

            // CleanStack();
            //
            // if (mostRecentInteractables.Count == 0)
            //     return;

            if (interactables.Count == 0)
            {
                interactionText.text = "";
                return;
            }
            
            IPlayerInteractable currInteractable = interactables.First();
            currInteractable.Interact(playerController);
        }
        
        [Obsolete]
        private void CleanStack()
        {
            while (mostRecentInteractables.Count > 0)
            {
                IPlayerInteractable currInteractable = mostRecentInteractables.Peek();
                if (!currentInteractables.ContainsKey(currInteractable) || currentInteractables[currInteractable] == 0)
                {
                    mostRecentInteractables.Pop();
                    continue;
                }

                break;
            }
        }

        private void UpdateInteractionText()
        {
            // CleanStack();
            // if (mostRecentInteractables.Count == 0)
            // {
            //     interactionText.text = "";
            //     return;
            // }
            
            // IPlayerInteractable currInteractable = mostRecentInteractables.Peek();
            // interactionText.text = currInteractable.GetInteractActionName();

            if (interactables.Count == 0)
            {
                interactionText.text = "";
                return;
            }
            
            IPlayerInteractable currInteractable = interactables.First();
            interactionText.text = currInteractable.GetInteractActionName();
        }
    }
}
