using UnityEngine.Events;

namespace PolymindGames
{
    public interface IHealthManager : ICharacterModule
    {
        bool IsAlive { get; }

        float Health { get; }
        float PrevHealth { get; }
        float MaxHealth { get; set; }

        event UnityAction<float, DamageContext> DamageTakenFullContext;

        event UnityAction<float> DamageTaken;
        event UnityAction<float> HealthRestored;

        event UnityAction Death;
        event UnityAction Respawn;


        void RestoreHealth(float healthRestore);
        void ReceiveDamage(float damage);
        void ReceiveDamage(float damage, DamageContext dmgContext);

        void ReceiveDamageCmd(float damage);
        void RestoreHealthCmd(float damage);
    }
}