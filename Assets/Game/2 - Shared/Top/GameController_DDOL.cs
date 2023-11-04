using UnityEngine;

public class GameController_DDOL : MonoBehaviour
{
    public int PreviousRoundMoves = 0;
    public StatSheet PreviousRoundStats;
    ChangeScene _sceneChanger;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Reset();
    }

    private void Start()
    {
        _sceneChanger = GetComponent<ChangeScene>();
        SetSoundLevel(2);
        SetMusicToggle(true);
    }

    public void ChangeScene(string sceneName)
    {
        _sceneChanger.SwapToScene(sceneName);
    }

    void Reset()
    {
        PreviousRoundMoves = 0;
    }



    public int currentVolume = 1;
    public bool musicOn = false;
    [SerializeField] public AudioSource audioSource_SFX;
    [SerializeField] public AudioSource audioSource_Music;

    public void SetMusicToggle(bool toggle)
    {
        musicOn = toggle;
        SetSoundLevel(currentVolume);
    }

    public void PauseSound()
    {
        audioSource_Music.Pause();
        audioSource_SFX.Pause();
    }

    public void ResumeSound()
    {
        audioSource_Music.UnPause();
        audioSource_SFX.UnPause();
    }

    public void PlaySound(string clipName)
    {
        AudioClip audioClip = GetAudioClipByName(clipName);
        if (audioClip != null)
        {
            audioSource_SFX.PlayOneShot(audioClip);
        }
        else
        {
            Debug.Log("null audio clip: " + clipName);
        }
    }
    public void StopSound()
    {
        audioSource_SFX.Stop();
    }
    public void PlayMusic(string songName)
    {
        AudioClip audioClip = GetSongClipByName(songName);
        if (audioClip != null)
        {
            audioSource_Music.Stop();
            audioSource_Music.clip = audioClip;
            audioSource_Music.Play();
        }
        else
        {
            Debug.Log("null audio clip: " + songName);
        }
    }

    AudioClip GetAudioClipByName(string clipName)
    {
        return (AudioClip)Resources.Load("Sounds/" + clipName);
    }

    AudioClip GetSongClipByName(string clipName)
    {
        return (AudioClip)Resources.Load("Songs/" + clipName);
    }

    public void SetSoundLevel(int volumeLevel)
    {
        currentVolume = volumeLevel;
        if (volumeLevel == 0)
        {
            audioSource_Music.volume = 0f;
            audioSource_SFX.volume = 0f;
        }
        if (volumeLevel == 1)
        {
            audioSource_Music.volume = musicOn ? 0.05f : 0f;
            audioSource_SFX.volume = 0.12f;
        }
        if (volumeLevel == 2)
        {
            audioSource_Music.volume = musicOn ? 0.12f : 0f;
            audioSource_SFX.volume = 0.18f;
        }
        if (volumeLevel == 3)
        {
            audioSource_Music.volume = musicOn ? 0.2f : 0f;
            audioSource_SFX.volume = 0.24f;
        }
    }
}
