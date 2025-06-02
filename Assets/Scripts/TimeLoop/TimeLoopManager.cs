using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Scripts.TimeLoop
{
    public class TimeLoopManager : MonoBehaviour
    {
        private struct TimestampedEvent
        {
            public float t; // timestamp in seconds
            public UnityEvent timeEvent;
        }

        [SerializeField] private Volume volume;
        
        [SerializedDictionary("Timestamp", "List of events")]
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<float, UnityEvent> events;

        private float t;

        private readonly List<TimestampedEvent> eventSequence = new();

        private void Awake()
        {
            // Transform the dictionary into a chronological list
            foreach (var (timestampMinutes, timeEvent) in events)
                eventSequence.Add(new TimestampedEvent
                {
                   t = timestampMinutes * 60, // Convert to seconds
                   timeEvent = timeEvent
                });
            
            eventSequence.Sort((te1, te2) => te1.t < te2.t ? 1 : -1);
        }

        private void Update()
        {
            t += Time.deltaTime;

            // Find the events to execute
            var eventsToExecute = eventSequence.FindAll(te => te.t < t);
            if (eventsToExecute.Count == 0)
                return;
            
            // Execute the events chronologically
            eventsToExecute.Sort((te1, te2) => te1.t < te2.t ? 1 : -1);
            eventsToExecute.ForEach(te => te.timeEvent.Invoke());
            
            // Remove all past events
            eventSequence.RemoveAll(te => te.t < t);
        }

        public void Cough()
        {
            // TODO: idk something, maybe just partially strain the eyes
            Debug.Log("[TimeLoopManager] Coughing");
        }

        public void StrainEyes()
        {
            // Slowly fade in the purple vignette
            Debug.Log("[TimeLoopManager] Strain");

            if (volume.profile.TryGet<Vignette>(out var vignette))
            {
                Debug.Log("[TimeLoopManager] Changing vignette effect..");
                vignette.intensity.value = 0.4f;
            }
        }

        public void InfectPlayer()
        {
            // TODO: Kill the player, reload the scene
            Debug.Log("[TimeLoopManager] Infected!!");
        }
    }
}
