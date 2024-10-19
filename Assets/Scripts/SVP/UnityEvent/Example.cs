using UnityEngine;
using UnityEngine.Events;

namespace SVP.Events
{
    public class Example : MonoBehaviour
    {
        [SerializeField] private UnityEvent _testEvent = null;

        [Event("Test")]
        public void TestGroup()
        {

        }

        [Event(0)]
        public void TestOrder0()
        {

        }

        [Event(1)]
        public void TestOrder1()
        {

        }
    }
}