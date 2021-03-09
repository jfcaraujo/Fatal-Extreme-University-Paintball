﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round_Controller : MonoBehaviour
{
    public GameObject enemyPistol;
    public GameObject enemyMachineGun;

    private int enemiesThisRoundPistol = 0;
    private int enemiesThisRoundMachineGun = 0;
    private int enemiesRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
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
        ;
        enemy.onEnemyDeath += EnemyDeath;
    }

    private void SpawnEnemyMachineGun()
    {
        Enemy_Controller enemy =
            Instantiate(enemyMachineGun, gameObject.transform.position, gameObject.transform.rotation)
                .GetComponent<Enemy_Controller>();
        ;
        enemy.onEnemyDeath += EnemyDeath;
    }

    private void EnemyDeath()
    {
        enemiesRemaining--;
        Debug.Log("Enemies remaining:" + enemiesRemaining);
        if (enemiesRemaining == 0)
            StartNextRound();
    }

    private void StartNextRound()
    {
        enemiesThisRoundPistol++;
        if (enemiesThisRoundPistol == 4)
        {
            enemiesThisRoundPistol = 0;
            enemiesThisRoundMachineGun++;
        }

        StartCoroutine(SpawnEnemies());
        enemiesRemaining = enemiesThisRoundPistol + enemiesThisRoundMachineGun;

        Debug.Log(enemiesThisRoundPistol + "  " + enemiesThisRoundMachineGun);
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(3);

        for (var i = 0; i < enemiesThisRoundPistol; i++)
        {
            SpawnEnemyPistol();
            yield return new WaitForSeconds(3);
        }

        for (var i = 0; i < enemiesThisRoundMachineGun; i++)
        {
            SpawnEnemyMachineGun();
            yield return new WaitForSeconds(3);
        }
    }
}