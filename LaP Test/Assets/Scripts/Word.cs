using System.Collections;
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

	BoxCollider2D boxCol;
	bool positionLocked = false;
	Div curLocation;

	void Awake () {
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
		boxCol = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
		if(deathTime > 0f)
		{
			deathTime -= Time.deltaTime;

			if(deathTime <= 0f)
			{
				gMan.RemoveWord(gameObject, curLocation);
			}
		}

		if (!positionLocked)
		{
			//CheckPosition();
		}
	}

	public void OnClick()
	{
		gMan.OnWordClick(wordInfo, gameObject, curLocation);
	}

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
	}

	void CheckPosition()
	{
		//Set up the circle cast based on the text bounds.
		Vector3 origin = gameObject.transform.position;
		origin.x += boxCol.bounds.extents.x;
		float radius = boxCol.bounds.extents.y;
		float distance = boxCol.bounds.size.x;

		RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, radius, -transform.right, distance);

		//If we hit anything (not including hitting yourself)...
		if(hits.Length > 1)
		{
			Vector3 adjustVec = Vector3.zero;

			//Adjust the position based on where the things we hit were.
			for (int indexA = 0; indexA < hits.Length; ++indexA)
			{
				if (hits[indexA].transform != transform)	//Ignore hitting yourself.
				{
					//Vector2 posVec = new Vector2(transform.position.x, transform.position.y);
					//Vector2 dirVec = posVec - hits[indexA].point;

					//Vector3 directionVector = new Vector3(dirVec.x, dirVec.y, 0f);

					Vector3 directionVector = transform.position - hits[indexA].transform.position;
					adjustVec += directionVector.normalized * adjustmentValue;
				}
			}

			transform.position += adjustVec;

			Debug.Log("Adjusting by " + adjustVec);
		}
		else
		{
			//Otherwise we're good. Stop trying to change things.
			positionLocked = true;
			Debug.Log("Hit nothing, position locked!");
		}
	}
}
