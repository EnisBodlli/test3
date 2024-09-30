using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    [AddComponentMenu("PolymindGames/Wieldables/Firearms/Recoil/No Recoil")]
    public class FirearmNoRecoil : FirearmRecoilBehaviour
    {
        public override void DoRecoil(bool isAiming, float heatValue, float triggerValue)
        {
        }
    }
}
