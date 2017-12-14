using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Used to define the current visual aesthetic of the game.
/// </summary>
public enum Aesthetic
{
	e_Deep,
	e_Soft
}

/// <summary>
/// Defines all the specific traits of a visual styles, such as colour, music, and font.
/// </summary>
[System.Serializable]
public class VisualStyle
{
	[Tooltip("The aesthetic this style is for. MUST BE UNIQUE.")]
	public Aesthetic aesthetic;
	[Tooltip("Colour of the camera background for this style.")]
	public Color bgCol;
	[Tooltip("The font of all generic text in this style.")]
	public TMP_FontAsset font;
	[Tooltip("The font of the large impact text in this style.")]
	public TMP_FontAsset impactFont;
	[Tooltip("The name of the animation the impact text should play.")]
	public string impactAnim;
	[Tooltip("The background music for this style.")]
	public AudioClip music;
	[Tooltip("The parent object for all the style's particles.")]
	public GameObject particleObject;
}

/// <summary>
/// Handles the setting and storage of the games various aesthetic themes.
/// </summary>
public class AestheticManager : MonoBehaviour {

	[Tooltip("All possible visual styles the game can take on.")]
	public List<VisualStyle> styles;
	[Tooltip("The rate at which the game switches styles. (Only applies to things that can be lerped.)")]
	public float transitionRate;
	[Tooltip("The default aesthetic the game will begin in.")]
	public Aesthetic startAesthetic;

	Camera mainCam;
	VisualStyle curStyle;
	Aesthetic curAesthetic;

	GameObject creditsPanel;
	TextMeshProUGUI pointsUI;
	TextMeshProUGUI instructionUI;
	ImpactText impactUI;
	List<Word> activeWords = new List<Word>();

	void Awake () {
		mainCam = Camera.main;

		//Grab and set up the UI
		creditsPanel = GameObject.FindGameObjectWithTag("CreditsPanel");
		creditsPanel.SetActive(false);
		pointsUI = GameObject.FindGameObjectWithTag("PointsUI").GetComponent<TextMeshProUGUI>();
		instructionUI = GameObject.FindGameObjectWithTag("InstructionUI").GetComponent<TextMeshProUGUI>();
		impactUI = GameObject.FindGameObjectWithTag("ImpactUI").GetComponent<ImpactText>();
		impactUI.SetAesMan(this);
	}

	void Start()
	{
		//Set the default style.
		curStyle = GetStyle(startAesthetic);
		SetAesthetic(startAesthetic);
	}

	void Update () {
		UpdateColor();
	}

	/// <summary>
	/// Lerps the current camera colour to the required value.
	/// </summary>
	void UpdateColor()
	{
		float rVal = mainCam.backgroundColor.r;
		float gVal = mainCam.backgroundColor.g;
		float bVal = mainCam.backgroundColor.b;
		float aVal = mainCam.backgroundColor.a;

		rVal = Mathf.Lerp(rVal, curStyle.bgCol.r, transitionRate);
		gVal = Mathf.Lerp(gVal, curStyle.bgCol.g, transitionRate);
		bVal = Mathf.Lerp(bVal, curStyle.bgCol.b, transitionRate);
		aVal = Mathf.Lerp(aVal, curStyle.bgCol.a, transitionRate);

		mainCam.backgroundColor = new Color(rVal, gVal, bVal, aVal);
	}

	/// <summary>
	/// Returns a visual style that matches the input aesthetic.
	/// </summary>
	/// <param name="_aesthetic">Aesthetic of the style you want returned.</param>
	/// <returns></returns>
	VisualStyle GetStyle(Aesthetic _aesthetic)
	{
		for(int indexA = 0; indexA < styles.Count; ++indexA)
		{
			if (styles[indexA].aesthetic == _aesthetic)
			{
				return styles[indexA];
			}
		}

		Debug.LogWarning("Style with aesthetic " + _aesthetic + " does not exist!");

		return styles[0];
	}

	/// <summary>
	/// Sets the fonts of all active words to match the current style.
	/// </summary>
	void SetActiveWordFonts()
	{
		for(int indexA = 0; indexA < activeWords.Count; ++indexA)
		{
			activeWords[indexA].SetFont(curStyle.font);
		}
	}

	#region Public Gets
	/// <summary>
	/// Returns the current style for reference elsewhere.
	/// </summary>
	/// <returns></returns>
	public VisualStyle GetCurrentStyle()
	{
		return curStyle;
	}

	/// <summary>
	/// Returns the current aesthetic for reference elsewhere, so we don't have to pass the whole style.
	/// </summary>
	public Aesthetic GetCurrentAesthetic()
	{
		return curAesthetic;
	}
	#endregion

	#region Public Sets
	/// <summary>
	/// Sets the current aesthetic to the input, then updates all instant aspects of the game's visual style.
	/// </summary>
	/// <param name="_aesthetic">The aesthetic to be set.</param>
	public void SetAesthetic(Aesthetic _aesthetic)
	{
		//Disable the current particles.
		curStyle.particleObject.SetActive(false);

		//Update Style info
		curStyle = GetStyle(_aesthetic);
		curAesthetic = _aesthetic;

		//Update all the relevant UI to match style.
		impactUI.SetFont(curStyle.impactFont);
		impactUI.SetAnimString(curStyle.impactAnim);

		pointsUI.font = curStyle.font;
		instructionUI.font = curStyle.font;

		curStyle.particleObject.SetActive(true);

		SetActiveWordFonts();
	}

	/// <summary>
	/// Sets the word for the Impact Text and has it start its animation.
	/// </summary>
	/// <param name="_text">The message the to show as Impact Text</param>
	public void SetImpactText(string _text)
	{
		impactUI.SetText(_text);
	}

	/// <summary>
	/// Sets the text for the points UI to the number of points passed to the function.
	/// </summary>
	/// <param name="_points">The number to display.</param>
	public void SetPoints(int _points)
	{
		pointsUI.text = _points.ToString();
	}

	/// <summary>
	/// Sets the text for the points UI to the string passed in.
	/// </summary>
	/// <param name="_pointsString">The string to display.</param>
	public void SetPoints(string _pointsString)
	{
		pointsUI.text = _pointsString;
	}
	#endregion

	/// <summary>
	/// Shows the credits panel if it is hidden, hides it if it is shown.
	/// </summary>
	public void ToggleCredits()
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

	/// <summary>
	/// Adds the given word to the list of active words.
	/// </summary>
	/// <param name="_word">The word script to add.</param>
	public void AddActiveWord(Word _word)
	{
		activeWords.Add(_word);
	}

	/// <summary>
	/// Removes the given word from the list of active words.
	/// </summary>
	/// <param name="_word">The word object containing the correct script to remove.</param>
	public void RemoveActiveWord(GameObject _word)
	{
		activeWords.Remove(_word.GetComponent<Word>());
	}
}
