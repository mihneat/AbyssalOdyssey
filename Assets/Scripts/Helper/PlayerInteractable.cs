using Scripts.Player;
using UnityEngine;

namespace Scripts.Helper
{
    public interface IPlayerInteractable
    {
        public string GetInteractActionName();
        public void Interact(PlayerController playerController);
    }
}
