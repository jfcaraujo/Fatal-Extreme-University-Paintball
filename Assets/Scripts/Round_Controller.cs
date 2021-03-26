using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the rounds and spawns enemies
/// </summary>
public class Round_Controller : MonoBehaviour
{
    public GameObject enemyPistol;
    public GameObject enemyMachineGun;

    private int enemiesThisRoundPistol = 0;
    private int enemiesThisRoundMachineGun = 0;
    private int enemiesRemaining = 0;
    [SerializeField] private float timeBetweenSpawns = 2;
    [SerializeField] private float timeBetweenRounds = 2;

    private bool fleeing = false;
    public HealthController healthController;
    public Transform player;

    public Text scoreText;
    private int score;
    public Text enemiesLeftText;

    private float leftBorder, rightBorder, up, down;
    private bool spawnAtRight = true;
    private float spawnAbove = 0;

    void Start()
    {
        healthController.onHeal += Flee;
        healthController.onStopHeal += StopFlee;
        leftBorder = transform.GetChild(0).position.x;
        rightBorder = transform.GetChild(1).position.x;
        up = transform.GetChild(0).position.y;
        down = transform.GetChild(1).position.y;
        if (MainMenu.difficulty == 2)
            enemiesThisRoundPistol = 2;
        if (MainMenu.difficulty == 3)
        {
            enemiesThisRoundPistol = 2;
            enemiesThisRoundMachineGun = 3;
        }

        WeaponAmmo pistolAmmoDrop = enemyPistol.GetComponent<Enemy_Controller>().ammoDrop;
        WeaponAmmo machineGunAmmoDrop = enemyMachineGun.GetComponent<Enemy_Controller>().ammoDrop;
        switch (MainMenu.difficulty)
        {
            case 1:
                pistolAmmoDrop.ammo = 5;
                machineGunAmmoDrop.ammo = 5;
                break;
            case 2:
                pistolAmmoDrop.ammo = 4;
                machineGunAmmoDrop.ammo = 4;
                break;
            case 3:
                pistolAmmoDrop.ammo = 3;
                machineGunAmmoDrop.ammo = 3;
                break;
        }

        StartNextRound();
    }

    ///<summary>
    /// Starts the next round and increases the number of enemies per round
    ///</summary>
    private void StartNextRound()
    {
        enemiesThisRoundPistol++;
        if (enemiesThisRoundPistol == 4)
        {
            enemiesThisRoundPistol = 0;
            enemiesThisRoundMachineGun++;
        }

        enemiesRemaining = enemiesThisRoundPistol + enemiesThisRoundMachineGun;
        enemiesLeftText.text = enemiesRemaining.ToString();

        StartCoroutine(SpawnEnemies());

    }

    ///<summary>
    /// Spawns all the enemies for this round
    ///</summary>
    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(timeBetweenRounds);

        for (var i = 0; i < enemiesThisRoundPistol; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            if (fleeing)
                i--;
            else
                SpawnEnemyPistol();
        }

        for (var i = 0; i < enemiesThisRoundMachineGun; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            if (fleeing)
                i--;
            else
                SpawnEnemyMachineGun();
        }
    }

    ///<summary>
    /// Spawns a pistol enemy
    ///</summary>
    private void SpawnEnemyPistol()
    {
        Enemy_Controller enemy = Instantiate(enemyPistol, GetSpawnPosition(), transform.rotation)
            .GetComponent<Enemy_Controller>();
        enemy.onEnemyDeath += EnemyPistolDeath;
    }

    ///<summary>
    /// Spawns a machine gun enemy
    ///</summary>
    private void SpawnEnemyMachineGun()
    {
        Enemy_Controller enemy =
            Instantiate(enemyMachineGun, GetSpawnPosition(), transform.rotation)
                .GetComponent<Enemy_Controller>();
        enemy.onEnemyDeath += EnemyMachineGunDeath;
    }

    ///<summary>
    /// Creates a new position to spawn the enemy
    ///</summary>
    /// <returns> The new position </returns>
    private Vector2 GetSpawnPosition()
    {
        Vector2 playerPosition = player.position;
        float x = playerPosition.x + (spawnAtRight ? 1 : -1) * 15;
        if (x < leftBorder || x > rightBorder) x = playerPosition.x + (spawnAtRight ? -1 : 1) * 15;
        float y = spawnAbove < 2 ? down : up;
        spawnAbove = (spawnAbove + 1) % 4;
        spawnAtRight = x < playerPosition.x;
        return new Vector2(x, y);
    }

    ///<summary>
    /// Called automatically if a pistol enemy dies
    ///</summary>
    private void EnemyPistolDeath()
    {
        enemiesRemaining--;
        enemiesLeftText.text = enemiesRemaining.ToString();
        score += 15;
        scoreText.text = score.ToString();
        if (enemiesRemaining == 0)
            StartNextRound();
    }

    ///<summary>
    /// Called automatically if a machine gun enemy dies
    ///</summary>
    private void EnemyMachineGunDeath()
    {
        enemiesRemaining--;
        enemiesLeftText.text = enemiesRemaining.ToString();
        score += 30;
        scoreText.text = score.ToString();
        if (enemiesRemaining == 0)
            StartNextRound();
    }

    ///<summary>
    /// Called automatically if the player is healing
    ///</summary>
    private void Flee()
    {
        fleeing = true;
    }

    ///<summary>
    /// Called automatically if the player stops healing
    ///</summary>
    private void StopFlee()
    {
        fleeing = false;
    }
}