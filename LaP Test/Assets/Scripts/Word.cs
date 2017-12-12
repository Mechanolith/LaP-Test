using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Word : MonoBehaviour {

	TextMeshProUGUI textMesh;
	Payload wordInfo;
	GameManager gMan;

	void Awake () {
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}
	
	void Update () {
		
	}

	public void OnClick()
	{
		gMan.OnWordClick(wordInfo);

		Destroy(gameObject);
	}

	public void SetWord(Payload _word, GameManager _manager)
	{
		gMan = _manager;

		wordInfo = _word;

		string tempString = wordInfo.content;
		textMesh.text = "";

		//Remove any punctuation we don't want. (Probably a better system for this many...)
		tempString = tempString.Replace("\n", "");
		tempString = tempString.Replace(".", "");
		tempString = tempString.Replace(",", "");
		tempString = tempString.Replace("/", "");
		tempString = tempString.Replace("!", "");
		tempString = tempString.Replace(";", "");
		tempString = tempString.Replace(" ", "");

		//Capitalise the first letter.
		tempString = char.ToUpper(tempString[0]) + tempString.Substring(1);

		textMesh.text = tempString;
		
		Debug.Log(wordInfo.content + " = " + textMesh.text);
	}
}
