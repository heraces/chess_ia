using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peon_negro : piece
{
    public override List<(int, int)> movimientos_posibles(bool[][] pos_ocupadas)
    {
        List<(int, int)> movimientos = new List<(int, int)>();
        bool puedo_mover = false;
        bool dobleStep = false;

        if (posX <= 0)
        {
            return null;
        }
        else
        {
            if (!pos_ocupadas[posX - 1][posY])
            {
                puedo_mover = true;
                if (posX >= 6 && !pos_ocupadas[posX - 2][posY])
                {
                    dobleStep = true;
                }
            }
        }
        if (puedo_mover)
        {
            movimientos.Add((posX - 1, posY));
        }
        if (dobleStep)
        {
            movimientos.Add((posX - 2, posY));
        }

        return movimientos;
    }

    public override void actualizar_ataque(bool[][] x)
    {
        casillas_de_ataque.Clear();
        casillas_de_ataque.Add((posX - 1, posY + 1));
        casillas_de_ataque.Add((posX - 1, posY - 1));
    }
    public void seriousAttack(bool[][] pos_ocupadas)
    {
        casillas_de_ataque.Clear();

        //torre
        int tracer = posX + 1;
        while (tracer <= 7 && !pos_ocupadas[tracer][posY])//si false, esta desocupada
        {
            tracer++;
        }
        if (tracer <= 7)
        {
            casillas_de_ataque.Add((tracer, posY));
        }

        tracer = posX - 1;
        while (tracer >= 0 && !pos_ocupadas[tracer][posY])
        {
            tracer--;
        }
        if (tracer >= 0)
        {
            casillas_de_ataque.Add((tracer, posY));
        }
        tracer = posY + 1;
        while (tracer <= 7 && !pos_ocupadas[posX][tracer])
        {
            tracer++;
        }
        if (tracer <= 7)
        {
            casillas_de_ataque.Add((posX, tracer));
        }
        tracer = posY - 1;
        while (tracer >= 0 && !pos_ocupadas[posX][tracer])
        {
            tracer--;
        }
        if (tracer >= 0)
        {
            casillas_de_ataque.Add((posX, tracer));
        }

        //alfil
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
