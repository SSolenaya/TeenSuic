using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts {

    public class MainLogic : MonoBehaviour {
        [SerializeField]
        private Button _continueBtn;
        [SerializeField]
        private GameObject _endGameModalInfo;
        [SerializeField]
        private Text _endGameText;
        [SerializeField]
        private Button _mainMenuBtn;
        [SerializeField]
        private Button _pauseBtn;
        [SerializeField]
        private GameObject _pauseModalInfo;
        [SerializeField]
        private Button _soundBtn;
        [SerializeField]
        private Sprite _soundOffSprite;
        [SerializeField]
        private Sprite _soundOnSprite;
        private Coroutine _coro;

        private void Awake() {
            _endGameModalInfo.SetActive(false);
            TeenController.OnEndGame += EndGame;
            MedsController.OnMCEndGame += EndGame;
        }

        void OnDisable() {
            TeenController.OnEndGame -= EndGame;
            MedsController.OnMCEndGame -= EndGame;
        }

        public void Start() {
            SoundController.OnSwitchSound += ChangeMuteBtnImgGame;
            
            _soundBtn.onClick.RemoveAllListeners();
            _soundBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                SoundController.Inst.SwitchSound();
            });
            _pauseBtn.onClick.RemoveAllListeners();
            _pauseBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                Time.timeScale = 0;
                _pauseModalInfo.SetActive(true);
            });
            _continueBtn.onClick.RemoveAllListeners();
            _continueBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                Time.timeScale = 1f;
                _pauseModalInfo.SetActive(false);
            });
            _mainMenuBtn.onClick.RemoveAllListeners();
            _mainMenuBtn.onClick.AddListener(() => // что происходит при нажатии кнопки Sound
            {
                SceneManager.LoadScene(GP.nameSceneMainMenu);
            });
        }

        public void ChangeMuteBtnImgGame(bool var) {
            _soundBtn.image.sprite = var ? _soundOnSprite : _soundOffSprite;
        }

        public void TextOnEndGame(string textForModalInfo) {
            StopCoro(_coro);
            _coro = StartCoroutine(IETextEndGame(textForModalInfo));
        }

        private void StopCoro(Coroutine corou) {
            if (corou != null) {
                StopCoroutine(corou);
                corou = null;
            }
        }

        private IEnumerator IETextEndGame(string textForModalInfo) {
            _endGameText.text = textForModalInfo;
            _endGameModalInfo.SetActive(true);
            SoundController.Inst.PlaySoundForGameOver();
            yield return new WaitForSeconds(1.5f);
            _endGameModalInfo.SetActive(false);
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