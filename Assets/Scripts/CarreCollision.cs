using UnityEngine;

public class CarreCollision : MonoBehaviour
{
    [SerializeField]
    private int numeroCarre; 

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

        Vector3 positionPiece = transform.position + new Vector3(0, 0.05f, 0);
        pieceActuelle = Instantiate(prefab, positionPiece, Quaternion.identity);

        GameController.Instance.CoupJoue(numeroCarre);

    }

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