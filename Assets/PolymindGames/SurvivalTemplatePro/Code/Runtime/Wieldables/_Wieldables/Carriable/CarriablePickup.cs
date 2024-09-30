using Mirror;
using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/interaction/interactable/demo-interactables")]
    public class CarriablePickup : Interactable
    {
        [SpaceArea]

        [SerializeField, DataReferenceDetails(HasNullElement = false, HasAssetReference = true)]
        [Tooltip("The corresponding carriable definition.")]
        private DataIdReference<CarriableDefinition> m_Definition;


        public override void OnInteract(Character character)
        {
            if (character.TryGetModule(out IWieldableCarriableHandler objectCarry))
            {
                if (objectCarry.TryCarryObject(m_Definition))
                {
                    base.OnInteract(character);
                    DestroyServerObject();
                    //Destroy(gameObject);
                }
            }
        }
        [Command(requiresAuthority = false)]
        protected void DestroyServerObject()
        {
            Debug.Log("Destroying");
            DestroySelf();

        }

        [Server]
        void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void Start()
        {
            Title = "Carry";
            Description = m_Definition.Name;
        }
    }
}