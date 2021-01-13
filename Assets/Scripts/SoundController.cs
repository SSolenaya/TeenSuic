using UnityEngine;
using  Seka;

namespace Assets.Scripts {
    public class SoundController: Singleton<SoundController> { 

        public AudioSource backgroundMusic;
        public AudioSource effectsAudioSource;

        public bool soundState = true;

        [SerializeField] private AudioClip _backgroundClip;
        [SerializeField] private AudioClip _clickOnMedClip;
        [SerializeField] private AudioClip _gameOverClip;

        public delegate void SoundSwitch(bool b); // делегат

        public static event SoundSwitch OnSwitchSound; // событие

       public void Start () {
           Debug.Log(Inst.soundState);
            backgroundMusic.clip = _backgroundClip;
            backgroundMusic.Play();
            backgroundMusic.loop = true;
            soundState = PlayerPrefs.GetInt(GP.stateOfSound, 1) == 1;
            backgroundMusic.mute = !soundState;
            effectsAudioSource.mute = !soundState;
            EventOnSoundSwitch(soundState);
       }

        public void PlaySoundForClick () {
            effectsAudioSource.PlayOneShot(_clickOnMedClip);
        }

        public void PlaySoundForGameOver () {
            effectsAudioSource.PlayOneShot(_gameOverClip, 0.5f);
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