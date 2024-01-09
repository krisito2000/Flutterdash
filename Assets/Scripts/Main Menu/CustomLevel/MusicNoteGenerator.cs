using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNoteGenerator : MonoBehaviour
{
    public AudioSource musicSource;

    // Notes for creating the custom map
    public GameObject WNote;
    public GameObject ANote;
    public GameObject SNote;
    public GameObject DNote;

    public Transform spawnPosition;

    public float threshold = 0.1f; // Adjust this threshold for note generation

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        spawnPosition.position = Vector3.zero;

        if (musicSource == null)
        {
            Debug.LogError("No AudioSource found on this GameObject!");
        }
    }

    void Update()
    {
        AnalyzeAudio();
    }

    void AnalyzeAudio()
    {
        float[] spectrumData = new float[1024]; // Adjust the array size based on your needs
        musicSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Analyze spectrumData for sound level changes or frequency bands
        // Detect changes in audio input to trigger note generation
        float soundLevel = CalculateAverageLevel(spectrumData); // Calculate average sound level

        if (soundLevel > threshold) // Define your threshold for note generation
        {
            GenerateNote();
        }
    }

    void GenerateNote()
    {
        // Instantiate a random note prefab from available choices
        GameObject[] notes = { WNote, ANote, SNote, DNote };
        int randomIndex = Random.Range(0, notes.Length);
        GameObject selectedNote = notes[randomIndex];
        Vector3 position = spawnPosition.position;

        Instantiate(selectedNote, position, Quaternion.identity);
    }

    float CalculateAverageLevel(float[] spectrumData)
    {
        // Calculate sound level or amplitude change from spectrumData
        // You might calculate the average amplitude or use specific frequency bands to determine changes
        float sum = 0f;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            sum += spectrumData[i];
        }
        return sum / spectrumData.Length;
    }
}
