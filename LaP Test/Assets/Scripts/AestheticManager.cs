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

	void Start () {
		mainCam = Camera.main;

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
	/// Sets the current aesthetic to the input, then updates all instant aspects of the game's visual style.
	/// </summary>
	/// <param name="_aesthetic">The aesthetic to be set.</param>
	public void SetAesthetic(Aesthetic _aesthetic)
	{
		curStyle = GetStyle(_aesthetic);
		curAesthetic = _aesthetic;
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
	/// Returns the current style for reference elsewhere.
	/// </summary>
	/// <returns></returns>
	public VisualStyle GetCurrentStyle()
	{
		return curStyle;
	}
}
