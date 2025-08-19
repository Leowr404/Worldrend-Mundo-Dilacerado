using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    [SerializeField] AudioSource loopingSFXSource;
    [SerializeField] AudioSource musicsource;
    [SerializeField] AudioSource SFXsource;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider MusicSlider;
    public static AudioManager instancia;
    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicVolumeKey = "MusicVolume";


    [Header("Configuracoes de Sons")]
    public AudioClip Tiro_sound;
    public AudioClip Coletavel;
    public AudioClip Select;
    public AudioClip Hit;
    public AudioClip Danger;
    public AudioClip Explosion;
    public AudioClip BossDeath;
    public AudioClip backgroundMusicMenu;
    public AudioClip backgroundMusicGameplay;
    private AudioSource backgroundMusicSource;
    //
    public AudioClip MouseEnter;
    public AudioClip MouseClick;

    private void Awake()
    {
        instancia = this;
    }
    void Start()
    {
        musicsource.clip = backgroundMusicMenu;
        musicsource.Play();




        //SetupBackgroundMusic();
        float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1.0f);
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);

        if (SFXSlider != null)
        {
            SFXSlider.value = savedSFXVolume;
        }

        if (MusicSlider != null)
        {
            MusicSlider.value = savedMusicVolume;
        }

        // Configurar os volumes iniciais
        SetSFX();
        SetMusic();
    }

    public void SetSFX()
    {
        if (mixer != null && SFXSlider != null)
        {
            float volume = SFXSlider.value;
            mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
            SFXSlider.value = volume;
            PlayerPrefs.SetFloat(SFXVolumeKey, volume);
            PlayerPrefs.Save();
        }

    }

    public void SetMusic()
    {
        if (mixer != null && MusicSlider != null)
        {
            float volume = MusicSlider.value;
            mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
            MusicSlider.value = volume;
            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
            PlayerPrefs.Save();
        }

    }

    public void PlaySFX(AudioClip clip, bool loop = false)
    {
        if (loop)
        {
            loopingSFXSource.clip = clip;      // Configura o clipe no looping source
            loopingSFXSource.loop = true;      // Ativa o loop
            loopingSFXSource.Play();           // Toca o som em loop
        }
        else
        {
            SFXsource.PlayOneShot(clip);       // Usa PlayOneShot para não interromper o som atual
        }

    }
    public void StopLoopingSFX()
    {
        loopingSFXSource.Stop();
        loopingSFXSource.loop = false;
    }
}
