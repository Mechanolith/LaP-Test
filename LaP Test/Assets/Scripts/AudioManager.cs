using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NAudio;
using NAudio.Wave;

public class AudioInstance
{
	private IWavePlayer waveOut;
	private WaveStream waveOutputStream;
	private WaveChannel32 volumeStream;
	public float duration;

	public AudioInstance()
	{
		duration = 0f;
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
		string soundPath = "file:///" + Application.streamingAssetsPath + "/" + _path;

		WWW www = new WWW(soundPath);
		byte[] soundData = www.bytes;

		LoadAudio(soundData);
		waveOut.Play();

		Resources.UnloadUnusedAssets();
	}

	public void UnloadAudio()
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

public class AudioManager : MonoBehaviour {

	List<AudioInstance> currentAudio = new List<AudioInstance>();
	
	void Update () {
		//Update all current audio
		for(int indexA = 0; indexA < currentAudio.Count; ++indexA)
		{
			if(currentAudio[indexA].duration > 0f)
			{
				currentAudio[indexA].duration -= Time.deltaTime;

				//If the audio has finished playing
				if (currentAudio[indexA].duration <= 0f)
				{
					//Unload resources.
					currentAudio[indexA].UnloadAudio();
					//Remove it from the list
					currentAudio.RemoveAt(indexA);
					//Update index to account of the shorter list.
					--indexA;
				}
			}
		}
	}
	
	/// <summary>
	/// Plays a sound located at a given file adress. Will be unloaded automatically when the _duration ends.
	/// </summary>
	/// <param name="_path">The file adress within Provided Assets where the file can be found.</param>
	/// <param name="_duration">The duration of the sound.</param>
	public void PlaySound(string _path, float _duration)
	{
		AudioInstance sound = new AudioInstance();
		sound.duration = _duration;
		sound.LoadAndPlayAudio(_path);
		currentAudio.Add(sound);
	}
}
