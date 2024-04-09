using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource music;
    private AudioSource sound;
    public AudioMixer audioMixer;

    public const string key_music = "key_music";
    public const string key_sound = "key_sound";

    private void Awake()
    {
        Instance = this;
        music = transform.Find("Music").GetComponent<AudioSource>();
        sound = transform.Find("Sound").GetComponent<AudioSource>();
        UpdateVolume();
    }

    //播放背景音乐
    public void  PlayMusic(AudioClip clip)
    {
        music.clip = clip;
        music.Play();
    }

    //播放音效
    public void PlaySound(AudioClip clip)
    {
        sound.PlayOneShot(clip);
    }

    //更新音量大小
    public void UpdateVolume()
    {
        //music.volume = PlayerPrefs.GetFloat(key_music, 0.3f);
        //sound.volume = PlayerPrefs.GetFloat(key_sound, 0.5f);
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat(key_music, 0.3f));
        audioMixer.SetFloat("SoundVolume", PlayerPrefs.GetFloat(key_sound, 0.5f));
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
