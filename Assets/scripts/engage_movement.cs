using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engage_movement : MonoBehaviour
{
    private layout layout;
    private int posX;
    private int posY;
    public void OnMouseUp()
    {
        layout.movePieceForReal(posX, posY);
    }

    public void getLayout(layout layout, int posX, int posY)
    {
        this.layout = layout;
        this.posX = posX;
        this.posY = posY;
    }
    public (int,int) getPos()
    {
        return (posX, posY);
    }
}
