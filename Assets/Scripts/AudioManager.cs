using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public AudioClip droppedAudio;
	public float droppedVolume = 0.5f;

    private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		BlockDropManager.Instance.OnDropped?.AddListener(PlayDropped);
    }

	protected void PlayDropped()
	{
		//audioSource.clip = droppedAudio;
		//audioSource.volume = droppedVolume;
		audioSource.Play();
	}
}
