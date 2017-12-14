using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Aesthetic
{
	e_Deep,
	e_Soft
}

public class GameManager : MonoBehaviour {

	[Header("General")]
	[Tooltip("How long before the game starts (in seconds).")]
	public float startDelay;
	[Tooltip("The number of words that will be spawned.")]
	public int wordsToSpawn;
	[Tooltip("The current visual/audio aesthetic of the game.")]
	public Aesthetic curStyle;

	[Header("Word Spawning")]
	[Tooltip("The number of divisions along the X axis of the screen. (Divisions determine viable word spawn locations.)")]
	public int xDivs;
	[Tooltip("The number of divisions along the Y axis of the screen. (Divisions determine viable word spawn locations.)")]
	public int yDivs;
	[Range(0f, 0.49f), Tooltip("The amount of padding (in normalised screenspace) between two divisions on the X axis.")]
	public float xDivPadding;
	[Range(0f, 0.49f), Tooltip("The amount of padding (in normalised screenspace) between two divisions on the Y axis.")]
	public float yDivPadding;
	[Tooltip("If true, words will only spawn in divisions on the outer edge of the grid.")]
	public bool outerOnly;

	[Tooltip("The base word object that will be spawned.")]
	public GameObject wordObject;

	[Header("Audio")]
	[Tooltip("Increases the amount of time before audio clips are unloaded.")]
	public float audioPadding;	//Some durations seem to be a bit short. Possibly an error in the JSON.

	Transform wordParent;
	JSONLoader jsonLoader;
	AudioManager audMan;
	DivisionSystem divMan;
	int points;
	TextMeshProUGUI pointsUI;
	ImpactText impactUI;

	void Start() {
		wordParent = GameObject.FindGameObjectWithTag("WordParent").transform;
		jsonLoader = GetComponent<JSONLoader>();
		audMan = GetComponent<AudioManager>();
		divMan = GetComponent<DivisionSystem>();

		divMan.GenerateDivs(xDivs, yDivs, xDivPadding, yDivPadding, outerOnly);

		pointsUI = GameObject.FindGameObjectWithTag("PointsUI").GetComponent<TextMeshProUGUI>();
		impactUI = GameObject.FindGameObjectWithTag("ImpactUI").GetComponent<ImpactText>();
		impactUI.SetGMan(this);

		//Load previous points.
		if (PlayerPrefs.HasKey("Points"))
		{
			points = PlayerPrefs.GetInt("Points");
			pointsUI.text = points.ToString();
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

	public void OnWordClick(Payload _wordInfo, GameObject _word, Div _location)
	{
		audMan.PlaySound(_wordInfo.audio.path, _wordInfo.duration + audioPadding);
		GetPoint();
		impactUI.SetText(_wordInfo.sanitisedContent);
		RemoveWord(_word, _location);
	}

	public void RemoveWord(GameObject _word, Div _location)
	{
		divMan.FreeDiv(_location);
		SpawnWord();
		Destroy(_word);
	}

	public void GetPoint()
	{
		++points;

		//Set UI counter.
		pointsUI.text = points.ToString();

		//Save it so it isn't lost.
		PlayerPrefs.SetInt("Points", points);
		PlayerPrefs.Save();
	}

	void SpawnWord()
	{
		Payload wordToSpawn = GetRandomWord();
		Div spawnLocation = divMan.GetRandomDiv();

		//Pick a random position within the Division.
		float xPos = Random.Range(spawnLocation.bottomLeftBound.x, spawnLocation.topRightBound.x);
		float yPos = Random.Range(spawnLocation.bottomLeftBound.y, spawnLocation.topRightBound.y);

		//Put it in world space.
		Vector3 screenVector = new Vector3(xPos, yPos, 0f);

		Vector3 spawnPos = Camera.main.ViewportToWorldPoint(screenVector);
		spawnPos.z = 0f;	//Make sure the z value is always consistent in world space.

		//Spawn and setup the word.
		GameObject word = Instantiate(wordObject, spawnPos, Quaternion.identity);
		word.transform.SetParent(wordParent);

		word.GetComponent<Word>().SetWord(wordToSpawn, this, spawnLocation);
	}

	Payload GetRandomWord()
	{
		JSONPayload jInfo = jsonLoader.GetJSONInfo();

		int index = Random.Range(0, jInfo.payload.Count);

		return jInfo.payload[index];
	}
}
