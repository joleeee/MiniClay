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
            /*float a;
            data[count] = (a = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate)));
            //print(a);
            position++;
            count++;
            print(a);*/
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

    // Update is called once per frame
    void Update () {
        if (!AS.isPlaying)
        {
            //print("AAAAYYYY");
            //SaveAudioClipToWav(ac, Application.streamingAssetsPath + "\audio.wav");
        }
	}

    static public void SaveAudioClipToWav(AudioClip audioClip, string filename)
    {
        FileStream fsWrite = File.Open(filename, FileMode.Create);

        BinaryWriter bw = new BinaryWriter(fsWrite);

        Byte[] header = { 82, 73, 70, 70, 22, 10, 4, 0, 87, 65, 86, 69, 102, 109, 116, 32 };
        bw.Write(header);

        Byte[] header2 = { 16, 0, 0, 0, 1, 0, 1, 0, 68, 172, 0, 0, 136, 88, 1, 0 };
        bw.Write(header2);

        Byte[] header3 = { 2, 0, 16, 0, 100, 97, 116, 97, 152, 9, 4, 0 };
        bw.Write(header3);

        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);
        int i = 0;

        while (i < audioClip.samples)
        {
            int sampleInt = (int)(32000.0 * samples[i++]);

            int msb = sampleInt / 256;
            int lsb = sampleInt - (msb * 256);

            bw.Write((Byte)lsb);
            bw.Write((Byte)msb);
        }

        fsWrite.Close();

    }
}
