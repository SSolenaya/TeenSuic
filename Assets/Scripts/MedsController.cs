using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Seka;

namespace Assets.Scripts {
    public enum TypeColors {
        c1 = 1,
        c2,
        c3,
        c4,
        c5
    }

    public class MedsController: Singleton<MedsController> {

        public Meds prefabMeds; // ссылка на префаб таблетки
        public Text counterText;

        public List<Meds> currentMeds = new List<Meds>();
        public RectTransform parentRectTransform; // ссылка на родителя таблетки

        private int quantityMeds = GP.totalamountMeds; // количество таблеток
        public int portionOfMeds = GP.portionOfMeds; // порция таблеток, выходящая на экран
        public int numMeds;// переменная для отображения обратного счетчика таблеток
        private float timeBetweenPortions = 2f; // время между появлением порций
        public float tempTime;
        public int currentQuantMeds; // переменная для счетчика таблеток

        public int value1;
        public int value2;
        public int value3;
        public int value4;
        public int value5;

        public Color color1;
        public Color color2;
        public Color color3;
        public Color color4;
        public Color color5;

        public Coroutine coroGenerationMeds;

        void Awake () {
            SetValueForType();
            numMeds = quantityMeds;
        }

        void Start() {
            counterText.text = numMeds + "/" + quantityMeds;
        }

        /*public void Reset() {
            numMeds = quantityMeds;
        }*/

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
            }

            return color1;
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
            }

            return value1;
        }

        public void StopGeneration () {
            if(coroGenerationMeds != null) {
                StopCoroutine(coroGenerationMeds);
                DeleteCurrentMeds();
            }
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

        public void CountMeds() {
            currentQuantMeds ++; // счетчик вышедших на сцену таблеток
            numMeds = quantityMeds - currentQuantMeds;
            counterText.text = numMeds + "/" + quantityMeds;
        }

        public IEnumerator IEnumGenerationMeds () {// корутин для запуска порций таблеток, чтобы между ними была пауза (появление по очереди)
            while(true) {
                TeenController.Inst.CheckEndGame();
                var temp = Random.Range(1, portionOfMeds);
                for(int i = 1; i <= temp; i++) {
                    if (currentMeds.Count >= GP.totalamountMeds) {
                        continue;
                    }
                    var med = Instantiate(prefabMeds);  // создание копий префаба таблетки на поле
                    med.parentRT = parentRectTransform;
                    CountMeds();
                    Get_random_TypeColor(med);
                    med.Setup(GetColorByTypeColors(med.colorOfMed));
                    med.value = GetValueByTypeColors(med.colorOfMed);
                    med.transform.SetParent(parentRectTransform); // назначение родителя для созданных экземпляров
                    med.transform.localScale = new Vector3(1, 1, 1);  // назначение локального масштаба
                    med.transform.localPosition = Vector3.zero;    // назначение локальных координат - относительно от родителя
                    med.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 91));

                    med.transform.localPosition = CoordOfMed(i, temp, parentRectTransform); // случайно генерировать экземпляры таблетки по всему полю

                    currentMeds.Add(med); // запись таблетки в список
                }
                TeenController.Inst.CheckEndGame();
                tempTime = Random.Range(1, timeBetweenPortions);
                yield return new WaitForSeconds(tempTime);
            }
        }

        public Vector3 CoordOfMed (int i, int temp, RectTransform parent) {
            float widthOfField = parent.rect.width; // узнать ширину родителя
            float heightOfField = parent.rect.height; // узнать высоту родителя

            var x = Random.Range(-widthOfField / 2 + (i - 1) * (widthOfField / temp), -widthOfField / 2 + i * (widthOfField / temp));
            var y = Random.Range(heightOfField /2, -150+heightOfField /2);
            var z = 0;
            var v = new Vector3(x, y, z);
            return v;
        }
    }
}