using UnityEngine;

public class PizzaSoundList : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioClip[] bgmClips;

    public AudioClip GetAudioClip(int idx) => audioClips[idx];
    public AudioClip GetBGM(int idx) => bgmClips[idx];
}
