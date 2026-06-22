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

    // --- Parámetros del Animator (Hashes para optimizar) ---
    private int isWalkingHash;
    private int inputXHash;
    private int inputYHash;
    private int lastInputXHash;
    private int lastInputYHash;

    // Guardamos la última dirección para cuando el NPC se detenga
    private Vector2 ultimaDireccion = Vector2.down; // Por defecto mira hacia abajo

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Convertimos los nombres exactos de tus parámetros en IDs de Unity
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
            Debug.LogWarning("No se encontró camino al destino");
            return;
        }

        StopCoroutine(nameof(SeguirCamino));
        StartCoroutine(SeguirCamino(camino));
    }

    private IEnumerator SeguirCamino(List<Vector2Int> camino)
    {
        // 1. EQUIVALENTE A: animator.SetBool("isWalking", true); de tu jugador
        if (animator != null) animator.SetBool(isWalkingHash, true);

        int inicio = 0;
        if (camino.Count > 1) inicio = 1;

        for (int i = inicio; i < camino.Count; i++)
        {
            Vector3 destino = MapaCliente.Instance.CeldaAMundo(camino[i].x, camino[i].y);

            while (Vector2.Distance(transform.position, destino) > toleranciaLlegada)
            {
                // Obtenemos el "Input" del NPC (su dirección matemática)
                Vector2 direccion = ((Vector2)destino - rb.position).normalized;
                
                // Guardamos esta dirección constantemente para saber a dónde miraba antes de parar
                ultimaDireccion = direccion;

                // Movemos físicamente al NPC
                rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);

                // 2. EQUIVALENTE A: animator.SetFloat("InputX" / "InputY"); de tu jugador
                if (animator != null)
                {
                    animator.SetFloat(inputXHash, direccion.x);
                    animator.SetFloat(inputYHash, direccion.y);
                }

                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(destino);
        }

        // 3. EQUIVALENTE A: context.canceled de tu jugador
        if (animator != null)
        {
            animator.SetBool(isWalkingHash, false);
            
            // Le pasamos la última dirección al Blend Tree Idle para que quede mirando hacia allá
            animator.SetFloat(lastInputXHash, ultimaDireccion.x);
            animator.SetFloat(lastInputYHash, ultimaDireccion.y);
        }
    }
}