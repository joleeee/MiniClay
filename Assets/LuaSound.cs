using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LuaSound : MonoBehaviour {

    AudioClip ac;
    AudioSource AS;
    int position;
    int samplerate = 44100;
    float frequency = 440;
    //float[] soundData = new float[16];
    float index = 0;
    [Range(0.01f, 1.99f)]
    public float pitchMutliplier = 1;

    // Use this for initialization
    void Start() {
        ac = AudioClip.Create("TestSound", samplerate * 120, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
        AS = GetComponent<AudioSource>();
        AS.clip = ac;
        //AS.volume = 0.01f;
        AS.Play();
	}

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            if(index > samplerate / frequency * pitchMutliplier)
            {
                index -= samplerate / frequency;
            }

            if(index > samplerate / frequency / 2 * pitchMutliplier)
            {
                data[count] = -1;
            }
            else
            {
                data[count] = 1;
            }
            position++;
            count++;
            index++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }
}
