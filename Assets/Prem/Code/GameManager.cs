using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Settings")]
    public GameObject player;
    public FirstPersonController fpsController;
    public Gun gun;

    [Header("Ammo Pickup Settings")]
    public GameObject ammoPickupPrefab;
    public List<Transform> ammoSpawnPoints = new List<Transform>();
    public GameObject mathPanel;
    public TMP_Text mathProblemText;
    public TMP_InputField answerInput;
    public int minAmmoReward = 5;
    public int maxAmmoReward = 15;

    [Header("Slow Motion Settings")]
    public float slowMotionFactor = 0.2f;
    public float slowMotionDuration = 5f;
    private bool isSlowMotion = false;
    private float originalFixedDeltaTime;

    [Header("Zombie Spawn Settings")]
    public GameObject zombiePrefab;
    public List<ZombieSpawnPoint> zombieSpawnPoints = new List<ZombieSpawnPoint>();
    public int maxZombies = 20;
    public float spawnCheckInterval = 3f;

    private int currentZombies = 0;
    private List<GameObject> activeZombies = new List<GameObject>();
    private bool isSpawning = false;

    [Header("UI Settings")]
    public TMP_Text ammoText;
    public TMP_Text healthText;
    public Image crosshair;

    private int correctAnswer;
    private int currentAmmoReward;
    private bool isMathPanelActive = false;

    [System.Serializable]
    public class ZombieSpawnPoint
    {
        public Transform transform;
        public float spawnRadius = 10f;
        public float activationDistance = 15f;
        public int zombieCount = 5;
        public float spawnDelay = 1f;
        [HideInInspector] public bool isActive = false;
        [HideInInspector] public bool hasFinishedSpawning = false;
        [HideInInspector] public int currentZombiesSpawned = 0;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Start()
    {
        if (mathPanel != null)
            mathPanel.SetActive(false);

        // ตรวจสอบการตั้งค่าที่สำคัญ
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (fpsController == null && player != null)
            fpsController = player.GetComponent<FirstPersonController>();

        if (gun == null && player != null)
            gun = player.GetComponentInChildren<Gun>();

        SpawnInitialAmmo();
        StartCoroutine(SpawnCheckCoroutine());
        UpdateUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("GameManager started successfully");
    }

    private void Update()
    {
        // เปิด/ปิด Slow Motion ด้วยปุ่ม Q
        if (Input.GetKeyDown(KeyCode.Q) && !isMathPanelActive)
        {
            ToggleSlowMotion();
        }

        // อัพเดทตำแหน่งเป้าเล็งให้อยู่กลางจอ
        if (crosshair != null)
        {
            crosshair.rectTransform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

        // ตรวจสอบการกด ESC เพื่อปิด Math Panel
        if (isMathPanelActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMathPanel();
        }

        // ตรวจสอบระยะทางผู้เล่นกับจุด spawn ซอมบี้
        CheckZombieSpawnPoints();

        // Debug controls
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DebugSpawnInfo();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TestSpawnZombie();
        }
    }

    private void CheckZombieSpawnPoints()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing!");
            return;
        }

        foreach (ZombieSpawnPoint spawnPoint in zombieSpawnPoints)
        {
            if (spawnPoint.transform != null && !spawnPoint.hasFinishedSpawning)
            {
                float distance = Vector3.Distance(player.transform.position, spawnPoint.transform.position);

                if (!spawnPoint.isActive && distance <= spawnPoint.activationDistance)
                {
                    spawnPoint.isActive = true;
                    Debug.Log("Spawn point activated: " + spawnPoint.transform.name +
                             ", Distance: " + distance.ToString("F1") + "/" + spawnPoint.activationDistance);
                }
            }
        }
    }

    private IEnumerator SpawnCheckCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Zombie spawn system started");

        while (true)
        {
            yield return new WaitForSeconds(spawnCheckInterval);

            if (!isSpawning && currentZombies < maxZombies)
            {
                CheckAndSpawnZombies();
            }
        }
    }

    private void CheckAndSpawnZombies()
    {
        bool foundActiveSpawnPoint = false;

        foreach (ZombieSpawnPoint spawnPoint in zombieSpawnPoints)
        {
            if (spawnPoint.isActive && !spawnPoint.hasFinishedSpawning && currentZombies < maxZombies)
            {
                foundActiveSpawnPoint = true;
                StartCoroutine(SpawnZombiesForPoint(spawnPoint));
            }
        }

        if (!foundActiveSpawnPoint)
        {
            Debug.Log("No active spawn points found or all points finished spawning");
        }
    }

    private IEnumerator SpawnZombiesForPoint(ZombieSpawnPoint spawnPoint)
    {
        if (spawnPoint.hasFinishedSpawning || currentZombies >= maxZombies) yield break;

        isSpawning = true;
        Debug.Log("Starting to spawn " + spawnPoint.zombieCount + " zombies at " + spawnPoint.transform.name);

        for (int i = 0; i < spawnPoint.zombieCount; i++)
        {
            if (spawnPoint.hasFinishedSpawning || currentZombies >= maxZombies) break;

            yield return new WaitForSeconds(spawnPoint.spawnDelay);

            if (CanSpawnZombie(spawnPoint))
            {
                SpawnSingleZombie(spawnPoint);
            }
            else
            {
                Debug.Log("Cannot spawn zombie at this time");
            }
        }

        spawnPoint.hasFinishedSpawning = true;
        isSpawning = false;
        Debug.Log("Finished spawning at: " + spawnPoint.transform.name);
    }

    private bool CanSpawnZombie(ZombieSpawnPoint spawnPoint)
    {
        bool canSpawn = currentZombies < maxZombies &&
                       !spawnPoint.hasFinishedSpawning &&
                       spawnPoint.isActive &&
                       zombiePrefab != null &&
                       spawnPoint.transform != null;

        if (!canSpawn)
        {
            Debug.Log("Cannot spawn zombie. Conditions: " +
                     "CurrentZombies=" + currentZombies +
                     ", MaxZombies=" + maxZombies +
                     ", FinishedSpawning=" + spawnPoint.hasFinishedSpawning +
                     ", IsActive=" + spawnPoint.isActive +
                     ", ZombiePrefab=" + (zombiePrefab != null) +
                     ", SpawnPointTransform=" + (spawnPoint.transform != null));
        }

        return canSpawn;
    }

    private void SpawnSingleZombie(ZombieSpawnPoint spawnPoint)
    {
        try
        {
            Vector3 spawnPosition = spawnPoint.transform.position +
                                   Random.insideUnitSphere * spawnPoint.spawnRadius;
            spawnPosition.y = spawnPoint.transform.position.y;

            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, spawnPoint.transform.rotation);

            Zombie zombieScript = zombie.GetComponent<Zombie>();
            if (zombieScript != null)
            {
                zombieScript.SetSpawnPoint(spawnPoint.transform);
            }
            else
            {
                Debug.LogWarning("Zombie script not found on prefab!");
            }

            activeZombies.Add(zombie);
            currentZombies++;
            spawnPoint.currentZombiesSpawned++;

            Debug.Log("Zombie spawned: " + spawnPoint.currentZombiesSpawned +
                     "/" + spawnPoint.zombieCount + " at " + spawnPoint.transform.name +
                     " Total zombies: " + currentZombies + "/" + maxZombies);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error spawning zombie: " + e.Message);
        }
    }

    public void ZombieKilled(GameObject zombie)
    {
        ZombieKilled(zombie, null);
    }

    public void ZombieKilled(GameObject zombie, Transform spawnPoint)
    {
        if (activeZombies.Contains(zombie))
        {
            activeZombies.Remove(zombie);
            currentZombies--;

            if (spawnPoint != null)
            {
                foreach (ZombieSpawnPoint zsp in zombieSpawnPoints)
                {
                    if (zsp.transform == spawnPoint)
                    {
                        zsp.currentZombiesSpawned--;
                        Debug.Log("Zombie killed from: " + spawnPoint.name +
                                 ", Remaining: " + zsp.currentZombiesSpawned);
                        break;
                    }
                }
            }

            Debug.Log("Zombies remaining: " + currentZombies);

            // ไม่ต้อง Destroy ที่นี่ เพราะ Zombie จัดการตัวเองแล้ว
        }
    }

    private void SpawnInitialAmmo()
    {
        if (ammoPickupPrefab == null || ammoSpawnPoints.Count == 0)
        {
            Debug.LogWarning("Ammo pickup prefab or spawn points not set!");
            return;
        }

        foreach (Transform spawnPoint in ammoSpawnPoints)
        {
            if (spawnPoint != null)
            {
                Instantiate(ammoPickupPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }

        Debug.Log("Spawned initial ammo pickups");
    }

    public void ShowMathProblem(int ammoAmount)
    {
        if (isMathPanelActive) return;

        StartCoroutine(ShowMathProblemCoroutine(ammoAmount));
    }

    private IEnumerator ShowMathProblemCoroutine(int ammoAmount)
    {
        EnableSlowMotion();
        isMathPanelActive = true;

        yield return new WaitForEndOfFrame();

        if (mathPanel != null)
            mathPanel.SetActive(true);
        else
            Debug.LogError("Math panel is not assigned!");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fpsController != null)
            fpsController.enabled = false;
        else
            Debug.LogWarning("FPS Controller not found");

        string problem = "";
        correctAnswer = 0;
        currentAmmoReward = ammoAmount;

        int problemType = Random.Range(0, 3);
        int a = Random.Range(1, 20);
        int b = Random.Range(1, 20);

        switch (problemType)
        {
            case 0:
                correctAnswer = a + b;
                problem = $"{a} + {b} = x";
                break;
            case 1:
                correctAnswer = a + b;
                problem = $"x - {a} = {b}";
                break;
            case 2:
                correctAnswer = a - b;
                problem = $"{a} - x = {b}";
                break;
        }

        if (mathProblemText != null)
            mathProblemText.text = problem;
        else
            Debug.LogError("Math problem text not assigned!");

        if (answerInput != null)
            answerInput.text = "";
        else
            Debug.LogError("Answer input not assigned!");

        if (answerInput != null)
        {
            answerInput.Select();
            answerInput.ActivateInputField();
        }
    }

    public void SubmitAnswer()
    {
        if (answerInput == null)
        {
            Debug.LogError("Answer input is null!");
            return;
        }

        if (int.TryParse(answerInput.text, out int playerAnswer))
        {
            if (playerAnswer == correctAnswer)
            {
                if (gun != null)
                {
                    gun.AddAmmo(currentAmmoReward);
                    Debug.Log("Correct answer! Added " + currentAmmoReward + " ammo");
                }
                else
                {
                    Debug.LogWarning("Gun reference is null!");
                }
            }
            else
            {
                Debug.Log("Wrong answer! Correct answer is: " + correctAnswer);
            }
        }
        else
        {
            Debug.Log("Please enter a valid number");
        }

        CloseMathPanel();
    }

    private void CloseMathPanel()
    {
        if (mathPanel != null)
            mathPanel.SetActive(false);
        else
            Debug.LogError("Math panel is null!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (fpsController != null)
            fpsController.enabled = true;
        else
            Debug.LogWarning("FPS Controller not found");

        DisableSlowMotion();
        isMathPanelActive = false;
    }

    public void EnableSlowMotion()
    {
        if (!isSlowMotion)
        {
            Time.timeScale = slowMotionFactor;
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
            isSlowMotion = true;
            Debug.Log("Slow motion enabled");
        }
    }

    public void DisableSlowMotion()
    {
        if (isSlowMotion)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime;
            isSlowMotion = false;
            Debug.Log("Slow motion disabled");
        }
    }

    public void ToggleSlowMotion()
    {
        if (isSlowMotion)
        {
            DisableSlowMotion();
        }
        else
        {
            EnableSlowMotion();
            StartCoroutine(ResetSlowMotion());
        }
    }

    private IEnumerator ResetSlowMotion()
    {
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        DisableSlowMotion();
    }

    public void UpdateUI()
    {
        if (ammoText != null && gun != null)
        {
            ammoText.text = "Ammo : " + gun.currentAmmo;
        }

        if (healthText != null && player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                healthText.text = "Health: " + playerHealth.currentHealth + "/100";
            }
        }
    }

    // Debug functions
    private void DebugSpawnInfo()
    {
        Debug.Log("=== ZOMBIE SPAWN DEBUG INFO ===");
        Debug.Log("Current Zombies: " + currentZombies + "/" + maxZombies);
        Debug.Log("Zombie Prefab: " + (zombiePrefab != null ? "Assigned" : "NULL!"));
        Debug.Log("Player: " + (player != null ? "Assigned" : "NULL!"));
        Debug.Log("Is Spawning: " + isSpawning);

        if (player != null)
        {
            foreach (ZombieSpawnPoint spawnPoint in zombieSpawnPoints)
            {
                if (spawnPoint.transform != null)
                {
                    float distance = Vector3.Distance(player.transform.position, spawnPoint.transform.position);
                    Debug.Log(spawnPoint.transform.name +
                             ": Active=" + spawnPoint.isActive +
                             ", Finished=" + spawnPoint.hasFinishedSpawning +
                             ", Distance=" + distance.ToString("F1") +
                             ", Spawned=" + spawnPoint.currentZombiesSpawned + "/" + spawnPoint.zombieCount);
                }
            }
        }
    }

    public void TestSpawnZombie()
    {
        if (zombieSpawnPoints.Count > 0 && zombiePrefab != null && currentZombies < maxZombies)
        {
            ZombieSpawnPoint testPoint = zombieSpawnPoints[0];
            if (!testPoint.isActive) testPoint.isActive = true;
            SpawnSingleZombie(testPoint);
            Debug.Log("Test zombie spawned manually");
        }
        else
        {
            Debug.Log("Cannot spawn test zombie. Conditions: " +
                     "SpawnPoints=" + zombieSpawnPoints.Count +
                     ", ZombiePrefab=" + (zombiePrefab != null) +
                     ", CurrentZombies=" + currentZombies +
                     ", MaxZombies=" + maxZombies);
        }
    }

    public void AddZombieForTest()
    {
        TestSpawnZombie();
    }

    public void AddAmmoForTest(int amount)
    {
        if (gun != null)
        {
            gun.AddAmmo(amount);
            UpdateUI();
            Debug.Log("Added " + amount + " ammo for testing");
        }
        else
        {
            Debug.LogWarning("Gun is null, cannot add ammo");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (zombieSpawnPoints != null)
        {
            foreach (ZombieSpawnPoint spawnPoint in zombieSpawnPoints)
            {
                if (spawnPoint != null && spawnPoint.transform != null)
                {
                    // วาดรัศมี activation
                    Gizmos.color = spawnPoint.isActive ? Color.red : Color.green;
                    Gizmos.DrawWireSphere(spawnPoint.transform.position, spawnPoint.activationDistance);

                    // วาดรัศมี spawn
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(spawnPoint.transform.position, spawnPoint.spawnRadius);

                    // วาดลูกศรแสดงทิศทาง
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(spawnPoint.transform.position, spawnPoint.transform.forward * 2f);
                }
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        // หยุดเกมชั่วคราว
        Time.timeScale = 0f;

        // แสดง game over UI
        // (เพิ่ม code ตามที่ต้องการ)
    }

}