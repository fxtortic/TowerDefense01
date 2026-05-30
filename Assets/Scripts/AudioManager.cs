using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Звуки")]
    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioClip enemyDeathSound;
    public AudioClip enemyReachBaseSound;
    public AudioClip placeTowerSound;
    public AudioClip buttonClickSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}