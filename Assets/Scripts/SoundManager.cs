using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static AudioClip currentClip;
    public static GameObject audioSourceInstance;

    public static void Play (SoundEffectTypes effectToPlay)
    {
        GetSoundEffect(effectToPlay);
        GetAudioSource().PlayOneShot(currentClip);
    }

    private static AudioSource GetAudioSource ()
    {
        //if it's null, build the object
        if (audioSourceInstance == null)
        {
            audioSourceInstance = new GameObject("GameSoundInstance");
            audioSourceInstance.AddComponent<AudioSource>();
        }
        return audioSourceInstance.GetComponent<AudioSource>();
    }

    private static AudioClip GetSoundEffect (SoundEffectTypes effectToPlay)
    {
        switch (effectToPlay)
        {
            case SoundEffectTypes.ErrorSound:
                GetAudioSource().volume = .05f;
                return currentClip = Resources.Load("Audio/ErrorAlert") as AudioClip;
        }
        return null;
    }

    public enum SoundEffectTypes
    {
        ErrorSound,
    }
}