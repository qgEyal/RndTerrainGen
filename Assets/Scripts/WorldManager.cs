using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//! WORLD BUILDER

/**
Basic framework is in place.
Next steps are to generate a noise generator that will integrate with the random walk algorithm, to create
a wider game area, while optimizing the resources needed.
Start looking at creating shaders to generate noise, and convert them to coordinates to map tiles onto.
**/

public class WorldManager : MonoBehaviour
{
    // array of ground objects
    public GameObject[] groundItems;

    public GameObject floorPrefab, borderPrefab, tilePrefab, exitPrefab;

    public int totalFloorCount; 

    [Range(0, 100)] public int groundSpawnPercent;

    //! establish the boundaries (bbox) of the GROUND tiles created by the tileSpawner
    public float minX, maxX, minY, maxY;

    // masks
    [SerializeField] LayerMask envLayerMask;

    List<Vector3> floorList = new List<Vector3>();

    private void Start() 
    {
        // WorldSurface();
        RandomWalker();
    }

    void Update() 
    {
        // reset level inEditor when pressing backspace
        if(Application.isEditor && Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //! create world surface
    //! [NEEDS OPTIMIZING - IGNORED FOR NOW]
    void WorldSurface()
    {
        int worldWidth, worldHeight;
        worldWidth = worldHeight = totalFloorCount/4;
        
        Debug.LogFormat("worldwidth {0} {1}",worldWidth, worldHeight);

        for (int x = -worldWidth; x <= worldWidth; x++)
        {
            for (int y = -worldHeight; y <= worldHeight; y++)
            {
                GameObject water = GameObject.Instantiate(borderPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                water.name = "water";
                water.transform.SetParent(transform);
            }
        }
    }   


    void RandomWalker()
    {
        //! Starting point of Random Walker agent
        Vector3 curPos = Vector3.zero;

        // set floor tile at the current position and add to List
        floorList.Add(curPos);

        // loop while floorCount var is viable
        while(floorList.Count < totalFloorCount)
        {
            // call function
            curPos += RandomDirection();
            
            // look for duplicates
            //! much more elegant than the original version, without use of booleans
            if(!floorList.Contains(curPos))
            {
                floorList.Add(curPos);
            }

            /* TRADITIONAL ALTERNATIVE
            bool inFloorList = false;
            for(int i = 0; i < floorList.Count; i++)
            {
                if (Vector3.Equals(curPos, floorList[i]))
                {
                    inFloorList = true;
                    break;
                }
            }
            if(!inFloorList)
            {
                floorList.Add(curPos);
            }
            */
        }
        // instantiate tilePrefab
        for(int i = 0; i < floorList.Count; i++)
            {
                GameObject goTile = Instantiate(tilePrefab, floorList[i], Quaternion.identity) as GameObject;
                goTile.name = tilePrefab.name;
                // parent to gameobject that script is attached to
                goTile.transform.SetParent(transform);
            }
        
        // call the DelayProgress once all tiles are laid down
        StartCoroutine(DelayProgress());

    }

    //! Random direction function
    // Can be further optimized using Hash set. Explore this <--

    Vector3 RandomDirection()
    {
         // set a random direction picker
            switch(Random.Range(1,5))
            {
                case 1:
                    return Vector3.up;

                case 2:
                    return Vector3.right;

                case 3:
                    return Vector3.down;

                case 4:
                    return Vector3.left;

            }
        // default return function. Does not get used but required
        return Vector3.zero;
    }


    // Create an IEnumerator function that will wait until all tilespawners are created
    // and then position exit doorway

    IEnumerator DelayProgress()
    {
        while(FindObjectsOfType<TileSpawner>().Length > 0)
        {
            // do nothing as long as there are tilespawner objects
            yield return null;
        }
        ExitDoorway();

        Vector2 hitSize = Vector2.one * 0.8f;

        // Add ground items
        //! start iterating through the groundItems array. Cast the min/max as ints
        // Debug.LogFormat("MinX: {0}, MaxX: {1}, MinY: {2}, MaxY: {3}", minX, maxX, minY,maxY);
        // Debug.LogFormat("+-2 --> MinX: {0}, MaxX: {1}, MinY: {2}, MaxY: {3}", minX-2, maxX+2, minY-2,maxY+2);

        for (int x = (int)minX - 2; x <= (int)maxX + 2; x++)
        {
            for (int y = (int)minY - 2; y <= (int)maxY; y++)
            {
                //! check if it's a floor. Use bitwise operators for LayerMask (see Inspector) 7 = wall, 9 = floor
                Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(x,y), hitSize, 0,(1<<9));

                if(hitFloor)
                {
                    // don't place anything on top of exit which is last item on the floorlist List
                    //! interesting approach but can be done better
                    if(!Vector2.Equals(hitFloor.transform.position, floorList[floorList.Count -1]))
                    {
                        // set 4 collider2D for each direction
                        Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x,y+1), hitSize, 0,(1<<7));
                        Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x+1,y), hitSize, 0,(1<<7));
                        Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x,y-1), hitSize, 0,(1<<7));
                        Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x-1,y), hitSize, 0,(1<<7));
                        
                        //! place ground items
                        RandomGroundItems(hitFloor, hitTop, hitRight, hitBottom, hitLeft);

                        
                    }
                }
            }
        }
        
    }

    void RandomGroundItems(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft)
    {
        //! check to see if ground items get spawned on top of Player at 0,0
        if (hitFloor.transform.position == Vector3.zero)
        {
            return;
        }
        
        //! if there is a wall in any one of the cardinal directions AND ther are not walls in the sides or top/bottom, add item
        //! in other words, place them right next to the walls
        // if((hitTop || hitRight || hitBottom || hitLeft) && !(hitTop && hitBottom) && !(hitLeft && hitRight))
        if( !(hitTop && hitBottom) && !(hitLeft && hitRight))
        {
            // Set range of ground decoration spawning
            int groundRoll = Random.Range(0,101);
            if (groundRoll < groundSpawnPercent)
            {
                int groundIndex = Random.Range(0, groundItems.Length);
                GameObject goGround = Instantiate(groundItems[groundIndex],hitFloor.transform.position, Quaternion.identity) as GameObject;
                goGround.name = groundItems[groundIndex].name;
                goGround.transform.SetParent(hitFloor.transform);
            }
        }
    }

    void ExitDoorway()
    {
        // set exit doorway at the last position the floor tile was created
        // check other methods of placing objects relative to position of player start (i.e. first tile --> currently not done in script)
        Vector3 doorPos = floorList[floorList.Count -1];
        GameObject goDoor = Instantiate(exitPrefab, doorPos, Quaternion.identity) as GameObject;
        goDoor.name = exitPrefab.name;
        // parent to gameobject that script is attached to
        goDoor.transform.SetParent(transform);
    }


}
