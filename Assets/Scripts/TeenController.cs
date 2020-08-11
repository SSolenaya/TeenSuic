using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Seka;

namespace Assets.Scripts {
    public class TeenController: Singleton<TeenController> {
        private int maxHP = GP.lives; // максимальный уровень жизни - конец игры
        private int minHP = 0; // минимальный уровень жизни - конец игры
        public int valueFromMeds; // текущее количество набранных из таблеток очков
        public int hp = 50; // количество жизней для отображения
        public Text textHP; // текстовое отображение текущего количества жизни
        public Image imgLifeBar; // изображение лайф бара
        private float fA = 0; // переменная для рассчета заполнения бара жизни
        public Image face;
        public List<Sprite> boyFace = new List<Sprite>(4);

        public delegate void EndGame (string msg); // делегат

        public static event EndGame OnEndGame; // событие  

        public void Start () {
            textHP.text = "HP:" + hp;
            imgLifeBar.color = new Color(0f, 0.43f, 0.07f, 0.86f);
            imgLifeBar.fillAmount = 0.5f;
            MedsController.OnMCmedAction += EatMeds;
        }

        public void OnDisable() {
            MedsController.OnMCmedAction -= EatMeds;
        }

        public void EatMeds (Meds someMed) { // расчет текущего количества очков и их отображение
            valueFromMeds += someMed.GetValueOfMed();
            hp = Mathf.Clamp(maxHP / 2 + valueFromMeds, 0, 100);
            Debug.Log(gameObject.name);
            textHP.text = "HP:" + hp;
            BoyFace(hp);
            fA = (float)valueFromMeds / maxHP;
            imgLifeBar.fillAmount = 0.5f + fA;
            CheckEndGame();
        }

        public void BoyFace (int hp) {
            var temp = Mathf.Clamp(maxHP / (hp + 1) - 1, 0, 3);
            face.sprite = boyFace[temp];
        }

        public void CheckEndGame () {
            if((hp <= minHP) || (hp >= maxHP) /*|| (MedsController.Inst.currentQuantMeds >= GP.totalAmountMeds)*/) {
                string textMsg = hp <= minHP ? "You die and win!" : "Game over";
                OnEndGame?.Invoke(textMsg);
            }
        }

        public void Reset () {
            valueFromMeds = 0;
            hp = maxHP / 2;
            textHP.text = "HP:" + hp;
        }
    }
}
