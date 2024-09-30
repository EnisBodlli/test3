using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    [AddComponentMenu("PolymindGames/Wieldables/Firearms/Ammo/Infinite Ammo")]
    public class FirearmInfiniteAmmo : FirearmAmmoBehaviour
    {
        public override int RemoveAmmo(int amount) => amount;
        public override int AddAmmo(int amount) => amount;
        public override int GetAmmoCount() => int.MaxValue;
    }
}