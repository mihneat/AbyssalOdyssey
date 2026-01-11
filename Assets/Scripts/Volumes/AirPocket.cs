using System;
using UnityEngine;

namespace Scripts.Volumes
{
    public class AirPocket : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            
            // TODO: Increment air pocket counter
        }
    }
}
