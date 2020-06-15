using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]    private float walk_speed;

    [SerializeField]    private float run_speed;
                        private float apply_speed;

    // 상태변수
    private bool is_run = false;

    // 강체 - 실제 몸의 역할
    //[SerializeField]
    private Rigidbody my_rigidbody;

    [SerializeField]
    private float look_sensitivity;

    // 카메라 한계
    [SerializeField]
    private float camera_rotaion_limit;
    private float current_camera_rotation_x = 0;

    [SerializeField]
    private Camera main_camera;
    // Start is called before the first frame update
    void Start()
    {
        my_rigidbody = GetComponent<Rigidbody>();
        apply_speed = walk_speed;
    }

    // Update is called once per frame
    void Update()
    {
        TryRun();
        Move();
        CameraRotation();
        CaracterRotation();
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }

    }

    private void Running()
    {
        is_run = true;
        apply_speed = run_speed;
    }
    private void RunningCancel()
    {
        is_run = false;
        apply_speed = walk_speed;
    }

    //좌우회전
    private void CaracterRotation()
    {
        float y_rotation = Input.GetAxisRaw("Mouse X");
        Vector3 character_rotation_y = new Vector3(0f, y_rotation, 0f) * look_sensitivity;
        my_rigidbody.MoveRotation(my_rigidbody.rotation * Quaternion.Euler(character_rotation_y));
        Debug.Log(my_rigidbody.rotation);
        Debug.Log(my_rigidbody.rotation.eulerAngles);
    }

    //상하 카메라회전
    private void CameraRotation()
    {
        float x_rotation = Input.GetAxisRaw("Mouse Y");
        float camera_rotation_x = x_rotation * look_sensitivity;
        current_camera_rotation_x -= camera_rotation_x;
        current_camera_rotation_x = Mathf.Clamp(current_camera_rotation_x, -camera_rotaion_limit, camera_rotaion_limit);

        main_camera.transform.localEulerAngles = new Vector3(current_camera_rotation_x, 0f, 0f);
    }

    private void Move()
    {
        float move_direction_x = Input.GetAxisRaw("Horizontal");
        float move_direction_z = Input.GetAxisRaw("Vertical");


        Vector3 move_horizontal = transform.right * move_direction_x;
        Vector3 move_vertical = transform.forward * move_direction_z;

        // normalized : 방향
        Vector3 velocity = (move_horizontal + move_vertical).normalized * apply_speed ;
        my_rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
    }
}
