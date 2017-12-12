using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NAudio;
using NAudio.Wave;

public class AudioManager : MonoBehaviour {

	private IWavePlayer waveOut;
	private WaveStream waveOutputStream;
	private WaveChannel32 volumeStream;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//if(waveOut != null && waveOut.PlaybackState == PlaybackState.Stopped)
		//{
		//	Debug.Log("Audio playback stopped.");
		//	UnloadAudio();
		//}
	}

	private void LoadAudio(byte[] _data)
	{
		MemoryStream memStream = new MemoryStream(_data);
		waveOutputStream = new Mp3FileReader(memStream);
		volumeStream = new WaveChannel32(waveOutputStream);

		waveOut = new WaveOut();
		waveOut.Init(volumeStream);
	}

	public void LoadAndPlayAudio(string _path)
	{
		string soundPath = "file:///" + Application.dataPath + "/Provided Assets/" + _path;

		WWW www = new WWW(soundPath);
		byte[] soundData = www.bytes;

		LoadAudio(soundData);
		waveOut.Play();

		Resources.UnloadUnusedAssets();
	}

	private void UnloadAudio()
	{
		waveOut.Stop();

		volumeStream.Close();
		volumeStream = null;

		waveOutputStream.Close();
		waveOutputStream = null;

		waveOut.Dispose();
		waveOut = null;
	}
}
