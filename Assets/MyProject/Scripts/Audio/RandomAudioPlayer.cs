using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public Material[] materials;
    public AudioClip[] audioClips;
}

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] defaultClips;
    public AudioClip[] voiceClips;
    public Sound[] sounds;

    private void Awake()
    {
        audioSource = transform.GetComponent<AudioSource>();
        if (audioSource==null)
        {
            throw new System.Exception("未查询到AudioSource");
        }
    }

    public void PlayRandomAudio()
    {
        PlayRandomAudio(defaultClips);
        PlayRandomAudio(voiceClips);
    }

    public void PlayRandomAudio(AudioClip[] clips)
    {
        if(clips.Length>0)
        {
            int index = Random.Range(0, clips.Length);
            audioSource.clip = clips[index];
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    public void PlayRandomSound(Material material)
    {
        if (material==null)
        {
            PlayRandomAudio(defaultClips);
            return;
        }
        for (int i = 0; i < sounds.Length; i++)
        {
            for (int j = 0; j < sounds[i].materials.Length; j++)
            {
                if (material==sounds[i].materials[j])
                {
                    PlayRandomAudio(sounds[i].audioClips);
                    return;
                }
            }
        }
    }
}
