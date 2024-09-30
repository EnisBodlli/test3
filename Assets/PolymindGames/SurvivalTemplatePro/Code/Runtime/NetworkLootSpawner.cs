using UnityEngine;
using Mirror;

public class NetworkLootSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject m_PickupPrefab;

    public static NetworkLootSpawner Instance;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (isServer == false) return;

        GameObject temp = Instantiate(m_PickupPrefab);
        NetworkServer.Spawn(temp);  
    }


    private void Start()
    {
        Instance = this;

    }

    [Server]
    private void DespawnObject(GameObject networkObj)
    {
        if (isServer == false) return;

        Debug.Log("Despawng object");
        NetworkServer.Destroy(networkObj);
    }

    [Command(requiresAuthority = false)]

    public void DestoryNetworkedObjectCMD(GameObject obj)
    {
        Debug.Log("Removing Object CMD");
        if (isServer == false) return;

        DespawnObject(obj);
        Debug.Log("removing object");
       // NetworkServer.Destroy(obj);
        //NetworkLootSpawner.Instance.DespawnObject(gameObject);
        //gameObject.SetActive(false);
    }

    [Client]
    public void TellServerToDestroyObject(GameObject obj)
    {
        DestoryNetworkedObjectCMD(obj);
    }


}
