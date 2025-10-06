using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private bool[] weaponAvailable;
    private int currentWeaponIndex = 0;

    [Header("Other Scripts")]
    [SerializeField] private PistolController playerShooting;
    [SerializeField] private PlayerMelee playerMelee;

    void Start()
    {
        if (!weaponAvailable[currentWeaponIndex])
            SelectNextAvailableWeapon(1);
        UpdateWeapon();
    }

    void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            SelectNextAvailableWeapon(1);
            UpdateWeapon();
        }
        else if (scroll < 0f)
        {
            SelectNextAvailableWeapon(-1);
            UpdateWeapon();
        }
    }

    private void SelectNextAvailableWeapon(int direction)
    {
        int attempts = 0;
        do
        {
            currentWeaponIndex += direction;

            if (currentWeaponIndex >= weapons.Length) currentWeaponIndex = 0;
            if (currentWeaponIndex < 0) currentWeaponIndex = weapons.Length - 1;

            attempts++;
            if (attempts > weapons.Length)
            {
                Debug.LogWarning("Нет доступного оружия!");
                return;
            }

        } while (!weaponAvailable[currentWeaponIndex]);
    }

    private void UpdateWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
        }

        if (currentWeaponIndex == 0)
        {
            playerMelee.canMelee = true;
            playerShooting.canRange = false;
        }
        else if (currentWeaponIndex == 1)
        {
            playerMelee.canMelee = false;
            playerShooting.canRange = true;
        }
    }

    public void RemoveWeaponAccess()
    {
        weaponAvailable[currentWeaponIndex] = false;
        SelectNextAvailableWeapon(1);
        UpdateWeapon();


    }

    public void GrantWeaponAccess(int index)
    {
        if (index < 0 || index >= weaponAvailable.Length)
        {
            Debug.LogWarning("Неверный индекс оружия!");
            return;
        }

        weaponAvailable[index] = true;
    }
}
