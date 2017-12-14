﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Word : MonoBehaviour {

	[Tooltip("The amount of time (in seconds) before this word will automatically destroy itself.")]
	public float deathTime = 5f;

	[Tooltip("How far the word will move to get out of the way of something.")]
	public float adjustmentValue;

	TextMeshProUGUI textMesh;
	Payload wordInfo;
	GameManager gMan;
	Div curLocation;

	void Awake () {
		//Grab the required components.
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}
	
	void Update () {
		//
		if(deathTime > 0f)
		{
			deathTime -= Time.deltaTime;

			if(deathTime <= 0f)
			{
				gMan.RemoveWord(gameObject, curLocation);
			}
		}
	}

	/// <summary>
	/// Called when the player clicks on the word.
	/// </summary>
	public void OnClick()
	{
		gMan.OnWordClick(wordInfo, gameObject, curLocation);
	}

	/// <summary>
	/// Called as soon as the word is spawned. Sets all relevant data.
	/// </summary>
	/// <param name="_word"></param>
	/// <param name="_manager"></param>
	/// <param name="_location"></param>
	public void SetWord(Payload _word, GameManager _manager, Div _location)
	{
		gMan = _manager;
		curLocation = _location;

		wordInfo = _word;

		string tempString = wordInfo.sanitisedContent;

		//If the style calls for it, put the text in all caps.
		if(gMan.curStyle == Aesthetic.e_Deep)
		{
			tempString = tempString.ToUpper();
		}

		textMesh.text = tempString;

		textMesh.font = gMan.GetCurrentStyle().font;
	}
}
