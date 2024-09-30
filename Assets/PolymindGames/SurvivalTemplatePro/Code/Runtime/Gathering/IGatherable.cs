using Mirror;
using UnityEngine;

namespace PolymindGames.ResourceGathering
{
    public interface IGatherable
    {
        GatherableDefinition Definition { get; }

        float Health { get; }
        float MaxHealth { get; }
        float GatherRadius { get; }
        Vector3 GatherOffset { get; }

        DamageResult HandleDamage(float damage, DamageContext context = default);

        void HandleDamageCmd(float dmg, DamageContext con);
        #region Monobehaviour
        GameObject gameObject { get; }
        Transform transform { get; }
        #endregion
    }
}