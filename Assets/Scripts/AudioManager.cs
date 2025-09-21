using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public AudioSource musicSource;

	public AudioClip playerMoveClip;
	public float playerMoveVolume = 0.5f;

	public AudioClip rowClearClip;
	public float rowClearVolume = 0.5f;

	public AudioClip MultiRowClearClip;
	public float MultiRowClearVolume = 0.5f;

	public AudioClip gameOverClip;
	public float gameOverVolume = 0.5f;
	public AudioClip gameOverMusic;

	public AudioClip droppedClip;
	public float droppedVolume = 0.5f;

    private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		//BlockDropManager.Instance.OnDropped?.AddListener(PlayDropped);
		BlockDropManager.Instance.OnPlayerMoved?.AddListener(PlayPlayerMoved);
		BlockDropManager.Instance.OnSingleRowCleared?.AddListener(PlayRowCleared);
		BlockDropManager.Instance.OnMultiRowCleared?.AddListener(PlayMultiRowCleared);
		BlockDropManager.Instance.OnGameOver?.AddListener(PlayGameOver);
	}

	protected void PlayPlayerMoved()
	{
		audioSource.PlayOneShot(playerMoveClip, playerMoveVolume);
	}

	protected void PlayRowCleared()
	{
		audioSource.PlayOneShot(rowClearClip, rowClearVolume);
	}

	protected void PlayMultiRowCleared()
	{
		audioSource.PlayOneShot(MultiRowClearClip, MultiRowClearVolume);
	}

	protected void PlayGameOver()
	{
		audioSource.PlayOneShot(gameOverClip, gameOverVolume);

		musicSource.Stop();
		//musicSource.clip = gameOverMusic;
	}

	protected void PlayDropped()
	{
		if (audioSource.isPlaying)
		{
			return;
		}
		audioSource.PlayOneShot(droppedClip, droppedVolume);
		//audioSource.volume = droppedVolume;
		//audioSource.Play();

	}
}
