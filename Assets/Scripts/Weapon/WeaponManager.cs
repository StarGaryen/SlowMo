using UnityEngine.Networking;
using UnityEngine;

public class WeaponManager : NetworkBehaviour {
    [SerializeField]
    private PlayerWeapon primaryWeapon;
    [SerializeField]
    private Transform weaponHolder;
    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentWeaponGraphics;
    [SerializeField]
    private string weaponLayerName = "Weapon";
    // Use this for initialization
    void Start () {
        EquipWeapon(primaryWeapon);
	}
	
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentWeaponGraphics;
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.weaponGraphics, weaponHolder);
        currentWeaponGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if (currentWeaponGraphics == null)
            Debug.Log("No weapon graphics on weapon :: " + _weaponIns.name);
        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }
}
