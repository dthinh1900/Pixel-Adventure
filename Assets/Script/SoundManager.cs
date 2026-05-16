using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Music")]
    public AudioClip backgroundMusic;

    [Header("Player")]
    public AudioClip jumpSFX;
    public AudioClip attackSFX;
    public AudioClip hurtSFX;
    public AudioClip playerDieSFX;

    [Header("Item")]
    public AudioClip collectSFX;

    [Header("Enemy")]
    public AudioClip enemyDieSFX;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}