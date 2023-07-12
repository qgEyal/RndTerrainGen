using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    //! rigidbody to check collision with other objects
    // Get the Player's sprite info in case we need to reference it for further effect
    [SerializeField] SpriteRenderer spriteObject; 


    // Access game layers
    [SerializeField] LayerMask moveLayerMask;

    // create move function and check for collisions
    void Move (Vector2 dir) 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.0f, moveLayerMask);

        // if there's no collision...
        if(hit.collider == null)
        {
            // move forward. --> have to use Vector3 since transform.position uses xyz even in 2D
            transform.position += new Vector3(dir.x, dir.y, 0);
        }
    }

    public void OnMoveUp (InputAction.CallbackContext context)
    {

        // have we pressed down the corresponding key?
        if(context.phase == InputActionPhase.Performed)
        {
            // if so, move towards the given direction.
            Move(Vector2.up);
        }
    }
    public void OnMoveDown (InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            Move(Vector2.down);
        }   

    }
    public void OnMoveLeft (InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            Move(Vector2.left);
            spriteObject.transform.localScale = new Vector3(-1, 1, 1);
            
        } 
    }
    public void OnMoveRight (InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            Move(Vector2.right);
            spriteObject.transform.localScale = new Vector3(1, 1, 1);
            
        } 
    }
}
