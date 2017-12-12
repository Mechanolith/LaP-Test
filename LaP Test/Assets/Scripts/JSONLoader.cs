using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// For loading all JSON in the game.
/// </summary>
public class JSONLoader : MonoBehaviour {

	public JSONPayload payload;

	private void Awake()
	{
		LoadJSON();
	}

	void Start () {
		
	}

	void Update () {
		
	}

	/// <summary>
	/// Extracts the JSON from its file and puts it in its appropriate class.
	/// </summary>
	void LoadJSON()
	{
		FileInfo fInfo = new FileInfo(Application.dataPath + "/Provided Assets/myworld.ver1.words.json");
		StreamReader reader = fInfo.OpenText();

		string text;
		string JSONText = "";

		//Read all the text from the file and store it in jsonText.
		do
		{
			text = reader.ReadLine();
			JSONText += text;
		}
		while (text != null);
		reader.Close();

		payload = JsonUtility.FromJson<JSONPayload>(JSONText);
	}
}
