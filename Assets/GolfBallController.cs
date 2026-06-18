using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    private Vector3 startPosition;
    public float power = 20f;
    public float upwardPower = 3f;

    public int stroke = 0;
    private Rigidbody rb;

    public float currentPower = 0f;
    public float maxPower = 25f;
    public float chargeSpeed = 20f;

    public float rotateSpeed = 50f;
    private bool isCharging = false;

    public LineRenderer trajectoryLine;

    public Transform aimPivot;

    public WindManager windManager;

    private float friction = 0.99f;

    public Camera aimCamera;
    public Camera followCamera;

    public CameraFollow cameraFollow;

    private bool isBallMoving = false;
    public bool gameClear = false;

    private float stopTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trajectoryLine = GetComponent<LineRenderer>();

        aimCamera.gameObject.SetActive(true);
        followCamera.gameObject.SetActive(false);

        startPosition = transform.position;
    }

    void Update()
    {
         if (gameClear)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            currentPower = 0f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            currentPower += chargeSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, 0, maxPower);

            DrawTrajectory();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isCharging = false;

            trajectoryLine.positionCount = 0; 

            Shoot();
        }
        if (Input.GetKey(KeyCode.A))
        {
            aimPivot.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            aimPivot.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
        
    }

    

    void Shoot()
    {
        stroke++;

        Debug.Log("Shoot");

        Debug.Log("Before : " + followCamera.gameObject.activeSelf);

        aimCamera.gameObject.SetActive(false);
        followCamera.gameObject.SetActive(true);

        Debug.Log("After : " + followCamera.gameObject.activeSelf);

        isBallMoving = true;

        Vector3 shootDirection = aimPivot.forward;
        shootDirection.y = 0;
        shootDirection.Normalize();

        Vector3 finalDirection =
            shootDirection * currentPower
            + Vector3.up * upwardPower;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(finalDirection, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        friction = 0.99f;
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            Vector3 horizontalVelocity =
                new Vector3(
                    rb.linearVelocity.x,
                    0,
                    rb.linearVelocity.z
                );

            horizontalVelocity *= friction;

            rb.linearVelocity =
                new Vector3(
                    horizontalVelocity.x,
                    rb.linearVelocity.y,
                    horizontalVelocity.z
                );
        }
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            stopTimer += Time.fixedDeltaTime;

            if (isBallMoving && stopTimer > 1f)
            {
                isBallMoving = false;

                cameraFollow.SetAimView();

                aimCamera.gameObject.SetActive(true);
                followCamera.gameObject.SetActive(false);

                Debug.Log("Aim Camera 복귀");
            }
        }
        else
        {
            stopTimer = 0f;
        }

        if (windManager != null &&
            rb.linearVelocity.magnitude > 0.1f)
        {
            rb.AddForce(windManager.windForce);
        }

        
        if(transform.position.y < 80f)
        {
            Debug.Log("Out Of Bounds");

            stroke++;

            transform.position = startPosition;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void DrawTrajectory()
    {
        int points = 30;
        trajectoryLine.positionCount = points;

        Vector3 startPos = transform.position;

        Vector3 shootDirection = aimPivot.forward;
        shootDirection.y = 0;
        shootDirection.Normalize();

        Vector3 velocity =
            shootDirection * currentPower
            + Vector3.up * upwardPower; 

        for (int i = 0; i < points; i++)
        {
            float t = i * 0.1f;
            Vector3 point =
                startPos
                + velocity * t
                + 0.5f * Physics.gravity * t * t
                + 0.5f * windManager.windForce * t * t;
            
            trajectoryLine.SetPosition(i, point);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            Vector3 velocity = rb.linearVelocity;

            if (velocity.y < -1f)
            {
                velocity.y = -velocity.y * 0.3f;

                rb.linearVelocity = velocity;
            }
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("Water Hazard");

            stroke++;

            transform.position = startPosition;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 reflect =
                Vector3.Reflect(
                    rb.linearVelocity,
                    collision.contacts[0].normal
                );

            rb.linearVelocity = reflect * 0.8f;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fairway"))
        {
            friction = 0.995f;
        }

        if (collision.gameObject.CompareTag("Rough"))
        {
            friction = 0.98f;
        }

        if (collision.gameObject.CompareTag("Sand"))
        {
            friction = 0.95f;
        }
    }

}

