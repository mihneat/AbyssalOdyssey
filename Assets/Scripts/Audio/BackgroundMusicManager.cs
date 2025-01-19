using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicManager : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> backgroundSongs;
        [SerializeField] private List<AudioClip> fishReelingSongs;

        // Audio source #1: background music
        // Audio source #2: fish reeling music
        private AudioSource[] audioSources;
        
        private void Awake()
        {
            audioSources = GetComponents<AudioSource>();

            SetRandomSong(0, backgroundSongs);
            audioSources[0].Play();
        }

        private void Update()
        {
            if (!audioSources[0].isPlaying && !audioSources[1].isPlaying)
            {
                // Change the background song
                SetRandomSong(0, backgroundSongs);
                audioSources[0].Play();
            }
        }

        private void SetRandomSong(int index, List<AudioClip> clips)
        {
            if (clips.Count == 0)
            {
                Debug.LogWarning($"[BackgroundMusicManager] No songs set to be used for audio source #{index}");
                return;
            }
            
            audioSources[index].clip = clips[Random.Range(0, clips.Count)];
        }
    }
}
