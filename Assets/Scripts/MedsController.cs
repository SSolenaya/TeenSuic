using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Seka;
using Random = UnityEngine.Random;

namespace Assets.Scripts {
    public enum TypeColors {
        c1 = 1,
        c2,
        c3,
        c4,
        c5
    }

    public class MedsController: MonoBehaviour {

        public Meds prefabMeds; // ссылка на префаб таблетки
        public Text counterText;    //  текст счетчика упавших таблеток
        public List<Meds> currentMeds = new List<Meds>();   
        public RectTransform parentRectTransform; // ссылка на родителя таблетки
        public int numMeds;// переменная для отображения обратного счетчика таблеток
        public float tempTime;
        public int currentQuantMeds; // переменная для счетчика таблеток
        private int value1;
        private int value2;
        private int value3;
        private int value4;
        private int value5;
        private Color color1;
        private Color color2;
        private Color color3;
        private Color color4;
        private Color color5;
        private Coroutine coroGenerationMeds;


        public delegate void MCEndGame (string msg); // делегат

        public static event MCEndGame OnMCEndGame; // событие

        public delegate void MCmedAction (Meds med); // делегат

        public static event MCmedAction OnMCmedAction; // событие

        void Awake () {
            SetValueForType();
            numMeds = GP.totalAmountMeds;
            PoolManager.Init(parentRectTransform);
            TeenController.OnEndGame += EndGameFromTeenController;
        }

        void OnDisable() {
            TeenController.OnEndGame -= EndGameFromTeenController;
        }

        void Start () {
            counterText.text = numMeds + "/" + GP.totalAmountMeds;
            StartGenerationFromController();
        }

        public void SetValueForType () {

            value1 = Random.value < 0.5 ? -1 : 1; // присвоение случайного значения из 2, для реализации "хорошая/плохая таблетка"
            value2 = Random.value < 0.5 ? -1 : 1;
            value3 = Random.value < 0.5 ? -1 : 1;
            value4 = Random.value < 0.5 ? -1 : 1;
            value5 = Random.value < 0.5 ? -1 : 1;

            color1 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            color2 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            color3 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            color4 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            color5 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        }

        public TypeColors Get_random_TypeColor (Meds m) {
            m.colorOfMed = (TypeColors)Random.Range(1, 6);
            return m.colorOfMed;
        }

        public Color GetColorByTypeColors (TypeColors tc) {
            switch(tc) {
                case TypeColors.c1:
                    return color1;
                case TypeColors.c2:
                    return color2;
                case TypeColors.c3:
                    return color3;
                case TypeColors.c4:
                    return color4;
                case TypeColors.c5:
                    return color5;
                default:
                    return color1;
            }
        }

        public int GetValueByTypeColors (TypeColors tc) {
            switch(tc) {
                case TypeColors.c1:
                    return value1;
                case TypeColors.c2:
                    return value2;
                case TypeColors.c3:
                    return value3;
                case TypeColors.c4:
                    return value4;
                case TypeColors.c5:
                    return value5;
                default:
                    return value1;
            }
        }

        public void StopGeneration () {
            if(coroGenerationMeds != null) {
                StopCoroutine(coroGenerationMeds);
            }
            DeleteCurrentMeds();
            parentRectTransform.gameObject.SetActive(false);
            coroGenerationMeds = null;
        }

        public void DeleteCurrentMeds () {// функция для удаления текущих таблеток при начале новой игры
            foreach(var m in currentMeds) {// перебираем все элементы списка
                if(m != null) {// если они не нулевые
                    Destroy(m.gameObject); // удаляем объект
                }
            }
            currentMeds.Clear(); // очищаем список
        }

        public void StartGenerationFromController () {    /* начало генерации таблеток: запуск корутина, активация родительского для таблеток 
                                                      объекта (для отображения на сцене)*/
            StopGeneration();
            parentRectTransform.gameObject.SetActive(true);
            coroGenerationMeds = StartCoroutine(IEnumGenerationMeds());
        }

        public void CountMeds () {
            currentQuantMeds++; // счетчик вышедших на сцену таблеток
            numMeds = GP.totalAmountMeds - currentQuantMeds;
            counterText.text = numMeds + "/" + GP.totalAmountMeds;
        }

        public IEnumerator IEnumGenerationMeds () {// корутин для запуска порций таблеток, чтобы между ними была пауза (появление по очереди)
            while(true) {
                CheckEndGame();
                var temp = Random.Range(1, GP.portionOfMeds);
                for(int i = 1; i <= temp; i++) {
                    if(currentMeds.Count >= GP.totalAmountMeds) {
                        continue;
                    }
                    //var med = Instantiate(prefabMeds);  // создание копий префаба таблетки на поле
                    var med = PoolManager.GetMedFromPull(prefabMeds);
                    med.parentRT = parentRectTransform;
                    med.medsController = this;
                    CountMeds();
                    Get_random_TypeColor(med);
                    med.value = GetValueByTypeColors(med.colorOfMed);
                    med.Setup(GetColorByTypeColors(med.colorOfMed));
                    med.transform.localPosition = CoordOfMed(i, temp, parentRectTransform); // случайно генерировать экземпляры таблетки по всему полю

                    currentMeds.Add(med); // запись таблетки в список
                }
                tempTime = Random.Range(1, GP.timeBetweenPortions);
                yield return new WaitForSeconds(tempTime);
            }
        }

        public Vector3 CoordOfMed (int i, int temp, RectTransform parent) {
            float widthOfField = parent.rect.width; // узнать ширину родителя
            float heightOfField = parent.rect.height; // узнать высоту родителя

            var x = Random.Range(-widthOfField / 2 + (i - 1) * (widthOfField / temp), -widthOfField / 2 + i * (widthOfField / temp));
            var y = Random.Range(heightOfField / 2, -150 + heightOfField / 2);
            var z = 0;
            var v = new Vector3(x, y, z);
            return v;
        }

        public void CheckEndGame () {

            if(currentQuantMeds >= GP.totalAmountMeds) {
                StopGeneration();
                OnMCEndGame?.Invoke("Game Over");
            }
        }

        public void EndGameFromTeenController(string msg) {
            StopGeneration();
        }

        public void ActionFromMed(Meds med) {
            OnMCmedAction?.Invoke(med);
        }
    }
}