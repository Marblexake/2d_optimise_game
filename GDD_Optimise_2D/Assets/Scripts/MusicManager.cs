using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{

    public Slider backgroundMusicSlider;            // Reference to the slider that controls the background music
    public Slider soundEffectsSlider;               // Reference to the slider that controls the sound effect slider

    public AudioSource backgroundMusicSource;       // Reference to the audiosource for background music
    public AudioSource soundEffectsSource;          // Reference to the sound effects source

    private float SoundEffectsVolume;               // For the main menu sound effects since there is no sound effects in the main menu

    void Start()
    {
        // Finds the slider values in player prefs and updates them accordingly as the player may have changed it in-game
        backgroundMusicSlider.value = PlayerPrefs.GetFloat("BackgroundMusicVolume", 1);
        soundEffectsSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume", 1);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateBackgroundMusicVolume();
        UpdateSoundEffectsVolume();
    }

    private void UpdateBackgroundMusicVolume()
    {
        // Sets the background music volume to be the slider's value
        backgroundMusicSource.volume = backgroundMusicSlider.value;

        // Saves the volume value in player prefs
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicSource.volume);
    }

    private void UpdateSoundEffectsVolume()
    {
        // Saves the slider value to a variable
        SoundEffectsVolume = soundEffectsSlider.value;

        // Checks if there is an object referenced in this var
        if (soundEffectsSource)
        {
            // Sets sound effects source volume to the value the player wants
            soundEffectsSource.volume = soundEffectsSlider.value;
        }

        // Saves the value in player prefs
        PlayerPrefs.SetFloat("SoundEffectsVolume", SoundEffectsVolume);
    }

}
