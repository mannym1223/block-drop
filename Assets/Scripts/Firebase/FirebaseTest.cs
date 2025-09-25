using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class FirebaseTest : MonoBehaviour
{
	public Text text;

    [DllImport("__Internal")]
    public static extern void GetJson(string path, string objectName, string callback, string fallback);

	private void Start()
	{
        GetJson("example", gameObject.name, nameof(OnRequestSuccess), nameof(OnRequestFailed));
	}

	private void OnRequestSuccess(string data)
	{
		Debug.Log("Called my function. Data: " + data);

		text.text = data;
		text.color = Color.green;
	}

	private void OnRequestFailed(string error)
	{
		Debug.Log("Error: " + error);
		text.text = error;
		text.color = Color.red;
	}
}
