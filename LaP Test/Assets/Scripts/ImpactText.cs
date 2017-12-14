using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Primarily handles the animation of the impact text.
/// Also changes the fonts etc. based on external commands.
/// </summary>
public class ImpactText : MonoBehaviour {

	Animator textAnim;
	TextMeshProUGUI tMesh;
	AestheticManager aesMan;
	string animString = "Shrink";

	void Start () {
		tMesh = GetComponent<TextMeshProUGUI>();
		textAnim = GetComponent<Animator>();
	}

	/// <summary>
	/// Sets the message of the impact text. Forces to all caps if it matches the current aesthetic/style.
	/// </summary>
	/// <param name="_text">The message to set.</param>
	public void SetText(string _text)
	{
		//If the style calls for it, force all caps.
		if (aesMan.GetCurrentAesthetic() == Aesthetic.e_Deep)
		{
			_text = _text.ToUpper();
		}

		tMesh.text = _text;

		textAnim.SetTrigger(animString);
	}

	#region Aesthetics
	/// <summary>
	/// Gets a reference to the Aesthetic Manager so it can stay up to date later on.
	/// </summary>
	/// <param name="_aesMan"></param>
	public void SetAesMan(AestheticManager _aesMan)
	{
		aesMan = _aesMan;
	}

	/// <summary>
	/// Changes the font of the text mesh.
	/// </summary>
	/// <param name="_font">Font to change to.</param>
	public void SetFont(TMP_FontAsset _font)
	{
		tMesh.font = _font;
	}

	/// <summary>
	/// Changes the desired animation of the text.
	/// </summary>
	/// <param name="_string">Name of the animation trigger.</param>
	public void SetAnimString(string _string)
	{
		animString = _string;
	}
	#endregion
}
