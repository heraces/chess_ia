using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alfil : piece
{
    private void Start()
    {
        value = 3;
    }
    public override List<(int,int)> movimientos_posibles(bool[][] pos_ocupadas)
    {
        List<(int, int)> movimientos = new List<(int, int)>();

        int traceX = posX+1;
        int traceY = posY+1;
        while (traceX <= 7 && traceY <=7 && !pos_ocupadas[traceX][traceY])
        {
            movimientos.Add((traceX, traceY));
            traceX++;
            traceY++;
        }
        traceX = posX+1;
        traceY = posY-1;
        while (traceX <= 7 && traceY >= 0 && !pos_ocupadas[traceX][traceY])
        {
            movimientos.Add((traceX, traceY));
            traceX++;
            traceY--;
        }

        traceX = posX-1;
        traceY = posY+1;
        while (traceX >= 0 && traceY <= 7 && !pos_ocupadas[traceX][traceY])
        {
            movimientos.Add((traceX, traceY));
            traceX--;
            traceY++;
        }
        traceX = posX-1;
        traceY = posY-1;
        while (traceX >=0 && traceY >=0 && !pos_ocupadas[traceX][traceY])
        {
            movimientos.Add((traceX, traceY));
            traceX--;
            traceY--;
        }

        return movimientos;
    }

    public override void actualizar_ataque(bool[][] pos_ocupadas)
    {
        casillas_de_ataque.Clear();

        int traceX = posX + 1;
        int traceY = posY + 1;
        while (traceX <= 7 && traceY <= 7 && !pos_ocupadas[traceX][traceY])
        {
            traceX++;
            traceY++;
        }

        if (traceX <= 7 && traceX <= 7)
        {
            casillas_de_ataque.Add((traceX, traceY));
        }

        traceX = posX + 1;
        traceY = posY - 1;
        while (traceX <= 7 && traceY >= 0 && !pos_ocupadas[traceX][traceY])
        {
            traceX++;
            traceY--;
        }

        if (traceX <= 7 && traceX >= 0)
        {
            casillas_de_ataque.Add((traceX, traceY));
        }

        traceX = posX - 1;
        traceY = posY + 1;
        while (traceX >= 0 && traceY <= 7 && !pos_ocupadas[traceX][traceY])
        {
            traceX--;
            traceY++;
        }
        if (traceX >= 0 && traceX <= 7)
        {
            casillas_de_ataque.Add((traceX, traceY));
        }

        traceX = posX - 1;
        traceY = posY - 1;
        while (traceX >= 0 && traceY >= 0 && !pos_ocupadas[traceX][traceY])
        {
            traceX--;
            traceY--;
        }
        if (traceX >= 0 && traceX >= 0)
        {
            casillas_de_ataque.Add((traceX, traceY));
        }
    }

}
