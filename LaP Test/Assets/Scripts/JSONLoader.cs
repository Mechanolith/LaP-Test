using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// For loading all JSON in the game.
/// </summary>
public class JSONLoader : MonoBehaviour {

	private JSONPayload jsonInfo;

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
		FileInfo fInfo = new FileInfo(Application.streamingAssetsPath + "/myworld.ver1.words.json");
		StreamReader reader = fInfo.OpenText();

		string text;
		string jsonText = "";

		//Read all the text from the file and store it in jsonText.
		do
		{
			text = reader.ReadLine();
			jsonText += text;
		}
		while (text != null);
		reader.Close();

		jsonInfo = JsonUtility.FromJson<JSONPayload>(jsonText);

		SanitiseText();
	}

	private void SanitiseText()
	{
		for(int indexA = 0; indexA < jsonInfo.payload.Count; ++indexA)
		{
			string tempString = jsonInfo.payload[indexA].content;

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

			jsonInfo.payload[indexA].sanitisedContent = tempString;
		}
	}

	public JSONPayload GetJSONInfo()
	{
		return jsonInfo;
	}
}
