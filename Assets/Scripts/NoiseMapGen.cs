using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGen : MonoBehaviour
{
    // pair ID code with the game object
    Dictionary<int, GameObject> tileset;
    // for tile group organization
     Dictionary<int, GameObject> tile_groups;

    //! world tiles
    // advantage of doing it like this is ability to change, rather than findbyname. Check if other options exist
    public GameObject pfGrass01;
    public GameObject pfGrass02;
    public GameObject pfGrass03;
    public GameObject pfGrass04;

    // map variables
    public int mapWdith = 160;
    public int mapHeight = 90;

    // create 2 2D lists to hold the noise grid, and the second to hold all the tiles
    // i.e. [[0,1,2,3], [0,2,3,1], [,1,2,1,3]]
    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // allow to "magnify" terrain for more or less detail in the Perlin noise map
    // use values between 4.0f and 20.0f
    public float magnification = 7.0f;

    // offset options. Can go either way with ints or floats
    public int x_offset = 0;
    public int y_offset = 0;

    // public float x_offset = 0.0f;
    // public float y_offset = 0.0f;

    //! add seed for randomization
    public float seed;

    
    
    // Start is called before the first frame update
    void Start()
    {
        //! center on grid
        gameObject.transform.position = new Vector3(-(mapWdith/2), -(mapHeight/2), 0.0f);
        
        CreateTileset();
        // organize tiles into named groups
        CreateTileGroups();

        GenerateMap();
    }

    void CreateTileset()
    {
        // instantiate new dictionary
        tileset = new Dictionary<int, GameObject>();
        // add to dictionary(key, value). Easier to do with int rather than string. Better iteration
        tileset.Add(0, pfGrass01);
        tileset.Add(1, pfGrass02);
        tileset.Add(2, pfGrass03);
        tileset.Add(3, pfGrass04);
    }

    void CreateTileGroups()
    {
        tile_groups = new Dictionary<int, GameObject>();
        // use the tileset names
        foreach(KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            // grab the name of the GameObject
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            // parent under Map object
            tile_group.transform.parent = gameObject.transform; //! <--- the lower case references the gameObject the script is attached to
            tile_group.transform.localPosition = new Vector3(0,0,0);
            // this ensures the groups match the tiles 1:1
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }

    void GenerateMap()
    {
        for (int x = 0; x < mapWdith; x++)
        {
            // initialize the ID inner list
            //! interesting approach. I used something similar for rigging in Python
            noise_grid.Add(new List<int>());

            // initialize the gameobject inner list
            tile_grid.Add(new List<GameObject>());

            for (int y = 0; y < mapHeight; y++)
            {
                int tile_id = GetIdUsingPerlin(x,y);
                noise_grid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }
    }

    int GetIdUsingPerlin(int x, int y)
    {
        // pseudo randomizer
        int rndNoise = Random.Range(0,100);
        
        // Perlin noise works best with floats. Ints will always return the same values
        float raw_perlin = Mathf.PerlinNoise(((x - x_offset ) / magnification) + seed, ((y - y_offset ) / magnification) + seed);

        // normalize the values
        float clamp_perlin = Mathf.Clamp(raw_perlin, 0.0f, 1.0f);

        // rescale raw_perlin output values to be the same size as the number of tiles
        float scaled_perlin = clamp_perlin * tileset.Count;

        Debug.LogFormat("tileset count total {0}", tileset.Count);

        
        // check the last number
        if (scaled_perlin == tileset.Count)
        {
            scaled_perlin = (tileset.Count - 1);
        }

        // round down to nearest integer, basically exclusive of the total tileset.Count   <--------- check
        return Mathf.FloorToInt(scaled_perlin);
    }

    void CreateTile(int tile_id, int x, int y)
    {
        // get the prefab the tile_id belongs to
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);

        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        
        // set position in local space, not world space because that would ignore parent
        tile.transform.localPosition = new Vector3(x,y,0);

        // add to tile_grid list
        tile_grid[x].Add(tile);
    }

    
}
