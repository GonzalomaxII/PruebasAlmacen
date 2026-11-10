using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class BultoAlmacenado
{
    public string tipoProducto;
    public int unidadesRestantes; // Oculto intencionalmente al jugador

    public BultoAlmacenado(string tipo, int unidades)
    {
        tipoProducto = tipo;
        unidadesRestantes = unidades;
    }
}

public class CodigoCaja : MonoBehaviour, interacuar
{
    [Header("Referencias Generales")]
    public IniciarPc ScriptMenu;
    public Valores Jugador;

    [Header("Configuración de Almacenamiento")]
    public int limiteCajas = 6;
    public List<BultoAlmacenado> cajasAlmacenadas = new List<BultoAlmacenado>();

    [Header("UI - Mensaje Informativo (PopUp)")]
    public GameObject PopUpMensaje;
    public TextMeshProUGUI TextoMensaje;

    [Header("UI - Panel de Selección")]
    public GameObject PanelInventarioUI;
    [Tooltip("Arrastrá acá tus 6 botones creados a mano en el panel")]
    public Button[] botonesSlots = new Button[6];

    [Header("Botón Salir Compartido")]
    [Tooltip("Arrastrá acá el botón rojo con la 'X'")]
    public Button botonCerrarPanel;

    public bool puedeinteract()
    {
        if (ScriptMenu == null) return true;
        return !ScriptMenu.MenuAbierto;
    }

    public void interact()
    {
        if (!puedeinteract()) return;

        // 1. Si lleva una caja encima -> Depositar
        if (Jugador.tieneCaja || Jugador.cantidad > 0)
        {
            DepositarCaja();
        }
        // 2. Si tiene las manos libres -> Abrir el panel de este mueble
        else
        {
            AbrirPanelDeRetiro();
        }
    }

    private void DepositarCaja()
    {
        if (cajasAlmacenadas.Count >= limiteCajas)
        {
            MostrarAviso("Este depósito está lleno.\n(Límite: 6 cajas)");
            return;
        }

        string tipo = Jugador.tipoCaja;
        int unidades = Jugador.unidadesCaja;

        // Compatibilidad por si faltaba asignar tipoCaja
        if (string.IsNullOrEmpty(tipo))
        {
            if (Jugador.enlatados > 0) { tipo = "Enlatados"; unidades = Jugador.enlatados; }
            else if (Jugador.especias > 0) { tipo = "Especias"; unidades = Jugador.especias; }
            else if (Jugador.carne > 0) { tipo = "Carne"; unidades = Jugador.carne; }
            else if (Jugador.pasta > 0) { tipo = "Pasta"; unidades = Jugador.pasta; }
            else if (Jugador.pizza > 0) { tipo = "Pizza"; unidades = Jugador.pizza; }
            else if (Jugador.fritos > 0) { tipo = "Fritos"; unidades = Jugador.fritos; }
        }

        cajasAlmacenadas.Add(new BultoAlmacenado(tipo, unidades));

        // Vaciamos el inventario del jugador
        Jugador.tieneCaja = false;
        Jugador.tipoCaja = "";
        Jugador.unidadesCaja = 0;
        Jugador.cantidad = 0;
        Jugador.enlatados = 0;
        Jugador.especias = 0;
        Jugador.carne = 0;
        Jugador.pasta = 0;
        Jugador.pizza = 0;
        Jugador.fritos = 0;

        MostrarAviso($"Guardaste una caja de {tipo.ToLower()} en este depósito.\nCajas guardadas: {cajasAlmacenadas.Count}/{limiteCajas}");
    }

    private void AbrirPanelDeRetiro()
    {
        if (cajasAlmacenadas.Count == 0)
        {
            MostrarAviso("Este depósito está vacío.");
            return;
        }

        if (PanelInventarioUI == null) return;

        if (ScriptMenu != null) ScriptMenu.MenuAbierto = true;
        PanelInventarioUI.SetActive(true);

        // Conectar el botón de salir 'X'
        if (botonCerrarPanel != null)
        {
            botonCerrarPanel.onClick.RemoveAllListeners();
            botonCerrarPanel.onClick.AddListener(CerrarPanelDeRetiro);
        }

        // Actualizamos los 6 slots fijos
        for (int i = 0; i < botonesSlots.Length; i++)
        {
            if (botonesSlots[i] == null) continue;

            // Si hay una caja guardada en esta posición
            if (i < cajasAlmacenadas.Count)
            {
                int indice = i;
                BultoAlmacenado bulto = cajasAlmacenadas[indice];

                botonesSlots[i].gameObject.SetActive(true);

                // Mostramos únicamente el nombre de la caja (sin cantidades)
                TextMeshProUGUI label = botonesSlots[i].GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = $"Caja de {bulto.tipoProducto}";
                }

                // Asignamos la acción al hacer clic
                botonesSlots[i].onClick.RemoveAllListeners();
                botonesSlots[i].onClick.AddListener(() => SeleccionarCaja(indice));
            }
            else
            {
                // Si no hay caja para este slot, apagamos el botón
                botonesSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void SeleccionarCaja(int indice)
    {
        if (indice < 0 || indice >= cajasAlmacenadas.Count) return;

        BultoAlmacenado bultoElegido = cajasAlmacenadas[indice];
        cajasAlmacenadas.RemoveAt(indice);

        // Se asigna la caja al jugador
        Jugador.tieneCaja = true;
        Jugador.tipoCaja = bultoElegido.tipoProducto;
        Jugador.unidadesCaja = bultoElegido.unidadesRestantes;
        Jugador.cantidad = bultoElegido.unidadesRestantes;

        CerrarPanelDeRetiro();
        MostrarAviso($"Agarraste una caja de {bultoElegido.tipoProducto.ToLower()}.\nLlevala al estante para reponer.");
    }

    private void MostrarAviso(string mensaje)
    {
        if (ScriptMenu != null) ScriptMenu.MenuAbierto = true;
        if (PopUpMensaje != null) PopUpMensaje.SetActive(true);
        if (TextoMensaje != null) TextoMensaje.text = mensaje;
    }

    public void CerrarAviso()
    {
        if (PopUpMensaje != null) PopUpMensaje.SetActive(false);
        if (ScriptMenu != null) ScriptMenu.MenuAbierto = false;
    }

    public void CerrarPanelDeRetiro()
    {
        if (PanelInventarioUI != null) PanelInventarioUI.SetActive(false);
        if (ScriptMenu != null) ScriptMenu.MenuAbierto = false;
    }
}