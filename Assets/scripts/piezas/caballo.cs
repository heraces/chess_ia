using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caballo : piece
{
    private void Start()
    {
        value = 3;
    }
    public override List<(int, int)> movimientos_posibles(bool[][] pos_ocupadas)
    {
        List<(int, int)> movimientos = new List<(int, int)>();

        if (posX - 2 >= 0 && posY - 1 >= 0 && !pos_ocupadas[posX - 2][posY - 1])
        {
            movimientos.Add((posX - 2, posY - 1));
        }
        if (posX - 2 >= 0 && posY + 1 <= 7 && !pos_ocupadas[posX - 2][posY + 1])
        {
            movimientos.Add((posX - 2, posY + 1));
        }
        if (posX + 2 <= 7 && posY - 1 >= 0 && !pos_ocupadas[posX + 2][posY - 1])
        {
            movimientos.Add((posX + 2, posY - 1));
        }
        if (posX + 2 <= 7 && posY + 1 <= 7 && !pos_ocupadas[posX + 2][posY + 1])
        {
            movimientos.Add((posX + 2, posY + 1));
        }

        if (posX - 1 >= 0 && posY - 2 >= 0 && !pos_ocupadas[posX - 1][posY - 2])
        {
            movimientos.Add((posX - 1, posY - 2));
        }
        if (posX + 1 <= 7 && posY - 2 >= 0 && !pos_ocupadas[posX + 1][posY - 2])
        {
            movimientos.Add((posX + 1, posY - 2));
        }
        if (posX - 1 >= 0 && posY + 2 <= 7 && !pos_ocupadas[posX - 1][posY + 2])
        {
            movimientos.Add((posX - 1, posY + 2));
        }
        if (posX + 1 <= 7 && posY + 2 <= 7 && !pos_ocupadas[posX + 1][posY + 2])
        {
            movimientos.Add((posX + 1, posY + 2));
        }
        return movimientos;
    }

    public override void actualizar_ataque(bool[][]s)
    {
        casillas_de_ataque.Clear();
        casillas_de_ataque.Add((posX + 2, posY + 1));
        casillas_de_ataque.Add((posX + 2, posY - 1));
        casillas_de_ataque.Add((posX - 2, posY + 1));
        casillas_de_ataque.Add((posX - 2, posY - 1));
        casillas_de_ataque.Add((posX + 1, posY + 2));
        casillas_de_ataque.Add((posX - 1, posY + 2));
        casillas_de_ataque.Add((posX + 1, posY - 2));
        casillas_de_ataque.Add((posX - 1, posY - 2));
    }
}
