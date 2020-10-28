using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class UIController:MonoBehaviour {
        public Button newGameButton;
        public Button btnSound;

        public Sprite soundOn;
        public Sprite soundOff;

        public void Start () {
            ChangeMuteBtnImg(SoundController.Inst.soundState);
            SoundController.OnSwitchSound += ChangeMuteBtnImg;

            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(() =>    // что происходит при нажатии кнопки Новая игра 
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneNames.gameSceneName);
            });

            btnSound.onClick.RemoveAllListeners();
            btnSound.onClick.AddListener(() =>    // что происходит при нажатии кнопки Sound
            {
                SoundController.Inst.SwitchSound();
            });
        }

        public void ChangeMuteBtnImg(bool var) {
            btnSound.image.sprite = var ? soundOn : soundOff;
        }

        public void OnDestroy() {
            SoundController.OnSwitchSound -= ChangeMuteBtnImg;
        }
    }
}
