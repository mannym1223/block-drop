using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    TextMeshPro textMesh;
	int scoreDigits;

	private void Awake()
	{
		textMesh = GetComponent<TextMeshPro>();
		scoreDigits = textMesh.text.Length;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		BlockDropManager.Instance.OnScoreChanged?.AddListener(UpdateScore);
		textMesh.text = "";
		for (int i = 0; i < scoreDigits; i++)
		{
			textMesh.text += "0";
		}
    }

	private void OnDisable()
	{
		BlockDropManager.Instance.OnScoreChanged?.RemoveListener(UpdateScore);
	}

	protected void UpdateScore(int newScore)
	{
		string extraZeroes = "";
		for (int pow = scoreDigits; pow > 0; pow--)
		{
			if (newScore < Mathf.Pow(10, pow))
			{
				extraZeroes += "0";
			}
			else
			{
				break;
			}
		}
		textMesh.text = extraZeroes + newScore;
	}
}
