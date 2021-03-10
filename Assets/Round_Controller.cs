using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float timeBetweenRounds = 3;

    private bool fleeing = false;
    private HealthController healthController;

    public Text scoreText;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        healthController = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();
        healthController.onHeal += Flee;
        healthController.onStopHeal += StopFlee;
        StartNextRound();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SpawnEnemyPistol()
    {
        Enemy_Controller enemy = Instantiate(enemyPistol, gameObject.transform.position, gameObject.transform.rotation)
            .GetComponent<Enemy_Controller>();
        enemy.onEnemyDeath += EnemyPistolDeath;
    }

    private void SpawnEnemyMachineGun()
    {
        Enemy_Controller enemy =
            Instantiate(enemyMachineGun, gameObject.transform.position, gameObject.transform.rotation)
                .GetComponent<Enemy_Controller>();
        enemy.onEnemyDeath += EnemyMachineGunDeath;
    }

    private void EnemyPistolDeath()
    {
        enemiesRemaining--;
        score += 15;
        scoreText.text = score.ToString();
        //Debug.Log("Enemies remaining:" + enemiesRemaining);
        if (enemiesRemaining == 0)
            StartNextRound();
    }

    private void EnemyMachineGunDeath()
    {
        enemiesRemaining--;
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

        StartCoroutine(SpawnEnemies());

        // Debug.Log(enemiesThisRoundPistol + "  " + enemiesThisRoundMachineGun);
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(timeBetweenRounds);

        for (var i = 0; i < enemiesThisRoundPistol; i++)
        {
            //Debug.Log("Spawning pistol " + i);
            if (fleeing)
                i--;
            else
                SpawnEnemyPistol();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        for (var i = 0; i < enemiesThisRoundMachineGun; i++)
        {
            if (fleeing)
                i--;
            else
                SpawnEnemyMachineGun();
            yield return new WaitForSeconds(timeBetweenSpawns);
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