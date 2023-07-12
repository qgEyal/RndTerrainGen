using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//! ensures these components are automatically added to the objects the script is attached to
//! add to exit doorway prefab
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]

public class ExitDoorway : MonoBehaviour
{
    SpriteRenderer sr;
    
    // Reset is called when the script is attached an not in playmode
    void Reset() 
    {
        // get access to the rigidbody
        GetComponent<Rigidbody2D>().isKinematic = true;
        
        // create a variable for the box collider created above
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        // resize boxcollider
        box.size = Vector2.one * 0.1f;
        
        // enable trigger
        box.isTrigger = true;

    }

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.yellow;
        
    }

    // collider2D functions (https://docs.unity3d.com/ScriptReference/Collider2D.html)
    // any collider than triggers the collider space...

    private void OnTriggerEnter2D(Collider2D other) {

        // confirm that the Player tag is assigned to other
        if(other.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); //! reloads the active scene rather than call it by name
        }

    }

}
