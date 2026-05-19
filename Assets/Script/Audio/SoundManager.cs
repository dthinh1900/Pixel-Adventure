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
        // Nếu đã có SoundManager khác thì xóa object mới
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Singleton
        instance = this;

        // Giữ lại khi chuyển scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Đọc volume đã lưu
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Áp dụng volume
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        // Chỉ phát nhạc nếu chưa phát
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            PlayMusic(backgroundMusic);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // Nếu đang phát đúng bài này thì không phát lại
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource == null) return;

        musicSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null) return;

        sfxSource.volume = volume;

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}