using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    public Text scoreText;
    private int score;
    public Text enemiesLeftText;

    // Start is called before the first frame update
    void Start()
    {
        healthController.onHeal += Flee;
        healthController.onStopHeal += StopFlee;
        StartNextRound();
    }

    private void SpawnEnemyPistol()
    {
        Transform gameObjectTransform = gameObject.transform;
        Enemy_Controller enemy = Instantiate(enemyPistol, gameObjectTransform.position, gameObjectTransform.rotation)
            .GetComponent<Enemy_Controller>();
        enemy.onEnemyDeath += EnemyPistolDeath;
    }

    private void SpawnEnemyMachineGun()
    {
        Transform gameObjectTransform = gameObject.transform;
        Enemy_Controller enemy =
            Instantiate(enemyMachineGun, gameObjectTransform.position, gameObjectTransform.rotation)
                .GetComponent<Enemy_Controller>();
        enemy.onEnemyDeath += EnemyMachineGunDeath;
    }

    private void EnemyPistolDeath()
    {
        enemiesRemaining--;
        enemiesLeftText.text = enemiesRemaining.ToString();
        score += 15;
        scoreText.text = score.ToString();
        //Debug.Log("Enemies remaining:" + enemiesRemaining);
        if (enemiesRemaining == 0)
            StartNextRound();
    }

    private void EnemyMachineGunDeath()
    {
        enemiesRemaining--;
        enemiesLeftText.text = enemiesRemaining.ToString();
        score += 30;
        scoreText.text = score.ToString();
        //Debug.Log("Enemies remaining:" + enemiesRemaining);
        if (enemiesRemaining == 0)
            StartNextRound();
    }

    private void StartNextRound()
    {
        /*score += 100;
        scoreText.text = score.ToString();*/
        enemiesThisRoundPistol++;
        if (enemiesThisRoundPistol == 4)
        {
            enemiesThisRoundPistol = 0;
            enemiesThisRoundMachineGun++;
        }

        enemiesRemaining = enemiesThisRoundPistol + enemiesThisRoundMachineGun;
        enemiesLeftText.text = enemiesRemaining.ToString();

        StartCoroutine(SpawnEnemies());

        // Debug.Log(enemiesThisRoundPistol + "  " + enemiesThisRoundMachineGun);
    }

    IEnumerator SpawnEnemies()
    {
        //Debug.Log(enemiesThisRoundPistol+"  "+enemiesThisRoundMachineGun+" "+enemiesRemaining);
        yield return new WaitForSeconds(timeBetweenRounds);

        for (var i = 0; i < enemiesThisRoundPistol; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            //Debug.Log("Spawning pistol " + i);
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

    private void Flee()
    {
        fleeing = true;
    }

    private void StopFlee()
    {
        fleeing = false;
    }
}