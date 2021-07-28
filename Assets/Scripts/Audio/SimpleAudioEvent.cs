using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : ScriptableObject
{
    #region Serialized Fields
    [SerializeField]
    private AudioClip[] _clips = new AudioClip[0];
    [SerializeField]
    private RangedFloat volume = new RangedFloat(1, 1);
    [SerializeField]
    [MinMaxRange(0f, 2f)]
    private RangedFloat pitch = new RangedFloat(1, 1);

    [SerializeField]
    [MinMaxRange(0f, 1000f)]
    private RangedFloat _distance = new RangedFloat(1, 1000);
    [SerializeField]
    private AudioMixerGroup _mixer;
    #endregion

    #region Class Defined Methods
    /// <summary>
    /// Plays a random clip from the list of assigned audio clips from the SimpleAudioEvent editor.
    /// </summary>
    /// <param name="source">AudioSource object passed from the gameobject that receives an audio Action event.</param>
    /// <param name="isOneShot">Bool value that determines if the audio clip will be played as a OneShot defined
    /// in Unitys AudioSource API.</param>
    public void Play(AudioSource source, bool isOneShot = false)
    {
        source.outputAudioMixerGroup = _mixer;

        int clipIndex = Random.Range(0, _clips.Length);
        source.clip = _clips[clipIndex];

        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        source.volume = Random.Range(volume.minValue, volume.maxValue);

        source.minDistance = _distance.minValue;
        source.maxDistance = _distance.maxValue;

        if (isOneShot)
            source.PlayOneShot(source.clip);
        else
            source.Play();
    } 
    #endregion
}
