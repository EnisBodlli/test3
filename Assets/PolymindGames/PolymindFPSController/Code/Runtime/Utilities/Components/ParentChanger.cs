using UnityEngine;

namespace PolymindGames
{
    public class ParentChanger : MonoBehaviour
    {
        public void ChangeParentResetPosition(Transform parent) => transform.SetParent(parent, false);
        public void ChangeParentWorldPositionStays(Transform parent) => transform.SetParent(parent, true);
    }
}
