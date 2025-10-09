using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private bool loop = true;

    private static AudioSource musicSource;

    private void Awake()
    {
        if (musicSource != null)
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = musicClip;
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.playOnAwake = false;

        musicSource.Play();
        DontDestroyOnLoad(gameObject);
    }
}
