using UnityEngine;
using UnityEngine.VFX;

public class SMG : MonoBehaviour {
    // Start is called before the first frame update

    [Header("Links")]
    public Camera fpCam;

    [Header("Damage")]
    public float damage = 10f;
    public float efRange = 100f;

    [Header("Ammo")]
    public int mag = 10;
    public int chamber = 1;

    [Header("FireRate")]
    public float fireRate = 0f;
    //Dropdowns for scripts are enumerators (in the case of unity) here I am using one for selecting the "fireMode" of the weapon. (both the enum and the var refrencing it must be the same privacy [IE: both public])
    public enum mode { Semi, Automatic };
    public mode fireMode;
    private float shotTime;

    [Header("Particles")]
    public VisualEffect fxSystem;
    public GameObject bulletImpact;
    public float particalDeath = 1f;

    // Update is called once per frame
    void Update() {
        //Changes if the function will repeat on run once depending on the fireMode selected
        if (fireMode == mode.Semi) {
            if (Input.GetButtonDown("Fire1")) {

                Shoot();
            }
        }
        else if (fireMode == mode.Automatic) {
            if (Input.GetButton("Fire1")) {
                Shoot();
            }
        }

        if (Input.GetButton("Fire1")) {
        //Space for any function that should be run for the duration of the "trigger pull" regardless of fire mode [for instance the litteral trigger pull].



        } else {
        //Space for basically the oppsite [For instance the litteral opposite of a trigger pull].

        }


        if (Input.GetKeyDown(KeyCode.R)) {
            Reload();
        }
    }

    void Shoot() {
        //Checks to see if the gun is able to be fired again (dictated by firerate)
        if (Time.time - shotTime > 1 / fireRate) {
            //Resets time for fireRate
            shotTime = Time.time;

            //Particle Handler
            fxSystem.Play();

            //Hit detection
            RaycastHit hit;
            if (Physics.Raycast(fpCam.transform.position, fpCam.transform.forward, out hit, efRange)) {
                Debug.Log(hit.transform.name);

                //Particle Handler
                //creates a new game object (bulletImpact) at the location of impact and rotates it to the normal surface of the impacted object. (basically perpendicular relative to the surface being hit)
                GameObject impactTemp = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactTemp, 1f);
            }

        }
    }

    void Reload() {
        Debug.Log("Reload");
    }
}
