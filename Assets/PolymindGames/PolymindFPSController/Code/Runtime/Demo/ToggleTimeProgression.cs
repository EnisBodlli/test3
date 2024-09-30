using PolymindGames.WorldManagement;
using UnityEngine;

namespace PolymindGames.Demo
{
    public class ToggleTimeProgression : MonoBehaviour
    {
        public void ToggleTimeProgress()
        {
            WorldManagerBase.Instance.TimeProgressionEnabled = !WorldManagerBase.Instance.TimeProgressionEnabled;
        }
    }
}