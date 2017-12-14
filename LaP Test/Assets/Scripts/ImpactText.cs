using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ImpactText : MonoBehaviour {

	Animator textAnim;
	TextMeshProUGUI tMesh;
	AestheticManager aesMan;
	string animString = "Shrink";

	void Start () {
		tMesh = GetComponent<TextMeshProUGUI>();
		textAnim = GetComponent<Animator>();
	}

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
	public void SetAesMan(AestheticManager _aesMan)
	{
		aesMan = _aesMan;
	}

	public void SetFont(TMP_FontAsset _font)
	{
		tMesh.font = _font;
	}

	public void SetAnimString(string _string)
	{
		animString = _string;
	}
	#endregion
}
