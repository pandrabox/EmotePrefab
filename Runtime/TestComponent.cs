
#if UNITY_EDITOR
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.runtime
{
    public class TestComponent : MonoBehaviour
    {
        [SerializeField]
        public PhysBoneSelection ShrinkPhysBone;
        public PhysBoneSelection Test2;
        public PhysBoneSelection Test3;
    }
}
#endif