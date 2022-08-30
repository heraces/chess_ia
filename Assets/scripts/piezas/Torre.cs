using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torre : piece
{
    private void Start()
    {
        value = 4;
    }
    public override List<(int, int)> movimientos_posibles(bool[][] pos_ocupadas)
    {
        List<(int, int)> movimientos = new List<(int, int)>();
        int tracer = posX+1;
        while (tracer <= 7 && !pos_ocupadas[tracer][posY])//si false, esta desocupada
        {
            movimientos.Add((tracer, posY));
            tracer++;
        }

        tracer = posX-1;
        while (tracer >= 0 && !pos_ocupadas[tracer][posY])
        {
            movimientos.Add((tracer, posY));
            tracer--;
        }
        tracer = posY+1;
        while (tracer <= 7 && !pos_ocupadas[posX][tracer])
        {
            movimientos.Add((posX, tracer));
            tracer++;
        }
        tracer = posY-1;
        while (tracer >= 0 && !pos_ocupadas[posX][tracer])
        {
            movimientos.Add((posX, tracer));
            tracer--;
        }

        return movimientos;
    }

    public override void actualizar_ataque(bool[][] pos_ocupadas)
    {
        casillas_de_ataque.Clear();
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
    }
}
