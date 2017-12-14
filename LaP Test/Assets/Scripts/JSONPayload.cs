using System.Collections.Generic;

[System.Serializable]
public class JSONPayload {
	public List<Payload> payload = new List<Payload>();
}

[System.Serializable]
public class Payload
{
	//public int id;
	//public int view_order;
	//public float offset;
	public float duration;
	public string content;
	public Audio audio;
	public string sanitisedContent;
}

[System.Serializable]
public class Audio
{
	public string path;
	public string altPath;
}