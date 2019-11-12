using UnityEngine;
using  Seka;

namespace Assets.Scripts {
    public class SoundController: Singleton<SoundController> { 

        public AudioSource backgroundMusic;
        public AudioSource effectsAudioSource;

        public bool soundState = true;
        
        public AudioClip backgroundClip;
        public AudioClip clickOnMedClip;
        public AudioClip gameOverClip;

        public delegate void SoundSwitch(bool b); // делегат

        public static event SoundSwitch OnSwitchSound; // событие

       public void Start () {
            backgroundMusic.clip = backgroundClip;
            backgroundMusic.Play();
            backgroundMusic.loop = true;
            soundState = PlayerPrefs.GetInt(GP.stateOfSound, 1) == 1;
            backgroundMusic.mute = !soundState;
            effectsAudioSource.mute = !soundState;
            EventOnSoundSwitch(soundState);
            Meds.OnMedsEvent += PlaySoundForClick;
       }

        public void PlaySoundForClick () {
            effectsAudioSource.PlayOneShot(clickOnMedClip);
        }

        public void PlaySoundForGameOver () {
            effectsAudioSource.PlayOneShot(gameOverClip, 0.5f);
        }

        public void SwitchSound () {
            soundState = !soundState;
            EventOnSoundSwitch(soundState);
            backgroundMusic.mute = !soundState;
            effectsAudioSource.mute = !soundState;
            PlayerPrefs.SetInt(GP.stateOfSound, soundState ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void EventOnSoundSwitch(bool b) {
            if (OnSwitchSound != null) {
                OnSwitchSound(b);
            }
            else {
                Debug.Log("У делегата нет подписчиков");
            }
        }

    }


}