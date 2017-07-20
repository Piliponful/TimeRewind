using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Player : MonoBehaviour {
    Camera cam;
    Main main;
    Animator animator;

    public float movementSpeed = 5, rotationSpeed = 60, forceMagnitude = 30000, forcePushEnemy = 1000; GameObject forceUI;

    float walkSpeed = 2f, runSpeed = 6f;

    float turnSmoothTime = .15f;
    float turnSmoothVelocity;

    float speedSmoothTime = .1f;
    float speedSmoothVelocity;
    float currentSpeed;

    void Start ()
    {
        forceUI = GameObject.Find("Force");
        forceUI.GetComponent<Text>().text = forceMagnitude.ToString();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GetComponentInChildren<Camera>();
        main = FindObjectOfType<Main>();
        animator = GetComponent<Animator>();
    }
	
	void Update ()
    {
        Vector2 rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")) * rotationSpeed * Time.deltaTime;
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = (running ? runSpeed : walkSpeed) * dir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        if(dir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector2.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float speedNormalized = (running ? 1 : .5f) * dir.magnitude;
        // Set animation type
        animator.SetFloat("speed", speedNormalized, speedSmoothTime, Time.deltaTime);

        cam.transform.Rotate(0, rotation.x, 0, Space.World);
        cam.transform.Rotate(rotation.y, 0, 0, Space.Self);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit h;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out h))
            {
                GameObject something = h.collider.gameObject;

                if(h.collider.gameObject.tag == "building")
                {
                    BlockScheme b = Town.buildings[something].setBlockInvisible(h.point);
                    Destroy(something);
                    GameObject newBuilding = Town.buildings[something].reBuild();
                    Town.buildings[something].splitBlock(b);
                    BlockRewind.block = b;
                    BlockRewind.building = Town.buildings[something];
                    BlockRewind.buildingGO = newBuilding;
                    BlockRewind.isRecording = true;
                    addForce(h.point);
                }
                if(h.collider != null && something.tag == "enemy_body_part")
                {
                    something.transform.root.GetComponent<Enemy>().kill(forcePushEnemy, h.point);
                    //h.collider.gameObject.GetComponent<Enemy_Part>().push(forcePushEnemy, h.point);
                }
            }
        }

        if (Input.GetKey(KeyCode.Z))
        {
            forceMagnitude += 100;
            forceUI.GetComponent<Text>().text = forceMagnitude.ToString();
        }

        if (Input.GetKey(KeyCode.X))
        {
            forceMagnitude -= 100;
            forceUI.GetComponent<Text>().text = forceMagnitude.ToString();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            cam.GetComponent<MotionBlur>().enabled = !GetComponent<MotionBlur>().enabled;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

	}
    void addForce(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 10);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(forceMagnitude, position, 2, 3.0F);

        }
    }
}
