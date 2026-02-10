using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField]
    private TextMeshProUGUI instructionsText;
    [SerializeField]
    private TextMeshProUGUI tourText;
    [SerializeField]
    private TextMeshProUGUI victoireText;

    public int tour = 0;
    public int nbCoupsMax = 9;

    private int[,] grille = new int[3, 3];
    private bool partieTerminee = false;

    private void Awake()
    {
        //Singleton pour avoir accès dans l'autre script (Placement.cs)
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitialiserGrille();
    }

    private void InitialiserGrille()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grille[i, j] = 0;
            }
        }
    }

    public bool EstTourX()
    {
        return tour % 2 == 0;
    }

    public void CoupJoue(int numeroCarre)
    {
        int ligne = (numeroCarre - 1) / 3;
        int colonne = (numeroCarre - 1) % 3;

        if (partieTerminee || grille[ligne, colonne] != 0)
        {
            return;
        }

        grille[ligne, colonne] = EstTourX() ? 1 : 2;

        tour++;

        if (VerifierVictoire())
        {
            partieTerminee = true;
            AfficherVictoire();
        }
        else if (tour >= nbCoupsMax)
        {
            partieTerminee = true;
            AfficherEgalite();
        }
        else
        {
            MettreAJourAffichage();
        }
    }

    private bool VerifierVictoire()
    {
        int joueur = EstTourX() ? 2 : 1;

        //Vérifier les lignes
        for (int i = 0; i < 3; i++)
        {
            if (grille[i, 0] == joueur && grille[i, 1] == joueur && grille[i, 2] == joueur)
            {
                return true;
            }

        }

        //Vérifier les colonnes
        for (int j = 0; j < 3; j++)
        {
            if (grille[0, j] == joueur && grille[1, j] == joueur && grille[2, j] == joueur)
            {
                return true;
            }
        }

        //Vérifier diagonale principale (\)
        if (grille[0, 0] == joueur && grille[1, 1] == joueur && grille[2, 2] == joueur)
        {
            return true;
        }

        //Vérifier diagonale secondaire (/)
        if (grille[0, 2] == joueur && grille[1, 1] == joueur && grille[2, 0] == joueur)
        {
            return true;
        }

        return false;
    }
    //Code créé avec l'aide de l'intelligence artificielle
    public bool JeuFini()
    {
        return partieTerminee;
    }
    public void Recommencer()
    {
        tour = 0;
        MettreAJourAffichage();
        InitialiserGrille();
        partieTerminee = false;
    }

    private void AfficherVictoire()
    {
        string gagnant = EstTourX() ? "O" : "X";
        victoireText.text = gagnant + " a gagné!";
        instructionsText.text = "Recommencez ou quittez";
        tourText.text = "Terminé";
    }

    private void AfficherEgalite()
    {
        victoireText.text = "Égalité!";
        instructionsText.text = "Recommencez ou quittez";
        tourText.text = "Terminé";
    }

    private void MettreAJourAffichage()
    {
        instructionsText.text = "Placez la pièce sur une case";
        victoireText.text = "";

        if (EstTourX())
        {
            tourText.text = "Tour du X";
        }
        else
        {
            tourText.text = "Tour du O";
        }
    }

    //Savoir si la case est libre
    public bool CaseLibre(int ligne, int colonne)
    {
        return grille[ligne, colonne] == 0 && !partieTerminee;
    }

    public bool CaseLibreParCarre(int numeroCarre)
    {
        int ligne = (numeroCarre - 1) / 3;
        int colonne = (numeroCarre - 1) % 3;
        return CaseLibre(ligne, colonne);
    }
}
