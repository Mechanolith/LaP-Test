using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[Header("General")]
	[Tooltip("How long before the game starts (in seconds).")]
	public float startDelay;
	[Tooltip("The number of words that will be spawned.")]
	public int wordsToSpawn;

	[Header("Word Spawning")]
	[Range(0f, 0.5f), Tooltip("Distance from the center (on the X axis) before words STOP spawning. Relative to screen size.")]
	public float xScreenMax;
	[Range(0f, 0.5f), Tooltip("Distance from the center (on the X axis) before words START spawning. Relative to screen size.")]
	public float xScreenMin;
	[Range(0f, 0.5f), Tooltip("Distance from the center (on the Y axis) before words STOP spawning. Relative to screen size.")]
	public float yScreenMax;
	[Range(0f, 0.5f), Tooltip("Distance from the center (on the Y axis) before words START spawning. Relative to screen size.")]
	public float yScreenMin;
	[Tooltip("The base word object that will be spawned.")]
	public GameObject wordObject;

	[Header("Audio")]
	[Tooltip("Increases the amount of time before audio clips are unloaded.")]
	public float audioPadding;	//Some durations seem to be a bit short. Possibly an error in the JSON.

	Transform canvas;
	JSONLoader jsonLoader;
	AudioManager aMan;
	int points;

	void Start() {
		canvas = GameObject.Find("Canvas").transform;
		jsonLoader = GetComponent<JSONLoader>();
		aMan = GetComponent<AudioManager>();

		//Make sure our deadzones make sense. If not, make them the closest viable option (prioritising Min values).
		if(xScreenMax < xScreenMin)
		{
			Debug.LogWarning("xScreenMax is smaller than xScreenMin. xScreenMax is now EQUAL to xScreenMin.");
			xScreenMax = xScreenMin;
		}

		if(yScreenMax < yScreenMin)
		{
			Debug.LogWarning("yScreenMax is smaller than yScreenMin. yScreenMax is now EQUAL to yScreenMin.");
			yScreenMax = yScreenMin;
		}

		//Load previous points.
		if (PlayerPrefs.HasKey("Points"))
		{
			points = PlayerPrefs.GetInt("Points");
		}
	}

	void Update() {
		if (startDelay > 0f)
		{
			startDelay -= Time.deltaTime;
			if (startDelay <= 0f)
			{
				for (int indexA = 0; indexA < wordsToSpawn; ++indexA)
				{
					SpawnWord();
				}
			}
		}
	}

	public void OnWordClick(Payload _wordInfo)
	{
		aMan.PlaySound(_wordInfo.audio.path, _wordInfo.duration + audioPadding);
		SpawnWord();
		GetPoint();
	}

	public void OnWordMiss(GameObject _missedWord)
	{
		SpawnWord();
		Destroy(_missedWord);
	}

	public void GetPoint()
	{
		++points;
		
		//Set UI counter.
		//TODO

		//Save it so it isn't lost.
		PlayerPrefs.SetInt("Points", points);
		PlayerPrefs.Save();
	}

	void SpawnWord()
	{
		Payload wordToSpawn = GetRandomWord();

		//Reduce the max X value relative to the word length (so things don't go off screen).
		float xMax = xScreenMax;
		//float minDist = 0.5f - (wordToSpawn.content.Length * 0.05f);

		////If the word is unreasonably large, let it go into the inner deadzone.
		//if(minDist < xScreenMin)
		//{
		//	xScreenMin = minDist;
		//}

		//xMax = Mathf.Clamp(xMax, xScreenMin, minDist);

		//Find where within each band each co-ord sits.
		float xPos = Random.Range(xMax, xMax);
		float yPos = Random.Range(yScreenMin, yScreenMax);

		//Randomise which band they're in.
		float xMod = Random.Range(0f, 1f) < 0.5f ? 0f : 0.5f;
		float yMod = Random.Range(0f, 1f) < 0.5f ? 0f : 0.5f;

		//Debug.Log("Spawning word at screen pos " + (xPos + xMod) + ", " + (yPos + yMod));

		//Put it in world space.
		Vector3 screenVector = new Vector3(xPos + xMod, yPos + yMod, 0f);

		Vector3 spawnPos = Camera.main.ViewportToWorldPoint(screenVector);
		spawnPos.z = 0f;	//Make sure the z value is always consistent in world space.

		//Spawn and setup the word.
		GameObject word = Instantiate(wordObject, spawnPos, Quaternion.identity);
		word.transform.SetParent(canvas);

		word.GetComponent<Word>().SetWord(wordToSpawn, this);
	}

	Payload GetRandomWord()
	{
		JSONPayload jInfo = jsonLoader.GetJSONInfo();

		int index = Random.Range(0, jInfo.payload.Count);

		return jInfo.payload[index];
	}
}
