using UnityEngine;
using System.Collections;
public class Delivery : MonoBehaviour
{ 
    public IniciarPc ScriptMenu;
    public GameObject Deliver;
    public GameObject UI;
    public void AlDelivery()
    {
        StartCoroutine(Espera(Random.Range(9,11)));
        
    }

    public void Salir()
    {
        
        var canvas = UI.GetComponent<Canvas>();
        canvas.enabled = true;
        ScriptMenu.MenuAbierto=false;
        Deliver.SetActive(false);
    }

    IEnumerator Espera(float duration)
    {
    Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
    yield return new WaitForSeconds(duration);
    Debug.Log($"Ended at {Time.time}"); 
    }
}
