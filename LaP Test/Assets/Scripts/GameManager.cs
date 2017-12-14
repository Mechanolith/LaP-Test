using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the core mangement of the entire game.
/// Spawns words and handles the results of them being click/not clicked.
/// Interfaces with all the other managers in the game to play audio, handle screen divisions, etc.
/// </summary>
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
	public float audioPadding;  //Some durations seem to be a bit short. Possibly an error in the JSON.
	public AudioClip introAudio;

	//Loaders and Managers
	Transform wordParent;
	JSONLoader jsonLoader;
	AudioManager audMan;
	DivisionSystem divMan;
	AestheticManager aesMan;

	//More UI (and Points Too)
	GameObject creditsPanel;
	int points;
	TextMeshProUGUI pointsUI;
	ImpactText impactUI;

	void Start() {
		//Grab required managers and tranforms.
		wordParent = GameObject.FindGameObjectWithTag("WordParent").transform;
		jsonLoader = GetComponent<JSONLoader>();
		audMan = GetComponent<AudioManager>();
		aesMan = GetComponent<AestheticManager>();
		divMan = GetComponent<DivisionSystem>();

		//Create the screen divisions.
		divMan.GenerateDivs(xDivs, yDivs, xDivPadding, yDivPadding, outerOnly);

		//Set up the UI and play the intro.
		creditsPanel = GameObject.FindGameObjectWithTag("CreditsPanel");
		creditsPanel.SetActive(false);
		pointsUI = GameObject.FindGameObjectWithTag("PointsUI").GetComponent<TextMeshProUGUI>();
		impactUI = GameObject.FindGameObjectWithTag("ImpactUI").GetComponent<ImpactText>();
		impactUI.SetGMan(this);
		impactUI.SetText("My World");
		AudioSource.PlayClipAtPoint(introAudio, Camera.main.transform.position);

		//Load previous points.
		if (PlayerPrefs.HasKey("Points"))
		{
			points = PlayerPrefs.GetInt("Points");
			pointsUI.text = points.ToString();
		}
	}

	void Update() {
		//Wait a moment for the title to play.
		if (startDelay > 0f)
		{
			startDelay -= Time.deltaTime;
			if (startDelay <= 0f)
			{
				//Once the game starts, spawn the required number of words at once.
				for (int indexA = 0; indexA < wordsToSpawn; ++indexA)
				{
					SpawnWord();
				}
			}
		}

		InputCheck();
	}

	/// <summary>
	/// Checks all possible player inputs (except clicks) and calls the appropriate responses.
	/// </summary>
	void InputCheck()
	{
		//Change the visual mode.
		if (Input.GetKeyDown(KeyCode.M))
		{
			//Currently an if because there's only two. Will update if more are added...
			if(aesMan.GetCurrentStyle().aesthetic == Aesthetic.e_Deep)
			{
				aesMan.SetAesthetic(Aesthetic.e_Soft);
			}
			else
			{
				aesMan.SetAesthetic(Aesthetic.e_Deep);
			}
		}

		//Show/Hide the Credits
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (creditsPanel.activeInHierarchy)
			{
				creditsPanel.SetActive(false);
			}
			else
			{
				creditsPanel.SetActive(true);
			}
		}

		//Quit the game.
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	/// <summary>
	/// Called by a word when clicked on. Plays a sound, gets a point, sets the impact text, and removes the word.
	/// </summary>
	/// <param name="_wordInfo">The details of the clicked word.</param>
	/// <param name="_word">The GameObject of the word that was clicked on.</param>
	/// <param name="_location">The screen Division the word was in.</param>
	public void OnWordClick(Payload _wordInfo, GameObject _word, Div _location)
	{
		switch (curStyle)
		{
			case Aesthetic.e_Soft:
				audMan.PlaySound(_wordInfo.audio.path, _wordInfo.duration + audioPadding);
				break;

			case Aesthetic.e_Deep:
				audMan.PlaySound(_wordInfo.audio.altPath, _wordInfo.duration + audioPadding * 3f);	//Add some extra padding because we don't have the exact length of all these...
				break;

			default:
				Debug.LogWarning("An audio response has not been set for the style " + curStyle + ".");
				break;
		}
		
		GetPoint();
		impactUI.SetText(_wordInfo.sanitisedContent);
		RemoveWord(_word, _location);
	}

	/// <summary>
	/// Destroys a given word and creates a new one.
	/// </summary>
	/// <param name="_word">GameObject of the word to be destroyed.</param>
	/// <param name="_location">The screen division the target word is in.</param>
	public void RemoveWord(GameObject _word, Div _location)
	{
		divMan.FreeDiv(_location);
		SpawnWord();
		Destroy(_word);
	}

	/// <summary>
	/// Called when a word is clicked on. Adds a point, updates the display, then saves that point to PlayerPrefs.
	/// </summary>
	public void GetPoint()
	{
		++points;

		//Set UI counter.
		pointsUI.text = points.ToString();

		//Save it so it isn't lost.
		PlayerPrefs.SetInt("Points", points);
		PlayerPrefs.Save();
	}

	/// <summary>
	/// Spawns a random word in a random location in a random division.
	/// </summary>
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

	/// <summary>
	/// Gets a random word from the JSON input.
	/// </summary>
	Payload GetRandomWord()
	{
		JSONPayload jInfo = jsonLoader.GetJSONInfo();

		int index = Random.Range(0, jInfo.payload.Count);

		return jInfo.payload[index];
	}

	/// <summary>
	/// Gets the current style from the aesthetic manager for reference elsewhere 
	/// (since other scripts cannot reference the manager directly).
	/// </summary>
	public VisualStyle GetCurrentStyle()
	{
		return aesMan.GetCurrentStyle();
	}
}
