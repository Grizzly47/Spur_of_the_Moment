using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] tracks; // Assign your audio clips in the Inspector

    private int currentTrackIndex = 0;

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
        PlayMusic(currentTrackIndex);
    }

    public void PlayMusic(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= tracks.Length) return;

        audioSource.clip = tracks[trackIndex];
        audioSource.Play();
        currentTrackIndex = trackIndex;
    }

    public void NextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        PlayMusic(currentTrackIndex);
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }
}
