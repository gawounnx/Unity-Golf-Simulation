using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 aimOffset = new Vector3(0, 15, -20);
    public Vector3 followOffset = new Vector3(0, 8, -12);

    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = aimOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Rigidbody ballRb =
            target.GetComponent<Rigidbody>();

        Vector3 moveDir =
            ballRb.linearVelocity.normalized;

        if(moveDir.magnitude < 0.1f)
        {
            moveDir = -target.forward;
        }

        Vector3 desiredPos =
            target.position
            - moveDir * 12f
            + Vector3.up * 8f;

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPos,
                Time.deltaTime * 3f
            );

        Quaternion targetRot =
            Quaternion.LookRotation(
                target.position - transform.position
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 3f
            );
        
        Debug.Log(
            "카메라 컴포넌트 상태 = "
            + GetComponent<Camera>().enabled);
    }

    public void SetAimView()
    {
        currentOffset = aimOffset;
    }

    public void SetFollowView()
    {
        currentOffset = followOffset;
    }
}