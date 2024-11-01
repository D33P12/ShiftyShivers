using UnityEngine;

public enum AudioType
{
     PLAYERWALK
    ,PLAYERIDLE,
     PLAYERASSASSINATE
    ,PLAYERTAKEDAMAGE
    ,TWONDER,
     TRUNAWAY,
     EPATROL,
     ECHASE,
     
    
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip[] audioList;
    private AudioSource audioSource;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
      
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void PlayAudio(AudioType sound, float volume = 0.25f)
    {
        if (audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(audioList[(int)sound], volume);
        }
    }

    public void PlayAudioContinuous(AudioType sound, float volume = 0.5f)
    {
        if (audioList.Length > (int)sound && audioList[(int)sound] != null)
        {
            audioSource.clip = audioList[(int)sound];
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
    public void StopPlaying()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }
}

