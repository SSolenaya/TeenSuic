using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Seka;

namespace Assets.Scripts {
    public class TeenController: Singleton<TeenController> {

        private int maxHP = 100; // максимальный уровень жизни - конец игры

        private int minHP = 0; // минимальный уровень жизни - конец игры
        public int valueFromMeds; // текущее количество набранных из таблеток очков
        public int hp = 50; // количество жизней для отображения

        public Text textHP; // текстовое отображение текущего количества жизни

        public Image imgLifeBar; // изображение лайф бара

        private float fA = 0; // переменная для рассчета заполнения бара жизни

        public Image face;
        public List<Sprite> boyFace = new List<Sprite>(4);

        public void Start () {
            textHP.text = "HP:" + hp;
            imgLifeBar.color = new Color(0f, 0.43f, 0.07f, 0.86f);
            imgLifeBar.fillAmount = 0.5f;
        }

        public void EatMeds (Meds someMed) { // расчет текущего количества очков и их отображение
            CheckEndGame();
            valueFromMeds += someMed.GetValueOfMed();
            //hp = maxHP / 2 + valueFromMeds;
            hp = Mathf.Clamp(maxHP / 2 + valueFromMeds, 0, 100);
            textHP.text = "HP:" + hp;
            BoyFace(hp);
            fA = (float)valueFromMeds / maxHP;
            imgLifeBar.fillAmount = 0.5f + fA;
        }

        public void BoyFace (int hp) {
            var temp = Mathf.Clamp(maxHP / (hp + 1) - 1, 0, 3);
            face.sprite = boyFace[temp];
        }

        public void CheckEndGame () {
            if((hp <= minHP) || (hp >= maxHP) || (MedsController.Inst.currentQuantMeds >= GP.totalamountMeds)) {
                string text = hp <= minHP ? "You die and win!" : "Game over";
                StartCoroutine(MainLogic.Inst.IETextEndGame(text));

                MedsController.Inst.parentRectTransform.gameObject.SetActive(false);
                MedsController.Inst.StopGeneration();
            }
        }

        public void Reset () {
            valueFromMeds = 0;
            hp = maxHP / 2;
            textHP.text = "HP:" + hp;
        }
    }
}
