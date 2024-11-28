using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public BaseWeapon CurrentSelectedWeapon;

	public GameObject[] WeaponObjectsWithWeaponBehaviors;

	void Start()
	{
		SelectWeapon(0);
	}

	void Update()
	{
		if (CurrentSelectedWeapon != null)
		{
			CurrentSelectedWeapon.Mouse0(Input.GetKey(KeyCode.Mouse0));
			CurrentSelectedWeapon.Mouse1(Input.GetKey(KeyCode.Mouse1));

			if (Input.GetKeyDown(KeyCode.R)) CurrentSelectedWeapon.RKeyDown();
		}
	}

	public void SelectWeapon(int itemIndex)
	{
		for (int i = 0; i < WeaponObjectsWithWeaponBehaviors.Length; i++)
		{
			if (i == itemIndex)
			{
				WeaponObjectsWithWeaponBehaviors[i].SetActive(true);
				CurrentSelectedWeapon = WeaponObjectsWithWeaponBehaviors[i].GetComponent<BaseWeapon>();
			}
			else
			{
				WeaponObjectsWithWeaponBehaviors[i].SetActive(false);

			}
		}
	}
}