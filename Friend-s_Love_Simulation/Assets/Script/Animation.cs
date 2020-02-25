using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    private Animator animator;
    public ButtonManager buttonManager;
    public GameObject Button;
    public GameObject Canvas2;
    public bool Click;
    private bool WaitFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Click = buttonManager.Click;

        if (Click == true)
        {
            animator.SetBool("appear", true);
            Button.SetActive(false);
            if (WaitFlag == false)
            {
                StartCoroutine("Wait");
                WaitFlag = true;
            }
        }

        if(Click == false)
        {
            Button.SetActive(true);
            Canvas2.SetActive(false);
            animator.SetBool("appear", false);
            WaitFlag = false;
        }
        
    }

    private IEnumerator Wait()
    { 
        yield return new WaitForSeconds(0.25f);
        Canvas2.SetActive(true);
    }
}
