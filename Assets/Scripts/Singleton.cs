using UnityEngine;


namespace Seka {

    public class Singleton<T>:MonoBehaviour where T:MonoBehaviour {

        private static T _inst;

        public static T Inst {
            get {
                if (_inst == null) {
                    _inst = FindObjectOfType<T>();
                }
                if(_inst == null) {
                    var singleton = new GameObject("SINGLETON"+typeof(T));
                    _inst = singleton.AddComponent<T>();
                    DontDestroyOnLoad(_inst);
                }
                return _inst;
            }
           
        }
    }
}
