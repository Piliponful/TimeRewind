using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Player : MonoBehaviour {

    public float movementSpeed = 5, rotationSpeed = 60, forceMagnitude = 30000, forcePushEnemy = 1000; GameObject forceUI;
    private Main GBAlgManager;
    Camera cam;
    Main main;

    void Start ()
    {
        forceUI = GameObject.Find("Force");
        forceUI.GetComponent<Text>().text = forceMagnitude.ToString();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = FindObjectOfType<Camera>();
        main = FindObjectOfType<Main>();
    }
	
	void Update ()
    {
        float hMove = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float vMove = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float hRotate = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vRotate = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Translate(hMove, 0, vMove);
        transform.Rotate(0, hRotate, 0, Space.World);
        cam.transform.Rotate(-vRotate, 0, 0, Space.Self);

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
                    Town.buildings[something].reBuild();
                    Town.buildings[something].splitBlock(b);
                    //GBAlgManager.splitIntoCubes(h.collider.gameObject);
                    //bc.isRecording = true;
                    //addForce(h.point);
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
