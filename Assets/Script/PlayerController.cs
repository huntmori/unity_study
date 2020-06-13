using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walk_speed;

    // 강체 - 실제 몸의 역할
    //[SerializeField]
    private Rigidbody my_rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        my_rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float move_direction_x = Input.GetAxisRaw("Horizontal");
        float move_direction_z = Input.GetAxisRaw("Vertical");


        Vector3 move_horizontal = transform.right * move_direction_x;
        Vector3 move_vertical = transform.forward * move_direction_z;

        // normalized : 방향
        Vector3 velocity = (move_horizontal + move_vertical).normalized * walk_speed ;
        my_rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
    }
}
