using UnityEngine;
using UnityEngine.Audio;
[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool isLoop;
    public bool hasCooldown;
    public bool randomPitch;
    public int priority = 127;

    [Range(0, 3)]
    public float MinRandomPitch = 1;
    [Range(0, 3)]
    public float MaxRandomPitch = 3;
    [HideInInspector] public AudioSource source;
}