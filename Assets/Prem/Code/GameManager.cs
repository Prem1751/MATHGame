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
    public float spawnInterval = 5f;
    private int currentZombies = 0;
    private List<GameObject> activeZombies = new List<GameObject>();

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

        SpawnInitialAmmo();
        StartCoroutine(ZombieSpawner());
        UpdateUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isMathPanelActive)
        {
            ToggleSlowMotion();
        }

        if (crosshair != null)
        {
            crosshair.rectTransform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

        if (isMathPanelActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMathPanel();
        }

        CheckZombieSpawnPoints();
    }

    private void CheckZombieSpawnPoints()
    {
        if (player == null) return;

        foreach (ZombieSpawnPoint spawnPoint in zombieSpawnPoints)
        {
            if (spawnPoint.transform != null && !spawnPoint.hasFinishedSpawning)
            {
                float distance = Vector3.Distance(player.transform.position, spawnPoint.transform.position);

                if (!spawnPoint.isActive && distance <= spawnPoint.activationDistance)
                {
                    spawnPoint.isActive = true;
                    Debug.Log("Spawn point activated: " + spawnPoint.transform.name);
                    StartCoroutine(SpawnZombiesForPoint(spawnPoint));
                }
            }
        }
    }

    private IEnumerator SpawnZombiesForPoint(ZombieSpawnPoint spawnPoint)
    {
        Debug.Log("Starting to spawn " + spawnPoint.zombieCount + " zombies at " + spawnPoint.transform.name);

        for (int i = 0; i < spawnPoint.zombieCount; i++)
        {
            if (spawnPoint.hasFinishedSpawning || currentZombies >= maxZombies) yield break;

            yield return new WaitForSeconds(spawnPoint.spawnDelay);

            if (zombiePrefab != null && spawnPoint.transform != null)
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

                activeZombies.Add(zombie);
                currentZombies++;
                spawnPoint.currentZombiesSpawned++;

                Debug.Log("Zombie spawned: " + spawnPoint.currentZombiesSpawned + "/" + spawnPoint.zombieCount + " at " + spawnPoint.transform.name);
            }
        }

        spawnPoint.hasFinishedSpawning = true;
        Debug.Log("Finished spawning zombies at: " + spawnPoint.transform.name);
    }

    private IEnumerator ZombieSpawner()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
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
                        Debug.Log("Zombie killed from: " + spawnPoint.name + ", Remaining: " + zsp.currentZombiesSpawned);
                        break;
                    }
                }
            }

            Debug.Log("Total zombies remaining: " + currentZombies);
        }
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fpsController != null)
            fpsController.enabled = false;

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

        if (answerInput != null)
            answerInput.text = "";

        if (answerInput != null)
        {
            answerInput.Select();
            answerInput.ActivateInputField();
        }
    }

    public void SubmitAnswer()
    {
        if (answerInput == null) return;

        if (int.TryParse(answerInput.text, out int playerAnswer))
        {
            if (playerAnswer == correctAnswer)
            {
                if (gun != null)
                {
                    gun.AddAmmo(currentAmmoReward);
                    Debug.Log("คำตอบถูกต้อง! ได้กระสุน " + currentAmmoReward + " นัด");
                }
            }
            else
            {
                Debug.Log("คำตอบผิด! คำตอบที่ถูกต้องคือ: " + correctAnswer);
            }
        }

        CloseMathPanel();
    }

    private void CloseMathPanel()
    {
        if (mathPanel != null)
            mathPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (fpsController != null)
            fpsController.enabled = true;

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
        }
    }

    public void DisableSlowMotion()
    {
        if (isSlowMotion)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime;
            isSlowMotion = false;
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
    }

    public void UpdateUI()
    {
        if (ammoText != null && gun != null)
        {
            ammoText.text = "กระสุน: " + gun.currentAmmo;
        }

        if (healthText != null && player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                healthText.text = "สุขภาพ: " + playerHealth.currentHealth;
            }
        }
    }

    public void AddZombieForTest()
    {
        if (zombiePrefab != null && currentZombies < maxZombies)
        {
            Vector3 spawnPosition = player.transform.position + player.transform.forward * 5f;
            spawnPosition.y = 0;
            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            activeZombies.Add(zombie);
            currentZombies++;
            Debug.Log("Test zombie spawned");
        }
    }

    public void AddAmmoForTest(int amount)
    {
        if (gun != null)
        {
            gun.AddAmmo(amount);
            UpdateUI();
            Debug.Log("Added " + amount + " ammo for testing");
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
                    Gizmos.color = spawnPoint.isActive ? Color.red : Color.green;
                    Gizmos.DrawWireSphere(spawnPoint.transform.position, spawnPoint.activationDistance);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(spawnPoint.transform.position, spawnPoint.spawnRadius);
                }
            }
        }
    }
}