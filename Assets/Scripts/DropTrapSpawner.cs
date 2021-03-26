using UnityEngine;

/// <summary>
/// Script to handle the spawn of Drop Traps in the map.
/// </summary>
public class DropTrapSpawner : MonoBehaviour
{
    // Limits the map area where the traps can spawn
    private float leftBorder, rightBorder;
    private int trapsPerFloor = 2;

    // Object to be spawned
    public GameObject trap;

    void Start()
    {
        leftBorder = transform.GetChild(0).position.x;
        rightBorder = transform.GetChild(1).position.x;
        
        if (MainMenu.difficulty == 2)
            trapsPerFloor = 4;
        else if (MainMenu.difficulty == 3)
            trapsPerFloor = 8;
        
        SpawnTraps();
    }

    /// <summary>
    /// Spawn initial traps.
    /// </summary>
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

    /// <summary>
    /// Spawn 1 trap on the top floor.
    /// </summary>
    private void SpawnTrapTop()
    {
        DropTrap dropTrap =
            Instantiate(trap, new Vector2(Random.Range(leftBorder, rightBorder), 10), transform.rotation)
                .GetComponent<DropTrap>();

        // Subscribe to the onTrapSprung event, to spawn another trap when this one is destroyed
        dropTrap.onTrapSprung += SpawnTrapTop;
    }

    /// <summary>
    /// Spawn 1 trap on the bottom floor.
    /// </summary>
    private void SpawnTrapBottom()
    {
        DropTrap dropTrap = Instantiate(trap, new Vector2(Random.Range(leftBorder, rightBorder), 5), transform.rotation)
            .GetComponent<DropTrap>();
        
        // Subscribe to the onTrapSprung event, to spawn another trap when this one is destroyed
        dropTrap.onTrapSprung += SpawnTrapBottom;
    }
}