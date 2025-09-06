using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;


using TMPro;


public class GameManager : MonoBehaviour
{
  
    public static GameManager Instance { get; private set; }

    [Header("Skor")]
    public int score = 0;

    
    public TMP_Text scoreText;
   

    public bool L1 = false;
    public bool l3 = false;

    public GameObject Cam1;
    public GameObject Cam2;
    public GameObject Cam3;

    public GameObject Cam2UI;

    public GameObject Bomb;
    public Transform BombLocation;
    public Transform BombRotation;
    public GameObject Plane;
    public float atishizi;
    public float bombCooldown = 2f;
    private float lastBombTime = -999f;

    public Transform Enemy;

    public AirplaneController AirplaneController;

    public Vector3 planeSpeed;

    bool RightShoulderPressed = false;

   
    public Volume cockpitVolume;
    public Volume outsideVolume;

    public AudioClip Explosion;
    private AudioSource audioSource;

    public float Health;





    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       
    }
   

    void Start()
    {
        Cam2UI.SetActive(false);
        QualitySettings.vSyncCount = 1;

        cockpitVolume.gameObject.SetActive(true);
        outsideVolume.gameObject.SetActive(false);

       
        UpdateScoreUI();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyUp(KeyCode.JoystickButton4) || Input.GetKeyUp(KeyCode.Alpha1))
        {
            L1 = false;
            l3 = false;
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            L1 = true;
            l3 = false;
        }

        if (Mouse.current != null && Mouse.current.middleButton.isPressed || Gamepad.current != null && Gamepad.current.rightStickButton.isPressed)
        {
            L1 = false;
            l3 = true;
        }
        else if (!L1)
        {
            l3 = false;
            L1 = false;
        }

        if (L1)
        {
            Cam2.SetActive(true);
            Cam1.SetActive(false);
            Cam3.SetActive(false);
            Cam2UI.SetActive(true);

            cockpitVolume.gameObject.SetActive(false);
            outsideVolume.gameObject.SetActive(true);
        }

        if (!L1)
        {
            Cam1.SetActive(true);
            Cam2.SetActive(false);
            Cam3.SetActive(false);
            Cam2UI.SetActive(false);

            cockpitVolume.gameObject.SetActive(true);
            outsideVolume.gameObject.SetActive(false);
        }

        if (l3)
        {
            Cam3.SetActive(true);
            Cam2.SetActive(false);
            Cam1.SetActive(false);
        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                RightShoulderPressed = true;
            }
            else { RightShoulderPressed = false; }
        }

        Rigidbody rb2 = Plane.GetComponent<Rigidbody>();
        float speed2 = rb2.linearVelocity.magnitude;

        float egim = AirplaneController.Pitch;
        float flap3 = AirplaneController.Flap;

        bool IsGround = AirplaneController.onGround;

        planeSpeed = Plane.GetComponent<Rigidbody>().linearVelocity;

        bool dropInput = (Input.GetKeyDown(KeyCode.V) || RightShoulderPressed);

        if (dropInput && egim < 0.2f && !IsGround && Time.time >= lastBombTime + bombCooldown)
        {
            GameObject instantiatedBomb = Instantiate(Bomb, BombLocation.position, BombRotation.rotation);

            Rigidbody rb = instantiatedBomb.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Plane.GetComponent<Rigidbody>().linearVelocity;
            }

            lastBombTime = Time.time; // cooldown resetle
        }
    }

    // =========[ YENİ: Skor API ]=========
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
        // Gerekirse burada ses/animasyon/feedback tetikle
    }

    public void OnEnemyAircraftEliminated()
    {
        AddScore(10); // hava hedefi +10
        Debug.Log("[GameManager] Enemy aircraft destroyed. +10  | Total: " + score);
    }

    public void OnHSSEliminated()
    {
        AddScore(20); // HSS +20
        Debug.Log("[GameManager] HSS destroyed. +20 | Total: " + score);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "PUAN: " + score;
        }
        else
        {
            // UI atamayı unuttuysan logla:
            // Debug.LogWarning("GameManager: scoreText atanmadı. (UI’ya yazılamadı)");
        }
    }
    // =====================================

    public void ExplosionSound()
    {
        if (Explosion != null)
        {
            audioSource.PlayOneShot(Explosion);
        }
    }
}
