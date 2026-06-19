using UnityEngine;
using UnityEngine.InputSystem;




public class Movimiento : MonoBehaviour
{
    [SerializeField] private float Vel = 5f;
    private Rigidbody2D rb;
    private Vector2 Inputmovimiento;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = Inputmovimiento * Vel;
    }


    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true); //le pasas el nombre del parametro del animator

        if (context.canceled) //condición ocure cuando dejas de tocar una tecla de movimiento
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", Inputmovimiento.x);
            animator.SetFloat("LastInputY", Inputmovimiento.y); 
        }
        Inputmovimiento = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", Inputmovimiento.x);
        animator.SetFloat("InputY", Inputmovimiento.y);
    }
}

