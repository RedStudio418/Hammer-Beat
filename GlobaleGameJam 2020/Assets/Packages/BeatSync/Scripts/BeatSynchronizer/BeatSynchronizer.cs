using UnityEngine;
using System.Collections;

/// <summary>
/// This class should be attached to the audio source for which synchronization should occur, and is 
/// responsible for synching up the beginning of the audio clip with all active beat counters and pattern counters.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BeatSynchronizer : MonoBehaviour {

	public float startDelay = 1f;	// Number of seconds to delay the start of audio playback.
	public delegate void AudioStartAction(double syncTime);
	public static event AudioStartAction OnAudioStart;

    [SerializeField] MusicProvider _manager;

    private void Awake()
    {
        _manager.OnLaunchMusic += StartMusic;
    }

    void StartMusic ()
	{
		double initTime = AudioSettings.dspTime;
		GetComponent<AudioSource>().PlayScheduled(initTime + startDelay);
		OnAudioStart?.Invoke(initTime + startDelay);
	}

}
