using UnityEngine;
using EZCameraShake;
using TMPro;

public class GunSystem : MonoBehaviour
{
    [Header("Links")]
    public PlayerRespawn playerRespawn;

    [Header("Gun stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, timeBetweenShots;
    private float nextShoot;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools 
    bool shooting;

    [Header("Reference")]
    public Camera fpsCam;
    public Transform attackPoint;
    RaycastHit rayHit;
    public LayerMask IgnoreLayer;

    [Header("Graphics")]
    public GameObject muzzleFlash, bulletObject;
    public float camShakeMagnitude, camShakeRoughness, camShakeDuration;
    public TextMeshProUGUI text;

    [Header("Sound")]
    public AudioClip[] impact;
    public AudioSource audioSource;

    Animator anim;


    private void Awake()
    {
        bulletsLeft = magazineSize;

        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        MyInput();

        //SetText
        if (text)
        {
            text.SetText(bulletsLeft.ToString());
        }
    }
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Shoot
        if (shooting && bulletsLeft > 0 && playerRespawn.GameIsPaused == false)
        {
            if (Time.time > nextShoot)
            {
                nextShoot = Time.time + timeBetweenShooting;

                bulletsShot = bulletsPerTap;
                Shoot();
            }
        }
    }
    private void Shoot()
    {
        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + fpsCam.transform.TransformDirection(new Vector3(x, y, 0));


        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, ~IgnoreLayer))
        {
            GameObject tempBullet = Instantiate(bulletObject, attackPoint.position, Quaternion.identity);
            tempBullet.GetComponent<Bullet>().damage = damage;
            tempBullet.transform.LookAt(rayHit.point);
        }
        else
        {
            GameObject tempBullet = Instantiate(bulletObject, attackPoint.position, Quaternion.identity);
            tempBullet.GetComponent<Bullet>().damage = damage;
            tempBullet.transform.LookAt(direction * 1000);
        }

        bulletsLeft--;
        bulletsShot--;

        if (bulletsShot == 0)
        {
            //Graphics
            muzzleFlash.SetActive(true);

            //ShakeCamera
            CameraShaker.Instance.ShakeOnce(camShakeMagnitude, camShakeRoughness, 0.1f, camShakeDuration);

            //Gun Animation
            anim.CrossFade("Shot", 0.001f);
        }

        audioSource.PlayOneShot(impact[Random.Range(0, impact.Length)], 0.25f);


        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

}