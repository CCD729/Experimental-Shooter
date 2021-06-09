using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] private float dropForce = 3f;
    [SerializeField] private float dropForceUpward = 3f;
    public GameObject[] pickupablesWeapon;
    public GameObject[] pickupablesWeaponPOV;

    public GameObject player;
    public ShootingScript shootingScript;
    public LevelSceneManager levelSceneManager;

    private bool matchfound = false;
    private bool matchfoundPOV = false;

    public void Pickup(WeaponInfo weaponInfo, GameObject pickupObj)
    {
        //TODO NEXT: port customized weaponInfo in the future
        int weaponid = weaponInfo.weaponID;
        GameObject targetWeapon;
        GameObject targetWeaponPOV;
        foreach (GameObject weapon in pickupablesWeapon){
            if(weapon.GetComponent<WeaponInfo>().weaponID == weaponid)
            {
                matchfound = true;
                targetWeapon = weapon;
                foreach (GameObject weaponPOV in pickupablesWeaponPOV)
                {
                    if (weaponPOV.GetComponent<WeaponInfo>().weaponID == weaponid)
                    {
                        matchfoundPOV = true;
                        targetWeaponPOV = weaponPOV;
                        //Destroy(pickupObj);
                        pickupObj.GetComponent<Rigidbody>().isKinematic = true;
                        pickupObj.GetComponent<BoxCollider>().isTrigger = true;
                        GameObject instanceWeapon = Instantiate(targetWeapon);
                        GameObject instanceWeaponPOV = Instantiate(targetWeaponPOV);
                        instanceWeapon.transform.SetParent(shootingScript.weaponContainer.transform);
                        instanceWeapon.transform.localPosition = Vector3.zero;
                        instanceWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        instanceWeapon.transform.localScale = Vector3.one;
                        instanceWeaponPOV.transform.SetParent(shootingScript.weaponContainerPOV.transform);
                        instanceWeaponPOV.transform.localPosition = Vector3.zero;
                        instanceWeaponPOV.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        instanceWeaponPOV.transform.localScale = Vector3.one;
                        if (shootingScript.weaponEquipped)
                        {
                            if (!shootingScript.weaponFull)
                            {
                                pickupObj.transform.SetParent(shootingScript.weaponBackDisplayContainer.transform);
                                pickupObj.transform.localPosition = Vector3.zero;
                                pickupObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                                pickupObj.transform.localScale = Vector3.one;
                                shootingScript.weaponFull = true;
                                shootingScript.currentWeaponSlot = 1;
                                shootingScript.secondaryWeapon = instanceWeapon.gameObject;
                                shootingScript.secondaryWeaponPOV = instanceWeaponPOV.gameObject;
                                shootingScript.currentWeapon = shootingScript.secondaryWeapon;
                                shootingScript.currentWeaponPOV = shootingScript.secondaryWeaponPOV;
                                shootingScript.secondaryWeaponBackDisplay = pickupObj;
                                shootingScript.secondaryWeaponBackDisplay.SetActive(false);
                                shootingScript.primaryWeaponBackDisplay.SetActive(true);
                                shootingScript.firePoint = instanceWeapon.transform.Find("FirePoint");
                                shootingScript.UpdateWeaponInfo();
                            }
                            else
                            {
                                //TODO: Switch weapon and drop equipped one out
                                //Delete old
                                Destroy(shootingScript.currentWeapon);
                                Destroy(shootingScript.currentWeaponPOV);

                                pickupObj.transform.SetParent(shootingScript.weaponBackDisplayContainer.transform);
                                pickupObj.transform.localPosition = Vector3.zero;
                                pickupObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                                pickupObj.transform.localScale = Vector3.one;
                                if (shootingScript.currentWeaponSlot == 0)
                                {
                                    shootingScript.primaryWeaponBackDisplay.transform.SetParent(null);
                                    DropPhysics(shootingScript.primaryWeaponBackDisplay);
                                    //Add new
                                    shootingScript.primaryWeapon = instanceWeapon.gameObject;
                                    shootingScript.primaryWeaponPOV = instanceWeaponPOV.gameObject;
                                    shootingScript.currentWeapon = shootingScript.primaryWeapon;
                                    shootingScript.currentWeaponPOV = shootingScript.primaryWeaponPOV;
                                    shootingScript.primaryWeaponBackDisplay = pickupObj;
                                }
                                else
                                {
                                    shootingScript.secondaryWeaponBackDisplay.transform.SetParent(null);
                                    DropPhysics(shootingScript.secondaryWeaponBackDisplay);
                                    //Add new
                                    shootingScript.secondaryWeapon = instanceWeapon.gameObject;
                                    shootingScript.secondaryWeaponPOV = instanceWeaponPOV.gameObject;
                                    shootingScript.currentWeapon = shootingScript.secondaryWeapon;
                                    shootingScript.currentWeaponPOV = shootingScript.secondaryWeaponPOV;
                                    shootingScript.secondaryWeaponBackDisplay = pickupObj;
                                }
                            }
                        }
                        else
                        {
                            pickupObj.transform.SetParent(shootingScript.weaponBackDisplayContainer.transform);
                            pickupObj.transform.localPosition = Vector3.zero;
                            pickupObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                            pickupObj.transform.localScale = Vector3.one;
                            shootingScript.weaponEquipped = true;
                            shootingScript.primaryWeapon = instanceWeapon.gameObject;
                            shootingScript.primaryWeaponPOV = instanceWeaponPOV.gameObject;
                            shootingScript.currentWeapon = shootingScript.primaryWeapon;
                            shootingScript.currentWeaponPOV = shootingScript.primaryWeaponPOV;
                            shootingScript.primaryWeaponBackDisplay = pickupObj;
                            shootingScript.primaryWeaponBackDisplay.SetActive(false);
                        }
                        shootingScript.firePoint = instanceWeapon.transform.Find("FirePoint");
                        shootingScript.UpdateWeaponInfo();
                        levelSceneManager.UpdateWeaponInfo();
                    }
                }
            }
        }
        if (!matchfound || !matchfoundPOV)
        {
            Debug.Log("Error: No match found with pickup object");
        }
    }

    /// <summary>
    /// Drops a item at hand (physics only)
    /// </summary>
    /// <param name="dropObj"></param>
    public void DropPhysics(GameObject dropObj)
    {
        dropObj.GetComponent<Rigidbody>().isKinematic = false;
        dropObj.GetComponent<BoxCollider>().isTrigger = false;
        dropObj.GetComponent<Rigidbody>().velocity = player.GetComponent<CharacterController>().velocity;
        dropObj.GetComponent<Rigidbody>().AddForce(shootingScript.playerCam.transform.forward * dropForce, ForceMode.Impulse);
        dropObj.GetComponent<Rigidbody>().AddForce(shootingScript.playerCam.transform.up * dropForceUpward, ForceMode.Impulse);
    }
}