using UnityEngine;
namespace Seka {

    public class Singleton<T>: MonoBehaviour where T : MonoBehaviour {

        private static T _inst;

        public static T Inst {
            get {
                if(_inst == null) {
                    _inst = FindObjectOfType<T>();
                    DontDestroyOnLoad(_inst.gameObject);
                }
                             
                return _inst;
            }

        }
    }
}
