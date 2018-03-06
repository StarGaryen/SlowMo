using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour {

    #region Health Varialbes
    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    private int currentHealth;
    #endregion

    #region Death Variables
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    #endregion

    #region On Death Disable

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled; // to keep record of  disableOnDeath array's componnets original status 
    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    #endregion

    #region  On death Special Effects

    [SerializeField]
    private GameObject explosionPrefab;
    private GameObject explosionInstance;

    #endregion

    private bool isFirstSetup = true;

    public void SetupPlayer()
    {
        // switch Camera for local player died
        if (isLocalPlayer)
        {
            GameManager.singelton.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
            
        }
        CmdBrodcastNewPlayerSetup();

    }

    [Command]
    private void CmdBrodcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {

        #region Saving disableOnDeath array's componnets original status
        if (isFirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            isFirstSetup = false;
        }
        

        #endregion

        SetDefaults();
    }

    public void SetDefaults()
    {
        //Reset Health
        isDead = false;
        currentHealth = maxHealth;

        //Enable Components
        for(int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        //Enable GameObjects
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        //Enable Collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;
        }   

    }


    public int GetCurrentHealth()
    {
        return currentHealth;
    }


    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
            return;
        currentHealth -= _amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
	

	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(gameObject.name+ " commited sucide.");
            RpcTakeDamage(9999);
        }
		
	}

    // On Player Death
    private void Die()
    {
       
        isDead = true;
        // Disable Components 
        for(int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //Disable GameObjects
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }
        //Disable Collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        // Spawn a death Effect
            //todo add object pool functionality
        explosionInstance = Instantiate(explosionPrefab,transform.position,transform.rotation);
        Destroy(explosionInstance, 2f);

        // switch Camera if local player died
        if(isLocalPlayer)
        {
            GameManager.singelton.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }



        //respawn
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        Debug.Log("Waiting for respawndelay");
        // wait for respawn Delay
        yield return new WaitForSeconds(GameManager.singelton.matchSetting.RespawnDelay);

        
        //Move to respawn point        
        Transform _startPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _startPoint.position;
        transform.rotation = _startPoint.rotation;       

        SetupPlayer();
    }

}
