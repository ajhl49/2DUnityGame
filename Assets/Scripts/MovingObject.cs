using UnityEngine;

public class MovingObject : MonoBehaviour
{

    public float Speed = 6f;
    public float SprintMult = 2f;
    public bool Sprint;

    private Vector3 _currentDirection = Vector3.zero;


    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _currentDirection = Vector3.left;
            transform.position += _currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
            //transform.Translate(currentDirection * Time.deltaTime * (Sprint ? SprintMult : 1) * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.D))
        {
            _currentDirection = Vector3.right;
            transform.position += _currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
            //transform.Translate(currentDirection * Time.deltaTime * (Sprint ? SprintMult : 1) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            _currentDirection = Vector3.up;
            transform.position += _currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
            //transform.Translate(currentDirection * Time.deltaTime * (Sprint ? SprintMult : 1) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _currentDirection = Vector3.down;
            transform.position += _currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
            //transform.Translate(currentDirection * Time.deltaTime * (Sprint ? SprintMult : 1) * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Sprint = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Sprint = false;
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("bomb"))
        {
            print("BOOM!!!!");
            Destroy(collision.gameObject);
        }
       
        //if (collision.relativeVelocity.magnitude > 2)
          //  audio.Play();

    }
  
    

}
