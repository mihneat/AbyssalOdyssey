using System;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;

namespace Scripts
{
    public class Fish : Harpoonable
    {
        // float size
        // float weight
        // float value

        [SerializeField] private float regularSpeed = 1.0f;
        [SerializeField] private float panicSpeed = 3.0f;

        private float speed;
        private bool isBeingReeled;
        
        private void Awake()
        {
            speed = regularSpeed;
        }

        private void Update()
        {
            if (!isBeingReeled)
                transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }

        public override void OnHarpoonHit()
        {
            Debug.Log($"[Fish] I, {name}, a fish, have been hit!");
            
            // Enter panic mode!!! :o
            speed = panicSpeed;
        }

        public override void OnReelResult(bool isSuccessful)
        {
            if (isSuccessful)
            {
                Debug.Log($"[Fish] I, {name}, was successfully reeled in.");
                
                // TODO: Other stuff, such as rewards
            
                GetComponentsInChildren<Collider>().ForEach(coll => coll.enabled = false);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"[Fish] I, {name}, managed to escape.");
            }
        }

        public override void OnStartReeling()
        {
            isBeingReeled = true;
        }

        public override void OnStopReeling()
        {
            isBeingReeled = false;
        }
    }
}
