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

    [SerializeField] private float crouch_speed;

    [SerializeField] private float jump_force;
    private bool is_ground = true;
    // 상태변수
    private bool is_run = false;
    private bool is_crouch = false;

    // 강체 - 실제 몸의 역할
    //[SerializeField]
    private Rigidbody my_rigidbody;

    [SerializeField]
    private float look_sensitivity;

    //앉기
    [SerializeField]
    private float crouch_position_y;
    private float original_position_y;
    private float apply_crouch_position_y;

    // 카메라 한계
    [SerializeField]
    private float camera_rotaion_limit;
    private float current_camera_rotation_x = 0;

    [SerializeField]
    private Camera main_camera;

    // 착지 여부 체크
    private CapsuleCollider capsule_collider;

    // Start is called before the first frame update
    void Start()
    {
        my_rigidbody = GetComponent<Rigidbody>();
        capsule_collider = GetComponent<CapsuleCollider>();
        apply_speed = walk_speed;
        original_position_y = main_camera.transform.localPosition.y;
        apply_crouch_position_y = original_position_y;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryCrouch();
        TryRun();
        Move();
        CameraRotation();
        CaracterRotation();
    }

    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        is_crouch = !is_crouch;

        if (is_crouch)
        {
            apply_speed = crouch_speed;
            apply_crouch_position_y = crouch_position_y;
        }
        else
        {
            apply_speed = walk_speed;
            apply_crouch_position_y = original_position_y;
        }

        StartCoroutine(CrouchCroutine());
    }

    IEnumerator CrouchCroutine()
    {
        float position_y = main_camera.transform.localPosition.y;
        int count = 0;

        while(position_y != apply_crouch_position_y)
        {
            count++;
            //보간
            position_y = Mathf.Lerp(position_y, apply_crouch_position_y, 0.3f);
            main_camera.transform.localPosition = new Vector3(0, position_y, 0);

            if (count > 15)
                break;

            yield return null;

        }
        main_camera.transform.localPosition = new Vector3(0, apply_crouch_position_y, 0);
    }

    private void IsGround()
    {
        is_ground = Physics.Raycast(transform.position, Vector3.down, capsule_collider.bounds.extents.y + 0.1f );
    }
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && is_ground)
        {
            Jump();
        }
    }
    private void Jump()
    {
        // 0, 1, 0
        my_rigidbody.velocity = transform.up * jump_force;
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
