using UnityEngine;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    ScrollRect scroll;

	private void Awake()
	{
		scroll = GetComponent<ScrollRect>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        scroll.velocity = Vector2.right * 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
