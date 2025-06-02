using System;
using Scripts.Helper;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private TMP_Text hudInteractText;
        [SerializeField] private CinemachineCamera cam;
        [SerializeField] private float interactionDistance;
        [SerializeField] private LayerMask interactionLayerMask;
        
        private IPlayerInteractable currentInteractable;
        
        private void Update()
        {
            CheckForInteractables();
        }

        private void OnEnable()
        {
            playerController.OnInteract += TryInteract;
        }

        private void OnDisable()
        {
            playerController.OnInteract -= TryInteract;
        }

        private void CheckForInteractables()
        {
            Ray ray = new Ray(transform.position, cam.transform.forward);
            bool hasHit = Physics.Raycast(ray, out var hit, interactionDistance, interactionLayerMask);

            if (hasHit && hit.transform.TryGetComponent<IPlayerInteractable>(out var interactable))
            {
                hudInteractText.text = interactable.GetInteractActionName();
                currentInteractable = interactable;
            }
            else
            {
                hudInteractText.text = "";
                currentInteractable = null;
            }
        }

        private void TryInteract()
        {
            if (currentInteractable == null)
            {
                // Debug.Log("[PlayerInteractor] No interactable to interact with");
                return;
            }
            
            currentInteractable.Interact(playerController);
        }

        public void ToggleComponent(bool state)
        {
            hudInteractText.text = "";
            currentInteractable = null;
            enabled = state;
        }
    }
}
