using System;
using System.Collections.Generic;
using Scripts.Helper;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Scripts.Player.VR
{
    public class HandInteractionChecker : MonoBehaviour
    {
        class ByPriority : IComparer<IPlayerInteractable>
        {
            public int Compare(IPlayerInteractable x, IPlayerInteractable y)
            {
                if (x == null || y == null)
                    return 0;
                    
                return x.GetPriority() - y.GetPriority();
            }
        }
        
        [SerializeField] private HandCollider handCollider;
        private HandInteractor handInteractor;

        private readonly SortedSet<IPlayerInteractable> interactables = new(new ByPriority());
        
        // TODO: Should this be moved to FixedUpdate to remove text flicker?
        private void Update()
        {
            LazyGetHandInteractor().AcceptInteractables(interactables);
            interactables.Clear();
        }

        private void OnTriggerStay(Collider other)
        {
            IPlayerInteractable interactable = other.GetComponent<IPlayerInteractable>();
            if (interactable == null)
                return;
            
            interactables.Add(interactable);
        }

        // private void OnTriggerEnter(Collider other)
        // {
        //     IPlayerInteractable interactable = other.GetComponent<IPlayerInteractable>();
        //     if (interactable == null)
        //         return;
        //     
        //     // Send the event to the hand
        //     LazyGetHandInteractor().AddInteractable(interactable);
        //     Debug.Log("[HandInteractionChecker] Added '" + interactable.GetInteractActionName() + "'");
        // }
        //
        // private void OnTriggerExit(Collider other)
        // {
        //     IPlayerInteractable interactable = other.GetComponent<IPlayerInteractable>();
        //     if (interactable == null)
        //         return;
        //     
        //     // Send the event to the hand
        //     LazyGetHandInteractor().RemoveInteractable(interactable);
        //     Debug.Log("[HandInteractionChecker] Removed '" + interactable.GetInteractActionName() + "'");
        // }

        private HandInteractor LazyGetHandInteractor()
        {
            if (handInteractor != null)
                return handInteractor;

            return handInteractor = handCollider.hand.GetComponent<HandInteractor>();
        }
    }
}
