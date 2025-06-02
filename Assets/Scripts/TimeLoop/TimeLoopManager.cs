using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        [SerializeField] private float vignetteSpeed;
        
        [SerializeField] private Image deathImage;
        
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
                StartCoroutine(FadeVignette(vignette));
        }

        IEnumerator FadeVignette(Vignette vignette)
        {
            const float maxVignette = 0.4f;
            while (true)
            {
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, maxVignette, vignetteSpeed * Time.deltaTime);
                if (Mathf.Abs(vignette.intensity.value - maxVignette) < 0.001f)
                    break;
                
                yield return null;
            }

            Debug.Log("[TimeLoopManager] Stopped vignette effect");
        }

        public void InfectPlayer()
        {
            Debug.Log("[TimeLoopManager] Infected!!");
            StartCoroutine(DeathFadeOut());
        }

        IEnumerator DeathFadeOut()
        {
            // Fade in purple
            float t = 0.0f;
            float purpleFadeInTime = 1.0f;
            Color deathCol = deathImage.color;
            deathCol.a = 1.0f;
            while (t < purpleFadeInTime)
            {
                deathImage.color = new Color(deathCol.r, deathCol.g, deathCol.b, Mathf.Lerp(0.0f, 1.0f, t / purpleFadeInTime));

                t += Time.deltaTime;
                yield return null;
            }

            deathImage.color = deathCol;
            
            // Hold for a bit
            t = 0.0f;
            float holdTime = 0.7f;
            while (t < holdTime)
            {
                t += Time.deltaTime;
                yield return null;
            }
            
            // Fade to black
            t = 0.0f;
            float fadeOutTime = 1.5f;
            while (t < fadeOutTime)
            {
                float u = t / fadeOutTime;
                deathImage.color = deathCol * (1 - u) + Color.black * u;
                
                t += Time.deltaTime;
                yield return null;
            }
            
            // Reload the scene
            SceneManager.LoadScene(0);
        }
    }
}
