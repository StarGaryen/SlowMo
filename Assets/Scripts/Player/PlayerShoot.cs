using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {
    private const string PlayerTag = "Player";

    private PlayerWeapon weapon;
    private WeaponManager weaponManager;
    
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;


    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot :: No Camera Ref");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        weapon = weaponManager.GetCurrentWeapon();
        if (weapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / weapon.fireRate);
            }else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }        
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    [ClientRpc]
    void RpcDoShootEffect()
    {
        if (weaponManager == null)
        {
            weaponManager = GetComponent<WeaponManager>();
        }
        else
        {
            weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        }
           
       

    }

    [Client]
    void Shoot()
    {
        CmdOnShoot();
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask))
        {
            if (_hit.transform.tag == PlayerTag)
            {
                CmdPlayerShot(_hit.collider.name,weapon.damage);
            }
            CmdOnHit(_hit.point, _hit.normal);
        }
    }

    [Command]
    void CmdPlayerShot(string _ID,int _damage)
    {
        Debug.Log(_ID + "has been shoot");
        PlayerManager _player =GameManager.GetPlayer(_ID);
        _player.RpcTakeDamage(_damage);
    }

    [Command]
    void CmdOnHit(Vector3 _position, Vector3 _normal)
    {
        RpcDoHitEffect(_position, _normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 _position, Vector3 _normal)
    {
        if (weaponManager == null)
        {
            weaponManager = GetComponent<WeaponManager>();
        }
        else
        {
            GameObject hitEffectRef =(GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _position, Quaternion.LookRotation(_normal));
            Destroy(hitEffectRef, 2f);
        }



    }

    public void OnDisable()
    {
        CancelInvoke();
    }
}
