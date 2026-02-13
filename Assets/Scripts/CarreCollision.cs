///Autheur: Breno Gomes da Silva
///Travail pratique - TicTacToe immersif
///Utilisation des notes de cours: https://envimmersif-cegepvicto.github.io/
///Ainsi que projet demonstratif: https://github.com/EnvImmersif-CegepVicto/AR_Demos.git
///Pour la réalisation du projet (Autheur des références: Frédérick Taleb
///Aide de l'intelligence artificielle pour certains concepts du projet 
///(plus de détails dans le ReadMe)

using System.Collections;
using UnityEngine;

public class CarreCollision : MonoBehaviour
{
    [Header("Référence")]
    [SerializeField]
    private int numeroCarre;
    public int NumeroCarre => numeroCarre;

    [SerializeField]
    private GameObject prefabX;
    [SerializeField]
    private GameObject prefabO;

    private GameObject pieceActuelle = null;

    public void PlacerPiece()
    {
        //Vérifier si la case est vide
        if (pieceActuelle != null)
        {
            Debug.Log("Case déjà occupée!");
            return;
        }

        //Vérifier si le jeu est fini
        if (GameController.Instance.JeuFini())
        {
            Debug.Log("Jeu terminé!");
            return;
        }

        //Vérifier si la case est libre dans GameController
        if (!GameController.Instance.CaseLibreParCarre(numeroCarre))
        {
            Debug.Log("Case non disponible!");
            return;
        }

        //Ajouter la bonne pièce dans le carré 
        GameObject prefab = GameController.Instance.EstTourX() ? prefabX : prefabO;

        //Lever un peu la pièce pour la voir comme du monde
        Vector3 positionPiece = transform.position + new Vector3(0, 0.05f, 0);
        pieceActuelle = Instantiate(prefab, positionPiece, Quaternion.identity);

        GameController.Instance.CoupJoue(numeroCarre);

    }

    /// <summary>
    /// S'occupe de l'animation de la ligne gagnante
    /// </summary>
    public void AnimationVictoire()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.yellow;
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.yellow * 2f);
        }
    }

    /// <summary>
    /// Réinitialiser l'apparence du carré
    /// </summary>
    public void ReinitialiserApparence()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.gray;
            rend.material.DisableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.black);
        }
    }
    ///100% généré avec l'IA, je voulais juste une petite animaton fun 
    ///à place d'une fait directment dans Unity



    /// <summary>
    ///Vider la grille
    /// </summary>
    public void ViderCase()
    {
        if (pieceActuelle != null)
        {
            Destroy(pieceActuelle);
            pieceActuelle = null;
        }
    }
}