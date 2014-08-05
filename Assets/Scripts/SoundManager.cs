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
        GetAudioSource().PlayOneShot(GetAudioSource().clip);
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
                GetAudioSource().pitch = 1f;
                return GetAudioSource().clip = Resources.Load("Audio/ErrorAlert") as AudioClip;
            case SoundEffectTypes.LineClear:
                GetAudioSource().volume = .05f;
                GetAudioSource().pitch = 1f;
                return GetAudioSource().clip = Resources.Load("Audio/LineClear") as AudioClip;
            case SoundEffectTypes.SoftDrop:
                GetAudioSource().volume = .05f;
                GetAudioSource().pitch = 0.8f;
                return GetAudioSource().clip = Resources.Load("Audio/SoftDrop") as AudioClip;
            case SoundEffectTypes.HardDrop:
                GetAudioSource().volume = .3f;
                GetAudioSource().pitch = 3f;
                return GetAudioSource().clip = Resources.Load("Audio/HardDrop") as AudioClip;
        }
        return null;
    }

    public enum SoundEffectTypes
    {
        ErrorSound,
        LineClear,
        SoftDrop,
        HardDrop
    }
}