using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    private float currentSpeed;

    [SerializeField]
    private float jumpForce;

    //상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isGround = true;

    //움직임 체크
    private Vector3 lastPos;

    //카메라 민감도
    [SerializeField]
    private float lookSensitivity;

    //카메라 각도 제한
    [SerializeField]
    private float cameraRatationLimit; //제한
    private float currentCameraRotationX = 0; //정면
    //private float originPosY;
    //필요한 컴포넌트
    [SerializeField]
    private Camera cam;
    private Rigidbody myRigid;
    private CapsuleCollider myCollider;

    [SerializeField]
    private Text textColor;

    public MyColor MyColor { get; set; }

    void Start()
    {
        myCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
      
        //초기화
        currentSpeed = walkSpeed;
        //originPosY = cam.transform.localPosition.y; //originPosY = transform.position.y;이 아님!! 얘가 바뀌면 플레이어가 땅에 꺼짐.
        //현재 카메라는 플레이어에 속해있음. 상대적인 좌표 사용을 위해 position이 아닌 localPosition 사용
    }

    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        Move();
        MoveCheck();
        CameraRotation();
        CharacterRotation();
    }

    public void SetTextColor()
    {
        textColor.text = MyColor.ToString();
    }

    private void IsGround()
    {
        //여기서 -transfrom.up 을 쓰게 된다면 문제가 생김. 고정된 값인 벡터 사용할 것
        //(현재 위치, 목표 방향, 이동할 거리)
        isGround = Physics.Raycast(transform.position, Vector3.down, myCollider.bounds.extents.y + 0.1f);
    }
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }
    private void Jump()
    {
        myRigid.velocity = transform.up * jumpForce;
    }
    //달리기 시도
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
    //달리기 실행
    private void Running()
    {
        isRun = true;
        isWalk = false;
        currentSpeed = runSpeed;
    }
    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        isWalk = true;
        currentSpeed = walkSpeed;
    }

    //움직임 실행
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * currentSpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.smoothDeltaTime);
        //Time.deltaTime(약 0.016)
    }
    private void MoveCheck()
    {
        if (!isRun && isGround)
        {
            if (Vector3.Distance(lastPos, transform.position) >= 0.01f) isWalk = true;
            else isWalk = false;
            lastPos = transform.position;

        }

    }
    private void CameraRotation()
    {
        //상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity; //민감도 조절
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRatationLimit, cameraRatationLimit);

        cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
    private void CharacterRotation()
    {
        //좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //내부에서는 쿼터늄(4원소) 값으로 이루어짐. 우리가 보기 편하게 오일러(3원소) 값으로 표기함.
    }
}
