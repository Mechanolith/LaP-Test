using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ImpactText : MonoBehaviour {

	Animator textAnim;
	TextMeshProUGUI tMesh;
	bool shrinking;
	GameManager gMan;

	void Start () {
		tMesh = GetComponent<TextMeshProUGUI>();
		textAnim = GetComponent<Animator>();
	}

	public void SetGMan(GameManager _gMan)
	{
		gMan = _gMan;
	}

	public void SetText(string _text)
	{
		//If the style calls for it, force all caps.
		if(gMan.curStyle == Aesthetic.e_Deep)
		{
			_text = _text.ToUpper();
		}

		//Debug.Log("Impact text is now " + _text);

		tMesh.text = _text;

		textAnim.SetTrigger("Shrink");
	}
}
