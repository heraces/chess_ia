using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class piece : MonoBehaviour
{
    public int posX;//0 lado blanco, 7 lado negro
    public int posY; // 0->7 == a->h
    public static_stuff.pieza_color pieza_Color;
    public List<(int, int)> casillas_de_ataque = new List<(int, int)>();
    public bool king = false;
    public int value = 1;
    public virtual List<(int,int)> movimientos_posibles(bool[][] pos_ocupadas)
    {
        return null;
    }

    public bool isthisYou(int x, int y)
    {
        return posX == x && posY == y;
    }
    public virtual void actualizar_ataque(bool[][] pos_ocupadas)
    {
    }

}
