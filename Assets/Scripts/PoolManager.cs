using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {
    private static Dictionary<string, Stack<GameObject>> _poolsDict;
    private static Dictionary<string, Stack<Meds>> _poolsMedDict;
    private static Transform parentForDeactivatedGO;

    public static void Init(Transform pooledObjContainer) {
        parentForDeactivatedGO = pooledObjContainer;
        _poolsDict = new Dictionary<string, Stack<GameObject>>();
        _poolsMedDict = new Dictionary<string, Stack<Meds>> ();
    }

    public static Meds GetMedFromPull(Meds medPrefab) { //  получение объекта из пула по имени префаба
        if (!_poolsMedDict.ContainsKey(medPrefab.name)) {
            _poolsMedDict[medPrefab.name] = new Stack<Meds>();
        }

        Meds result;
        if (_poolsMedDict[medPrefab.name].Count > 0) {
            result = _poolsMedDict[medPrefab.name].Pop();
            return result;
        }

        result = Instantiate(medPrefab, parentForDeactivatedGO);
        result.name = medPrefab.name;
        return result;
    }

    public static void PutMedToPool(Meds target) {
        _poolsMedDict[target.name].Push(target);
        //target.transform.parent = parentForDeactivatedGO;
        target.gameObject.SetActive(false);
    }


    public static GameObject GetGOFromPull(GameObject prefab) { //  получение объекта из пула по имени префаба
        if (!_poolsDict.ContainsKey(prefab.name)) {
            _poolsDict[prefab.name] = new Stack<GameObject>();
        }

        GameObject result;
        if (_poolsDict[prefab.name].Count > 0) {
            result = _poolsDict[prefab.name].Pop();
            return result;
        }

        result = Instantiate(prefab);
        result.name = prefab.name;
        return result;
    }

    public static void PutGOToPool(GameObject target) {
        _poolsDict[target.name].Push(target);
        //target.transform.parent = parentForDeactivatedGO;
        target.SetActive(false);
    }
}
