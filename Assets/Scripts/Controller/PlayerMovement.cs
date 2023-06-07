using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    public GameObject playerObject;
    public BoxCollider2D boxColldier;
    public Rigidbody2D Rig2D;
    public Camera cam;
    public Animator anim;
    private bool bRunnning;

    // Start is called before the first frame update
    void Start()
    {
        Rig2D = GetComponent<Rigidbody2D>();   
        boxColldier = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
            anim.SetBool("IsRun", true);
        else
            anim.SetBool("IsRun", false);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 moveVec = new Vector3(0f, 1f, 0f) * 10f;


            playerObject.transform.localPosition += (moveVec * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        { 
            Vector3 moveVec = new Vector3(0f, -1f, 0f) * 10f;


            playerObject.transform.localPosition += (moveVec * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 moveVec = new Vector3(-1f, 0f, 0f) * 10f;

            playerObject.transform.localPosition += (moveVec * Time.deltaTime);
        }


        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 moveVec = new Vector3(1f, 0f, 0f) * 10f;

            playerObject.transform.localPosition += (moveVec * Time.deltaTime);
        }

    }
}
