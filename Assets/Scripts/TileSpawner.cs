using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    WorldManager worldManager;

    public LayerMask envLayerMask;

    // create list for wall/water boundaries
    List<Vector3> boundaryList = new List<Vector3>();
    
    //! FIRST ACTION 
    void Awake()
    {
        // get access to World Manager on Environment GameObject before Start()
        // could use a Singleton here instead. Find most efficient way. Consider also Get/Set or TryGetComponent(out functionName name)
        worldManager = FindObjectOfType<WorldManager>();

        //!instantiate floor tile FIRST. Saving as gameobject allows for parenting
        GameObject goFloor = Instantiate(worldManager.floorPrefab, transform.position, Quaternion.identity) as GameObject; 
        
        // names the gameobject (removes the word Clone) and parents
        goFloor.name = worldManager.floorPrefab.name;
        goFloor.transform.SetParent(worldManager.transform);

        // Debug.LogFormat("Before Instantiate: MinX: {0}, MaxX: {1}, MinY: {2}, MaxY: {3}", worldManager.minX, worldManager.maxX, worldManager.minY,worldManager.maxY);

        //! modify the min & max X&Y conditions after a floor is intantiated
        if (transform.position.x > worldManager.maxX)
        {
            worldManager.maxX = transform.position.x;
        }
        if (transform.position.x < worldManager.minX)
        {
            worldManager.minX = transform.position.x;
        }
         if (transform.position.y > worldManager.maxY)
        {
            worldManager.maxY = transform.position.y;
        }
        if (transform.position.y < worldManager.minY)
        {
            worldManager.minY= transform.position.y;
        }

        // Debug.LogFormat("AFter Instantiate: MinX: {0}, MaxX: {1}, MinY: {2}, MaxY: {3}", worldManager.minX, worldManager.maxX, worldManager.minY,worldManager.maxY);

    }


    // Start is called before the first frame update
    void Start()
    {
        
        //! ensure the hitSize overlap box is smaller than a Unity unit, otherwise it doesn't work
        Vector2 hitSize = Vector2.one * 0.8f;
        
        // loop for positions between -1,1 on X & Y axis
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 targetPos = new Vector2(transform.position.x + x, transform.position.y + y);
                Collider2D hit = Physics2D.OverlapBox(targetPos, hitSize, 0, envLayerMask);

                // check if there's NO collision
                if(!hit) 
                {
                    //! add a wall  <-- INCLUDING COLLISIONS
                    GameObject goWall = Instantiate(worldManager.borderPrefab, targetPos, Quaternion.identity) as GameObject; 
                    goWall.name = worldManager.borderPrefab.name;
                    goWall.transform.SetParent(worldManager.transform);
                }

            }
        }
        
        // destroy
        Destroy(gameObject);
    }

    // create a gizmo to visualize tile placement
    void OnDrawGizmos() 
    {
        Gizmos.color = Color.white;
        // what to draw
        Gizmos.DrawCube(transform.position, Vector3.one);
    }

    

   
}


/* NOTES

gameObject refers to the GameObject that the current script is attached to
whereas
GameObject refers to a object type / entity

with that being said:
gameObject would be used when you want to reference the GameObject that the script is attached to for example:

gameObject.GetComponent<Transform>();

The code above will reference the Transform component that is on the GameObject that the script is attached to
where as
GameObject would be used in an instance where you want to create a variable etc
for example:

GameObject myPlayer;

the code above simply create a varible called 'myPlayer'



*/
