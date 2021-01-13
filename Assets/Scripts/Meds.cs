using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Meds: MonoBehaviour, IPointerDownHandler {
    private float _speed = GP.medSpeed;
    public Image img;
    public int value;
    public RectTransform parentRT;
    private int delta_HP = 3;
    public TypeColors colorOfMed;
    public float speedRotation;
    public MedsController medsController;

    public void Start() {
        speedRotation = Random.Range(20, 50) * (Random.value > 0.5f ? 1 : -1);
    }
    
    public void Update () {
        transform.localPosition += Vector3.down * _speed * Time.deltaTime; // при каждом кадре уменьшать координату "y" таблетки

        if((transform.localPosition.y < parentRT.rect.height * (-0.5f))
           || (transform.localPosition.x > parentRT.rect.width * (0.5f))
           || (transform.localPosition.x < parentRT.rect.width * (-0.5f))) {
            Destroy(gameObject); // удаление объекта при выходе за рамки экрана
            
        }
        transform.localEulerAngles += new Vector3(0, 0, Time.deltaTime * speedRotation);
    }

    public void OnPointerDown (PointerEventData eventData) {// реализация синтаксиса интерфейса нажатия
        EventOnClick();
    }

    public void EventOnClick () {
        medsController.ActionFromMed(this);
        SoundController.Inst.PlaySoundForClick();;
        //Destroy(gameObject); // удаление текущего объекта по клику
        PoolManager.PutMedToPool(this); //  перемещение в пул
    }

    public int GetValueOfMed () {
        return value * delta_HP;
    }


    public void Setup (Color col) {
        img.color = col;
        var spriteNum = Random.Range(1, 4);
        Sprite s = Resources.Load<Sprite>("pill" + spriteNum);
        img.sprite = s;
        gameObject.transform.localScale = new Vector3(1, 1, 1);  // назначение локального масштаба
        gameObject.transform.localPosition = Vector3.zero;    // назначение локальных координат - относительно от родителя
        gameObject.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 91));
     }

   
}
