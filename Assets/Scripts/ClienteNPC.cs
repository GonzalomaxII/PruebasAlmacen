using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClienteNPC : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float velocidad = 3f;
    public float toleranciaLlegada = 0.05f;

    private Rigidbody2D rb;
    private Animator animator;

    private int isWalkingHash, inputXHash, inputYHash, lastInputXHash, lastInputYHash;
    private Vector2 ultimaDireccion = Vector2.down;

    // --- EL CEREBRO LEERÁ ESTO PARA SABER SI YA LLEGÓ ---
    public bool estaMoviendose { get; private set; } 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        inputXHash = Animator.StringToHash("InputX");
        inputYHash = Animator.StringToHash("InputY");
        lastInputXHash = Animator.StringToHash("LastInputX");
        lastInputYHash = Animator.StringToHash("LastInputY");
    }

    public void IrA(Vector2Int celdaDestino)
    {
        Vector2Int celdaActual = MapaCliente.Instance.MundoACelda(transform.position);
        List<Vector2Int> camino = Pathfinding.Instance.FindPath(celdaActual, celdaDestino);

        if (camino == null || camino.Count == 0)
        {
            Debug.LogWarning("El cuerpo no encontró camino al destino");
            estaMoviendose = false;
            return;
        }

        StopAllCoroutines(); // Detiene cualquier movimiento anterior
        StartCoroutine(SeguirCamino(camino));
    }

    private IEnumerator SeguirCamino(List<Vector2Int> camino)
    {
        estaMoviendose = true; // Avisa que arrancó
        if (animator != null) animator.SetBool(isWalkingHash, true);

        int inicio = camino.Count > 1 ? 1 : 0;

        for (int i = inicio; i < camino.Count; i++)
        {
            Vector3 destino = MapaCliente.Instance.CeldaAMundo(camino[i].x, camino[i].y);

            while (Vector2.Distance(transform.position, destino) > toleranciaLlegada)
            {
                Vector2 direccion = ((Vector2)destino - rb.position).normalized;
                ultimaDireccion = direccion;

                rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);

                if (animator != null)
                {
                    animator.SetFloat(inputXHash, direccion.x);
                    animator.SetFloat(inputYHash, direccion.y);
                }

                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(destino);
        }

        if (animator != null)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetFloat(lastInputXHash, ultimaDireccion.x);
            animator.SetFloat(lastInputYHash, ultimaDireccion.y);
        }
        
        estaMoviendose = false; // Avisa que ya llegó
    }
}