using System.Collections.Generic;

/// <summary>
/// Holds the entire JSON payload.
/// </summary>
[System.Serializable]
public class JSONPayload {
	public List<Payload> payload = new List<Payload>();
}

/// <summary>
/// An individual JSON payload entry.
/// </summary>
[System.Serializable]
public class Payload
{
	//In case we need them for later...
	//public int id;
	//public int view_order;
	//public float offset;
	public float duration;
	public string content;
	public Audio audio;
	public string sanitisedContent;
}

/// <summary>
/// Contains all audio paths relevant to a JSON payload entry.
/// </summary>
[System.Serializable]
public class Audio
{
	public string path;
	public string altPath;
}