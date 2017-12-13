using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[Tooltip("How long before the game starts (in seconds).")]
	public float startDelay;
	[Tooltip("The maximum number of words allowed on the screen at any one time.")]
	public int maxWords;
	[Tooltip("Increases the amount of time before audio clips are unloaded.")]
	public float audioPadding;	//Some durations seem to be a bit short. Possibly an error in the JSON.
	[Tooltip("The base word object that will be spawned.")]
	public GameObject wordObject;
	Transform canvas;
	JSONLoader jsonLoader;
	AudioManager aMan;
	int currentWords;
	float maxBoundX;    //Min and Max bounds for spawns on the X axis.
	float minBoundX;
	float maxBoundY;    //Min and Max bounds for spawns on the Y axis.
	float minBoundY;

	void Start() {
		canvas = GameObject.Find("Canvas").transform;
		jsonLoader = GetComponent<JSONLoader>();
		aMan = GetComponent<AudioManager>();
	}

	void Update() {
		if (startDelay > 0f)
		{
			startDelay -= Time.deltaTime;
			if (startDelay <= 0f)
			{
				SpawnWord();
			}
		}
		else
		{

		}
	}

	public void OnWordClick(Payload _wordInfo)
	{
		--currentWords;
		aMan.PlaySound(_wordInfo.audio.path, _wordInfo.duration + audioPadding);
		SpawnWord();
	}

	void SpawnWord()
	{
		if (currentWords < maxWords)
		{
			float xPos = Random.Range(minBoundX, maxBoundX);
			float yPos = Random.Range(minBoundY, maxBoundY);

			Vector3 spawnPos = new Vector3(xPos, yPos, 0f);

			GameObject word = Instantiate(wordObject, spawnPos, Quaternion.identity);
			word.transform.SetParent(canvas);

			word.GetComponent<Word>().SetWord(GetRandomWord(), this);

			++currentWords;
		}
	}

	Payload GetRandomWord()
	{
		JSONPayload jInfo = jsonLoader.GetJSONInfo();

		int index = Random.Range(0, jInfo.payload.Count);

		return jInfo.payload[index];
	}
}
