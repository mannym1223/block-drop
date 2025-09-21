using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public AudioClip playerMoveClip;
	public float playerMoveVolume;

	public AudioClip rowClearClip;
	public float rowClearVolume;

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
		BlockDropManager.Instance.OnRowCleared?.AddListener(PlayRowCleared);
	}

	protected void PlayPlayerMoved()
	{
		audioSource.PlayOneShot(playerMoveClip, playerMoveVolume);
	}

	protected void PlayRowCleared()
	{
		audioSource.PlayOneShot(rowClearClip, rowClearVolume);
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
