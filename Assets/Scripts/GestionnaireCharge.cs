using UnityEngine;

/// <summary>
/// Gestionnaire de charge qui gère les objets de charge disponibles pour le joueur et leur utilisation.
/// </summary>
public class GestionnaireCharge : MonoBehaviour
{
    [SerializeField, Tooltip("Référence aux objets de charge que le joueur peut utiliser.")]
    private GameObject[] objetsCharge;

    // Le nombre de charges disponibles pour le joueur.
    private int nombreCharges;

    public void AjouterCharge()
    {
        if(nombreCharges < objetsCharge.Length)
        {
            nombreCharges++;
            objetsCharge[nombreCharges - 1].SetActive(true);
        }
    }


    public bool UtiliserCharge()
    {
        if(nombreCharges == 0)
        {
            return false;
        }

        nombreCharges--;
        objetsCharge[nombreCharges].SetActive(false);
        return true;
    }
}
