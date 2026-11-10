using UnityEngine;
using TMPro;

public class AdministradorTiempo : MonoBehaviour
{
    [Header("Configuración del Horario")]
    [Tooltip("Hora a la que abre el local (Formato 24h)")]
    public int horaApertura=7;

    [Tooltip("Hora a la que cierra el local (Formato 24h)")]
    public int horaCierre=19;

    [Header("Interfa usuario")]
    public TMP_Text textoReloj;

    private float minutosActuales;
    private int horaActual;
    private bool turnoFinalizado=false;

    void Start()
    {
        // El día arranca a la hora de apertura
        horaActual=horaApertura;
        minutosActuales=0f;
        
        ActualizarPantallaReloj();
    }
    void Update()
    {
        if (turnoFinalizado) return; // Si ya son las 7 PM, el reloj se detiene

        minutosActuales += Time.deltaTime; // Como 1 segundo real = 1 minuto de juego, sumamos el tiempo real directamente a los minutos

        if (minutosActuales>=60f)
        {
            minutosActuales-=60f;
            horaActual++;

            if(horaActual >= horaCierre)
            {
                horaActual=horaCierre;
                minutosActuales=0f;
                FinalizarTurno();
            }
        }
        ActualizarPantallaReloj();
    }

    void ActualizarPantallaReloj()
    {
        if (textoReloj == null) return;

        int minutosEnteros = Mathf.FloorToInt(minutosActuales);
        // Lógica para formato AM/PM (Reloj de 12 horas en pantalla, aunque internamente usemos 24h)
        string amPm = horaActual >= 12 ? "PM" : "AM";
        int horaMostrar = horaActual > 12 ? horaActual - 12 : horaActual;
        if (horaMostrar == 0) horaMostrar = 12; // Caso especial para las 12 del mediodía
        textoReloj.text = string.Format("{0:00}:{1:00} {2}", horaMostrar, minutosEnteros, amPm); // Formatea el texto para que siempre tenga 2 dígitos (ej: 07:05 AM)
    }

    void FinalizarTurno()
    {
        turnoFinalizado = true;
        Debug.Log("<color=yellow><b>[Reloj]</b></color> ¡Son las 7 PM! El turno ha terminado.");

        // TODO: Acá más adelante pondremos la lógica para cerrar el nivel,
        // frenar el GeneradorClientes y calcular el dinero ganado.
    }
}
