using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorClientes : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Clientes utilizados en escena")]
    public List<GameObject> poolDeClientes;
    
    [Tooltip("puntos donde puede aparecer")]
    public Transform[] puntosDeSpawn;

    [Header("Configuración de Tiempos")]
    public float tiempoMinimoSpawn = 10f;
    public float tiempoMaximoSpawn = 30f;

    [Header("Estado del Local")]
    public bool localAbierto = true;

    void Start()
    {
        // Apagamos todos los clientes al inicio por las dudas, para que no arranquen comprando
        foreach (GameObject cliente in poolDeClientes)
        {
            cliente.SetActive(false);
        }

        // Arrancamos el ciclo de aparición
        StartCoroutine(RutinaGeneracion());
    }

    IEnumerator RutinaGeneracion()
    {
        // Un pequeño delay inicial para que el local no se llene al segundo 0
        yield return new WaitForSeconds(2f);

        while (localAbierto)
        {
            // 1. Buscamos a alguien que esté libre (apagado)
            GameObject clienteDisponible = ObtenerClienteInactivo();

            if (clienteDisponible != null)
            {
                if (puntosDeSpawn.Length > 0)
                {
                    int indiceAleatorio = Random.Range(0, puntosDeSpawn.Length);
                    Transform spawnElegido = puntosDeSpawn[indiceAleatorio];
                    clienteDisponible.transform.position = spawnElegido.position;
                    clienteDisponible.SetActive(true); // Lo encendemos (esto hará que su Start() y rutinas se activen de nuevo)
                    
                    Debug.Log($"<color=green><b>[Puerta]</b></color> Un cliente entró al local.");
                }
            }
            else
            {
                Debug.LogWarning("¡El local está llenísimo! No hay NPCs libres en el pool.");
            }

            // 3. Esperamos un tiempo aleatorio antes de dejar entrar al siguiente
            float tiempoEspera = Random.Range(tiempoMinimoSpawn, tiempoMaximoSpawn);
            yield return new WaitForSeconds(tiempoEspera);
        }
    }

    // Busca en la lista el primer cliente que esté desactivado
    private GameObject ObtenerClienteInactivo()
    {
        foreach (GameObject cliente in poolDeClientes)
        {
            if (!cliente.activeInHierarchy)
            {
                return cliente;
            }
        }
        return null; // Si todos están comprando, devuelve null
    }

    // Tu AdministradorTiempo puede llamar a esta función a las 7 PM
    public void CerrarLocal()
    {
        localAbierto = false;
        Debug.Log("La puerta se cerró. Ya no entran más clientes.");
    }
}

