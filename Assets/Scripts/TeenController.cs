using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Seka;

namespace Assets.Scripts {
    public class TeenController: MonoBehaviour {
        private int _maxHP = GP.lives; // максимальный уровень жизни - конец игры
        private int _minHP = 0; // минимальный уровень жизни - конец игры
        private int _valueFromMeds; // текущее количество набранных из таблеток очков
        private int _hp = GP.lives/2; // количество жизней для отображения на старте игры
        [SerializeField] private Text _textHP; // текстовое отображение текущего количества жизни
        [SerializeField] private Image _imgLifeBar; // изображение лайф бара
        [SerializeField] private Image _face;
        [SerializeField] private List<Sprite> _boyFaceSprites = new List<Sprite>(4);

        public delegate void EndGame (string msg); // делегат

        public static event EndGame OnEndGame; // событие  

        public void Start () {
            _textHP.text = "HP:" + _hp;
            _imgLifeBar.color = new Color(0f, 0.43f, 0.07f, 0.86f);
            _imgLifeBar.fillAmount = 0.5f;
            MedsController.OnMCmedAction += EatMeds;
        }

        public void OnDisable() {
            MedsController.OnMCmedAction -= EatMeds;
        }

        public void EatMeds (Meds someMed) { // расчет текущего количества очков и их отображение
            _valueFromMeds += someMed.GetValueOfMed();
            _hp = Mathf.Clamp(_maxHP / 2 + _valueFromMeds, 0, 100);
            _textHP.text = "HP:" + _hp;
            BoyFace(_hp);
            float _fA = 0; // переменная для рассчета заполнения бара жизни
            _fA = (float)_valueFromMeds / _maxHP;
            _imgLifeBar.fillAmount = 0.5f + _fA;
            CheckEndGame();
        }

        public void BoyFace (int hp) {
            var temp = Mathf.Clamp(_maxHP / (hp + 1) - 1, 0, 3);
            _face.sprite = _boyFaceSprites[temp];
        }

        public void CheckEndGame () {
            if((_hp <= _minHP) || (_hp >= _maxHP) /*|| (MedsController.Inst.currentQuantMeds >= GP.totalAmountMeds)*/) {
                string textMsg = _hp <= _minHP ? "You die and win!" : "Game over";
                OnEndGame?.Invoke(textMsg);
            }
        }

        public void Reset () {
            _valueFromMeds = 0;
            _hp = _maxHP / 2;
            _textHP.text = "HP:" + _hp;
        }
    }
}
