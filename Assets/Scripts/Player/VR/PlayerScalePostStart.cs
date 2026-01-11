using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Scripts.Player.VR
{
    public class PlayerScalePostStart : MonoBehaviour
    {
        private bool hasScaled = false;
        
        void Update()
        {
            if (!hasScaled)
            {
                hasScaled = true;
                transform.localScale = new Vector3(2.15f, 2.15f, 2.15f);
            }
        }
    }
}
