using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rey : piece
{
    private void Awake()
    {
        king = true;
        value = 4;
    }
    public override List<(int, int)> movimientos_posibles(bool[][] pos_ocupadas)
    {
        List<(int, int)> movimientos = new List<(int, int)>();
        if (posX - 1 >= 0 && posY - 1 >= 0 && !pos_ocupadas[posX - 1][posY - 1])
        {
            movimientos.Add((posX - 1, posY - 1));
        }
        if (posX - 1 >= 0 && !pos_ocupadas[posX - 1][posY])
        {
            movimientos.Add((posX - 1, posY));
        }
        if (posX -1 >= 0 && posY + 1 <= 7 && !pos_ocupadas[posX -1 ][posY + 1])
        {
            movimientos.Add((posX -1 , posY + 1));
        }
        if (posY -1 >= 0 && !pos_ocupadas[posX][posY - 1])
        {
            movimientos.Add((posX, posY - 1));
        }
        if (posY + 1 <= 7 && !pos_ocupadas[posX][posY + 1])
        {
            movimientos.Add((posX, posY +1));
        }
        if (posX+1 <= 7 && posY +1  <= 7 && !pos_ocupadas[posX +1][posY +1])
        {
            movimientos.Add((posX +1, posY+1));
        }
        if (posX + 1 <= 7 && !pos_ocupadas[posX +1][posY])
        {
            movimientos.Add((posX + 1, posY));
        }
        if (posX + 1 <= 7 && posY -1 >= 0 && !pos_ocupadas[posX + 1][posY -1])
        {
            movimientos.Add((posX + 1, posY -1));
        }

        return movimientos;
    }

    public override void actualizar_ataque(bool[][]x)
    {
        casillas_de_ataque.Clear();
        if (posX - 1 >= 0 && posY - 1 >= 0)
        {
            casillas_de_ataque.Add((posX - 1, posY - 1));
        }
        if (posX - 1 >= 0)
        {
            casillas_de_ataque.Add((posX - 1, posY));
        }
        if (posX - 1 >= 0 && posY + 1 <= 7)
        {
            casillas_de_ataque.Add((posX - 1, posY + 1));
        }
        if (posY - 1 >= 0)
        {
            casillas_de_ataque.Add((posX, posY - 1));
        }
        if (posY + 1 <= 7)
        {
            casillas_de_ataque.Add((posX, posY + 1));
        }
        if (posX + 1 <= 7 && posY + 1 <= 7)
        {
            casillas_de_ataque.Add((posX + 1, posY + 1));
        }
        if (posX + 1 <= 7)
        {
            casillas_de_ataque.Add((posX + 1, posY));
        }
        if (posX + 1 <= 7 && posY - 1 >= 0)
        {
            casillas_de_ataque.Add((posX + 1, posY - 1));
        }
    }
}
