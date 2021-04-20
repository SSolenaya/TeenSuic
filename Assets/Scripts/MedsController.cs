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

        [SerializeField] private Meds _prefabMeds; // ссылка на префаб таблетки
        [SerializeField] private Text _counterTxt;    //  текст счетчика упавших таблеток
        private List<Meds> _currentMedsList = new List<Meds>();   
        [SerializeField] private RectTransform _medParentRT; // ссылка на родителя таблетки
        private int _medsLeft;// переменная для отображения обратного счетчика таблеток
        private int _currentQuantMeds; // переменная для счетчика таблеток
        private int _value1;
        private int _value2;
        private int _value3;
        private int _value4;
        private int _value5;
        private Color _color1;
        private Color _color2;
        private Color _color3;
        private Color _color4;
        private Color _color5;
        private Coroutine _coroGenerationMeds;


        public delegate void MCEndGame (string msg); // делегат

        public static event MCEndGame OnMCEndGame; // событие

        public delegate void MCmedAction (Meds med); // делегат

        public static event MCmedAction OnMCmedAction; // событие

        void Awake () {
            SetValueForType();
            _medsLeft = GP.totalAmountMeds;
            PoolManager.Init(_medParentRT);
            TeenController.OnEndGame += EndGameFromTeenController;
        }

        void OnDisable() {
            TeenController.OnEndGame -= EndGameFromTeenController;
        }

        void Start () {
            _counterTxt.text = _medsLeft + "/" + GP.totalAmountMeds;
            StartGenerationFromController();
        }

        public void SetValueForType () {

            _value1 = Random.value < 0.5 ? -1 : 1; // присвоение случайного значения из 2, для реализации "хорошая/плохая таблетка"
            _value2 = Random.value < 0.5 ? -1 : 1;
            _value3 = Random.value < 0.5 ? -1 : 1;
            _value4 = Random.value < 0.5 ? -1 : 1;
            _value5 = Random.value < 0.5 ? -1 : 1;

            _color1 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            _color2 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            _color3 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            _color4 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            _color5 = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        }

        public TypeColors GetRandomTypeColor (Meds m) {
            m.colorOfMed = (TypeColors)Random.Range(1, 6);
            return m.colorOfMed;
        }

        public Color GetColorByTypeColors (TypeColors tc) {
            switch(tc) {
                case TypeColors.c1:
                    return _color1;
                case TypeColors.c2:
                    return _color2;
                case TypeColors.c3:
                    return _color3;
                case TypeColors.c4:
                    return _color4;
                case TypeColors.c5:
                    return _color5;
                default:
                    return _color1;
            }
        }

        public int GetValueByTypeColors (TypeColors tc) {
            switch(tc) {
                case TypeColors.c1:
                    return _value1;
                case TypeColors.c2:
                    return _value2;
                case TypeColors.c3:
                    return _value3;
                case TypeColors.c4:
                    return _value4;
                case TypeColors.c5:
                    return _value5;
                default:
                    return _value1;
            }
        }

        public void StopGeneration () {
            if(_coroGenerationMeds != null) {
                StopCoroutine(_coroGenerationMeds);
            }
            DeleteCurrentMeds();
            _medParentRT.gameObject.SetActive(false);
            _coroGenerationMeds = null;
        }

        public void DeleteCurrentMeds () {// функция для удаления текущих таблеток при начале новой игры
            foreach(var m in _currentMedsList) {// перебираем все элементы списка
                if(m != null) {// если они не нулевые
                    Destroy(m.gameObject); // удаляем объект
                }
            }
            _currentMedsList.Clear(); // очищаем список
        }

        public void StartGenerationFromController () {    /* начало генерации таблеток: запуск корутина, активация родительского для таблеток 
                                                      объекта (для отображения на сцене)*/
            StopGeneration();
            _medParentRT.gameObject.SetActive(true);
            _coroGenerationMeds = StartCoroutine(IEnumGenerationMeds());
        }

        public void CountMeds () {
            _currentQuantMeds++; // счетчик вышедших на сцену таблеток
            _medsLeft = GP.totalAmountMeds - _currentQuantMeds;
            _counterTxt.text = _medsLeft + "/" + GP.totalAmountMeds;
        }

        public IEnumerator IEnumGenerationMeds () {// корутин для запуска порций таблеток, чтобы между ними была пауза (появление по очереди)
            while(true) {
                CheckEndGame();
                var currentPortionSize = Random.Range(1, GP.portionOfMeds);
                for(int i = 1; i <= currentPortionSize; i++) {
                    if(_currentMedsList.Count >= GP.totalAmountMeds) {
                        continue;
                    }
                    var med = PoolManager.GetMedFromPull(_prefabMeds); // создание копий префаба таблетки на поле
                    med.SetParentForThisMed(_medParentRT);
                    med.SetMedsController(this);
                    CountMeds();
                    GetRandomTypeColor(med);
                    med.SetValueForMed(GetValueByTypeColors(med.colorOfMed));
                    med.Setup(GetColorByTypeColors(med.colorOfMed));
                    med.transform.localPosition = CoordOfMed(i, currentPortionSize, _medParentRT); // случайно генерировать экземпляры таблетки по всему полю

                    _currentMedsList.Add(med); // запись таблетки в список
                }
                float tempTime = Random.Range(1f, GP.timeBetweenPortions);
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

            if(_currentQuantMeds >= GP.totalAmountMeds) {
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