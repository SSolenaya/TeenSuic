using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Seka;
using TMPro.EditorUtilities;

namespace Assets.Scripts {

    public class MainLogic : Singleton<MainLogic> {
        public Button continueBtn;
        public GameObject endGameModalInfo;
        public Text endGameText;
        public Button mainMenuBtn;
        public Button pauseBtn;
        public GameObject pauseModalInfo;
        public Button soundBtn;
        public Sprite soundOff;
        public Sprite soundOn;
        private Coroutine _coro;

        private void Awake() {
            endGameModalInfo.SetActive(false);
            TeenController.OnEndGame += EndGame;
            MedsController.OnMCEndGame += EndGame;
        }

        public void Start() {
            SoundController.OnSwitchSound += ChangeMuteBtnImgGame;
            
            soundBtn.onClick.RemoveAllListeners();
            soundBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                SoundController.Inst.SwitchSound();
            });
            pauseBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                Time.timeScale = 0;
                pauseModalInfo.SetActive(true);
            });
            continueBtn.onClick.RemoveAllListeners();
            continueBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                Time.timeScale = 1f;
                pauseModalInfo.SetActive(false);
            });
            mainMenuBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                SceneManager.LoadScene(GP.nameSceneMainMenu);
            });
        }

        public void ChangeMuteBtnImgGame(bool var) {
            soundBtn.image.sprite = var ? soundOn : soundOff;
        }

        public void TextOnEndGame(string textForModalInfo) {
            StopCoro(_coro);
            _coro = StartCoroutine(IETextEndGame(textForModalInfo));
        }

        private void StopCoro(Coroutine corou) {
            if (corou == null) return;
            StopCoroutine(corou);
            corou = null;
        }

        private IEnumerator IETextEndGame(string textForModalInfo) {
            endGameText.text = textForModalInfo;
            endGameModalInfo.SetActive(true);
            SoundController.Inst.PlaySoundForGameOver();
            yield return new WaitForSeconds(1.5f);
            endGameModalInfo.SetActive(false);
            SceneManager.LoadScene(GP.nameSceneMainMenu); // start screen
        }

        public void EndGame(string messageOnGameEnd) {
            TextOnEndGame(messageOnGameEnd);
            }

        public void OnDestroy() {
            SoundController.OnSwitchSound -= ChangeMuteBtnImgGame;
        }
    }
}