using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Meds: MonoBehaviour, IPointerDownHandler {
    private float _speed = GP.medSpeed;
    [SerializeField] private Image _img;
    private int _value;
    private RectTransform _parentRT;
    public TypeColors colorOfMed;
    private float _speedRotation;
    private MedsController _medsController;

    public void Start() {
        _speedRotation = Random.Range(20, 50) * (Random.value > 0.5f ? 1 : -1);
    }
    
    public void Update () {
        transform.localPosition += Vector3.down * _speed * Time.deltaTime; // при каждом кадре уменьшать координату "y" таблетки

        if((transform.localPosition.y < _parentRT.rect.height * (-0.5f))
           || (transform.localPosition.x > _parentRT.rect.width * (0.5f))
           || (transform.localPosition.x < _parentRT.rect.width * (-0.5f))) {
            Destroy(gameObject); // удаление объекта при выходе за рамки экрана
            
        }
        transform.localEulerAngles += new Vector3(0, 0, Time.deltaTime * _speedRotation);
    }

    public void OnPointerDown (PointerEventData eventData) {// реализация синтаксиса интерфейса нажатия
        EventOnClick();
    }

    public void EventOnClick () {
        _medsController.ActionFromMed(this);
        SoundController.Inst.PlaySoundForClick();;
        //Destroy(gameObject); // удаление текущего объекта по клику
        PoolManager.PutMedToPool(this); //  перемещение в пул
    }

    public void SetValueForMed(int val) {
        _value = val;
    }

    public void SetMedsController(MedsController mC)
    {
        _medsController = mC;
    }

    public void SetParentForThisMed(RectTransform rT) {
        _parentRT = rT;
    }

    public int GetValueOfMed () {
        return _value * GP.deltaHP;
    }


    public void Setup (Color col) {
        _img.color = col;
        var spriteNum = Random.Range(1, 4);
        Sprite s = Resources.Load<Sprite>("pill" + spriteNum);
        _img.sprite = s;
        gameObject.transform.localScale = new Vector3(1, 1, 1);  // назначение локального масштаба
        gameObject.transform.localPosition = Vector3.zero;    // назначение локальных координат - относительно от родителя
        gameObject.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 91));
     }

   
}
