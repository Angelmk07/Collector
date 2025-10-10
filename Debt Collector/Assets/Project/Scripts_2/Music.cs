using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private bool loopPlaylist = true; 

    private static AudioSource musicSource;
    private static int currentTrackIndex = 0;

    private void Awake()
    {
        if (musicSource != null)
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.volume = volume;
        musicSource.loop = false;
        musicSource.playOnAwake = false;

        DontDestroyOnLoad(gameObject);
        StartCoroutine(PlayMusicSequentially());
    }

    private IEnumerator PlayMusicSequentially()
    {
        while (true)
        {
            if (musicClips.Length == 0)
                yield break;

            musicSource.clip = musicClips[currentTrackIndex];
            musicSource.Play();

            yield return new WaitForSeconds(musicSource.clip.length);

            currentTrackIndex++;

            if (currentTrackIndex >= musicClips.Length)
            {
                if (loopPlaylist)
                    currentTrackIndex = 0; 
                else
                    yield break;
            }
        }
    }
}
