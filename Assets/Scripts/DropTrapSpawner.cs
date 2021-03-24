using UnityEngine;

public class DropTrapSpawner : MonoBehaviour
{
    private float leftBorder, rightBorder;
    private int trapsPerFloor = 2;
    public GameObject trap;

    void Start()
    {
        leftBorder = transform.GetChild(0).position.x;
        rightBorder = transform.GetChild(1).position.x;
        if (MainMenu.difficulty == 2)
            trapsPerFloor = 4;
        if (MainMenu.difficulty == 3)
            trapsPerFloor = 8;
        SpawnTraps();
    }

    private void SpawnTraps()
    {
        for (var i = 0; i < trapsPerFloor; i++)
        {
            SpawnTrapTop();
        }

        for (var i = 0; i < trapsPerFloor; i++)
        {
            SpawnTrapBottom();
        }
    }

    private void SpawnTrapTop()
    {
        DropTrap dropTrap =
            Instantiate(trap, new Vector2(Random.Range(leftBorder, rightBorder), 10), transform.rotation)
                .GetComponent<DropTrap>();
        dropTrap.onTrapSprung += SpawnTrapTop;
    }

    private void SpawnTrapBottom()
    {
        DropTrap dropTrap = Instantiate(trap, new Vector2(Random.Range(leftBorder, rightBorder), 5), transform.rotation)
            .GetComponent<DropTrap>();
        dropTrap.onTrapSprung += SpawnTrapBottom;
    }
}