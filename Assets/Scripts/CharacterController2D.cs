using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    private BoxCollider2D boxCollider;
    GameObject art_right;
    GameObject art_left;
    GameObject walk_right;
    GameObject walk_left;

    enum Direction
    {
        Left,
        Right
    }
    Direction old_dir;  
    bool old_walk;  

    private Vector2 velocity;

    /// <summary>
    /// Set to true when the character intersects a collider beneath
    /// them in the previous frame.
    /// </summary>
    private bool grounded;

    private void changeAnimation(Direction dir, bool walking){
        if (!walking){
            if (dir == Direction.Left){
                art_right.SetActive(false);
                art_left.SetActive(true);
            }
            else{
                art_right.SetActive(true);
                art_left.SetActive(false);
            }
            walk_right.SetActive(false);
            walk_left.SetActive(false);
        }
        else{
            if (dir == Direction.Left){
                walk_right.SetActive(false);
                walk_left.SetActive(true);
            }
            else{
                walk_right.SetActive(true);
                walk_left.SetActive(false);
            }
            art_right.SetActive(false);
            art_left.SetActive(false);
        }
    }

    private void Awake()
    {      
        boxCollider = GetComponent<BoxCollider2D>();
        art_right = GameObject.Find("Art_right");
        art_left = GameObject.Find("Art_left");
        walk_right = GameObject.Find("walk_right");
        walk_left = GameObject.Find("walk_left");
        old_dir = Direction.Right;
        old_walk = false;
        changeAnimation(old_dir, old_walk); 
    }

    private void Update()
    {
        // Use GetAxisRaw to ensure our input is either 0, 1 or -1.
        float moveInput = Input.GetAxisRaw("Horizontal");
        float moveInput_x = Input.GetAxisRaw("Horizontal");
        float moveInput_y = Input.GetAxisRaw("Vertical");
        Debug.Log("x = " + moveInput_x.ToString() + " y = " + moveInput_y.ToString());
        Direction act_dir = old_dir;
        bool act_walk = old_walk;

        /*
        if (grounded)
        {
            velocity.y = 0;

            if (Input.GetButtonDown("Jump"))
            {
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        }
        */

        //float acceleration = grounded ? walkAcceleration : airAcceleration;
        //float deceleration = grounded ? groundDeceleration : 0;
        float acceleration = walkAcceleration;
        float deceleration = groundDeceleration;

        if (moveInput_x != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput_x, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (moveInput_y != 0)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, speed * moveInput_y, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.y = Mathf.MoveTowards(velocity.y, 0, deceleration * Time.deltaTime);
        }

        if (velocity.x != 0 || velocity.y != 0)
        {
            act_walk = true;
            if (velocity.x > 0) act_dir = Direction.Right;
            if (velocity.x < 0) act_dir = Direction.Left;
        }
        else
        {
            act_walk = false;
            act_dir = old_dir;
        }
       // Debug.Log("act_dir = "+act_dir+" act_walk = "+act_walk);
        if (act_dir != old_dir || act_walk != old_walk) changeAnimation(act_dir,act_walk);
        old_dir = act_dir;
        old_walk = act_walk;

       

        //velocity.y += Physics2D.gravity.y * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

        grounded = false;

        // Retrieve all colliders we have intersected after velocity has been applied.
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                {
                    //grounded = true;
                }
            }
        }
    }
}
