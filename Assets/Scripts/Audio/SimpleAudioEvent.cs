using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : ScriptableObject
{
    [SerializeField]
    private AudioClip[] _clips = new AudioClip[0];
    [SerializeField]
    private RangedFloat volume = new RangedFloat(1, 1);
    [SerializeField]
    [MinMaxRange(0f, 2f)]
    private RangedFloat pitch = new RangedFloat(1, 1);

    internal void Play(object audioSource)
    {
        throw new System.NotImplementedException();
    }

    [SerializeField]
    [MinMaxRange(0f, 1000f)]
    private RangedFloat _distance = new RangedFloat(1, 1000);
    [SerializeField]
    private AudioMixerGroup _mixer;


    public void Play(AudioSource source)
    {
        source.outputAudioMixerGroup = _mixer;

        int clipIndex = Random.Range(0, _clips.Length);
        source.clip = _clips[clipIndex];

        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        source.volume = Random.Range(volume.minValue, volume.maxValue);

        source.minDistance = _distance.minValue;
        source.maxDistance = _distance.maxValue;

        source.Play();
    }
}
