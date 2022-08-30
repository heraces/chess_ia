using System.Collections;
using System.Collections.Generic;

public class chess_logic
{
    private bool[][] casillas_ocupadas; //used for movement
    private bool[][] casillas_ocupadas_blancas; //used for attack
    private bool[][] casillas_ocupadas_negras;
    private List<piece> casillas_negras;
    private int enJaque; // 0 no, 1, jaque favor blancas, 2 jaque favor negras

    private int posibleMoves;

    public void inizialice()
    {
        casillas_ocupadas = new bool[8][];
        casillas_ocupadas_blancas = new bool[8][];
        casillas_ocupadas_negras = new bool[8][];
        enJaque = 0;

        for (int i = 0; i < 8; i++)
        {

            bool[] s = new bool[8];
            bool[] r = new bool[8];
            bool[] c = new bool[8];
            for (int x = 0; x < 8; x++)
            {
                s[x] = false;
                r[x] = false;
                c[x] = false;
            }
            casillas_ocupadas[i] = s;
            casillas_ocupadas_blancas[i] = r;
            casillas_ocupadas_negras[i] = c;
        }
    }

    public void editPiece(int posX, int posY, bool esta)
    {
        casillas_ocupadas[posX][posY] = esta;
    }

    public void editWhitePiece(int posX, int posY, bool esta)
    {
        casillas_ocupadas_blancas[posX][posY] = esta;
    }

    public void editBlackPiece(int posX, int posY, bool esta)
    {
        casillas_ocupadas_blancas[posX][posY] = esta;
    }
}
