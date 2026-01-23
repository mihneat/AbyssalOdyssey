using Scripts.Player;
using Scripts.Player.VR;
using UnityEngine;

namespace Scripts.Helper
{
    public interface IPlayerInteractable
    {
        public string GetInteractActionName();
        public int GetPriority();
        public void Interact(PlayerController playerController);
        public void Interact(PlayerControllerVR playerController);
    }
}
