using System;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Meds: MonoBehaviour, IPointerDownHandler {
    private float speed = 270f;
    public Image img;
    public Sprite pillOne;
    public Sprite pillTwo;
    public Sprite pillThree;
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
        transform.localPosition += Vector3.down * speed * Time.deltaTime; // при каждом кадре уменьшать координату "y" таблетки

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
        var temp = Random.Range(1, 4);
        switch(temp) {
            case 1:
                img.sprite = pillOne;
                break;
            case 2:
                img.sprite = pillTwo;
                break;
            case 3:
                img.sprite = pillThree;
                break;
            default:
                img.sprite = pillOne;
                break;
        }
        //gameObject.transform.SetParent(parentRT); // назначение родителя для созданных экземпляров
        gameObject.transform.localScale = new Vector3(1, 1, 1);  // назначение локального масштаба
        gameObject.transform.localPosition = Vector3.zero;    // назначение локальных координат - относительно от родителя
        gameObject.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 91));
     }

   
}
