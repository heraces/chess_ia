using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class layout : MonoBehaviour
{
    //tablero//logica
    private bool[][] casillas_ocupadas;
    private bool[][] casillas_ocupadas_blancas;
    private bool[][] casillas_ocupadas_negras;

    private int enJaque; // 0 no, 1, jaque favor blancas, 2 jaque favor negras

    [SerializeField]
    private GameObject[] tablero;
    [SerializeField]
    private GameObject row;
    [SerializeField]
    private GameObject row2;


    [SerializeField]
    private GameObject pantalla_final;
    private Text pantalla_final_text;
    [SerializeField]
    private Dropdown tipe;

    //cambiar turno
    private bool turno; //true blancas, false negras
    [SerializeField]
    private GameObject button;
    private Text button_text;
    private Image button_image;

    [SerializeField]
    private Button enJaqueButton;
    private Text enJaqueText;
    private Image enJaqueButtonImage;

    //move piece

    private Ray ray;
    private RaycastHit hit;
    private piece pieceToMove;

    [SerializeField]
    private GameObject resaltar;
    [SerializeField]
    private GameObject ataque;
    private List<GameObject> casillas_alcanzables;
    private List<GameObject> casillas_ataque;

    //pieczas
    [SerializeField]
    private GameObject[] pieces; // blanco,negro -> peon,torre,caballo,alfil,rey,reina
    //lista de piezas activa
    private List<piece> ma_pisis = new List<piece>();


    //logic 
    Layoutslogic logic;
    Layoutslogic dummy_logic;

    private const int RHEA_ITERATIONS = 2;
    private const float TIME_BUDGET = 0.5f; //s
    private const int GENETIC_GENS = 5; //Num de individuos
    private const int GENS_TO_MUTATE = 1; //its gonna be 1 in every case but who cares, there it is


    void Start()
    {
        logic = new Layoutslogic();
        dummy_logic = new Layoutslogic();

        logic.inizialice();
        dummy_logic.inizialice();

        //inicializa la pantalla final
        pantalla_final_text = pantalla_final.GetComponentInChildren<Text>();

        //inicializar boton
        button_text = button.GetComponentInChildren<Text>();
        button_image = button.GetComponent<Image>();

        //inicializa enjaque button
        enJaqueText = enJaqueButton.GetComponentInChildren<Text>();
        enJaqueButtonImage = enJaqueButton.GetComponent<Image>();

        for(int i = 0; i<8; i++)
        {
            //inicializar tablero
            if (i % 2 == 0)
            {
                tablero[i] = Instantiate(row, tablero[i].transform.position, Quaternion.identity);
            }
            else
            {
                tablero[i] = Instantiate(row2, tablero[i].transform.position, Quaternion.identity);
            }
        }


    inicializar();
        
    }


    void Update()
    {
        if (Input.GetMouseButtonUp(0))//si clik izquierdo down
        {
            clean_tiles();
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100) && hit.transform.GetComponent<piece>())
            {
                if (turno) {
                    if (hit.transform.GetComponent<piece>().pieza_Color == static_stuff.pieza_color.Blanco)
                    {
                        //si turno blanco checamos los blancos
                        accesible_tiles(hit.transform.GetComponent<piece>());
                    }

                }//solo funciona si es pvp
                else if(tipe.options[tipe.value].text == "PvP")
                {
                    if (hit.transform.GetComponent<piece>().pieza_Color == static_stuff.pieza_color.Negro)
                    {
                        //si turno negro checamos las piezas negras 
                        accesible_tiles(hit.transform.GetComponent<piece>());
                    }
                }
            }
        }

        if (!turno)
        {

            //llama al algoridmo que sea
            if (tipe.options[tipe.value].text == "random bot")
            {
                randomBot();
            }
            else if (tipe.options[tipe.value].text == "OSLA")
            {
                OSLA();
            }
            else if (tipe.options[tipe.value].text == "HC")
            {
                HC();
            }
            else if (tipe.options[tipe.value].text == "RHEA")
            {
                RHEA();
            }
        }
    }

    private void clean_tiles()//quitamos las casillas alcanzables y de ataque
    {
        foreach (GameObject item in casillas_alcanzables)
        {
            Destroy(item);
        }
        foreach (GameObject item in casillas_ataque)
        {
            Destroy(item);
        }
        casillas_alcanzables.Clear();
        casillas_ataque.Clear();
    }

    private void RHEA()
    {
        float time = 0;
        List<(piece, (int, int))> firstOpt = Rrandom();
        while (time < TIME_BUDGET)
        {
            time += Time.deltaTime;

            int best = 0;
            int secondBest = 1;
            List<(piece, (int, int))>[] grupo = new List<(piece, (int, int))>[RHEA_ITERATIONS];
            float[] evaluation = new float[RHEA_ITERATIONS];
            for (int i = 0; i < RHEA_ITERATIONS; i++)
            {
                grupo[i] = Rrandom();
                evaluation[i] = 0;
                for (int b = 0; b < grupo[i].Count; b++)
                {
                    evaluation[i] += Revaluate(grupo[i][b]);
                }


                if (evaluation[i] > evaluation[best])
                {
                    best = i;
                }
                else if (evaluation[i] > secondBest)
                {
                    secondBest = i;
                }
            }

             firstOpt = individua_cross_over(grupo[best],grupo[secondBest]);//primer grupo de opciones
        }
        int final = 0;
        float value = Revaluate(firstOpt[0]);
        for (int i = 0; i < firstOpt.Count; i++)//escogemos el mejor valor del mejor grupo
        {
            float value2 = Revaluate(firstOpt[i]);

            if (value2 > value)
            {
                final = i;
                value = value2;
            }
        }
        pieceToMove = firstOpt[final].Item1;
        movePieceForReal(firstOpt[final].Item2.Item1, firstOpt[final].Item2.Item2);
        clean_tiles();
    }
    private void HC()
    {
        List<(piece,(int,int))> firstOpt = Rrandom();//primer grupo de opciones
        float value = 0;
        for (int i = 0; i<firstOpt.Count; i++)//valor total
        {
            value = Revaluate(firstOpt[i]);
        }
        float time = 0;
        while (time < TIME_BUDGET)
        {
            time += Time.deltaTime;
            for(int i = 0; i<GENETIC_GENS; i++)
            {
                List<(piece, (int, int))> nextOne = Rmutate(firstOpt);
                float value2 = 0;
                for (int e = 0; i < firstOpt.Count; i++)
                {
                    value2 = Revaluate(nextOne[e]);
                }

                if (value2 > value)
                {
                    firstOpt = nextOne;
                    value = value2;
                }

            }
        }
        int final = 0;
        value = Revaluate(firstOpt[0]);
        for (int i = 0; i< firstOpt.Count; i++)//escogemos el mejor valor del mejor grupo
        {
            float value2 = Revaluate(firstOpt[i]);

            if (value2 > value)
            {
                final = i;
                value = value2;
            }
        }
        pieceToMove = firstOpt[final].Item1;
        movePieceForReal(firstOpt[final].Item2.Item1, firstOpt[final].Item2.Item2);
        clean_tiles();
    }

    private void randomBot()
    {
        List<int> listaDeOpciones = new List<int>();
        for(int i = 0; i< ma_pisis.Count; i++) 
        { 
            if (ma_pisis[i].pieza_Color == static_stuff.pieza_color.Negro)
            {
                accesible_tiles(ma_pisis[i]);
                if (casillas_ataque.Count + casillas_alcanzables.Count > 0)
                {
                    listaDeOpciones.Add(i);
                }
            }
        }

        accesible_tiles(ma_pisis[listaDeOpciones[Random.Range(0, listaDeOpciones.Count)]]);

        int x = Random.Range(0, casillas_ataque.Count + casillas_alcanzables.Count-1);
        if (x < casillas_ataque.Count)
        {
            (int, int) s = casillas_ataque[x].GetComponent<engage_movement>().getPos();
            movePieceForReal(s.Item1, s.Item2);
        }
        else
        {
            (int, int) s = casillas_alcanzables[x - casillas_ataque.Count].GetComponent<engage_movement>().getPos();
            movePieceForReal(s.Item1, s.Item2);
        }

        clean_tiles();//para que no se este repitiendo en el update
    }

    private void OSLA()
    {
        piece best_pieza = null;
        (int, int) best_move = (-1, -1);
        float balance = -9999999;

        foreach (piece pieza in ma_pisis)//por cada pieza
        {
            if(pieza.pieza_Color == static_stuff.pieza_color.Negro)
            {
                accesible_tiles(pieza);
                foreach (GameObject s in casillas_alcanzables)//calculamos el mejor movimiento para ambas
                {
                    (int, int) valor = s.GetComponent<engage_movement>().getPos();
                    (int, int) auxPos = (pieza.posX,pieza.posY);

                    movePiece(valor.Item1, valor.Item2);
                    float example = calculate_balance(valor);
                    Debug.Log(example);
                    Debug.Log(pieza.value);
                    if (example > balance)
                    {
                        balance = example;
                        best_pieza = pieza;
                        best_move = valor;
                    }

                    movePiece(auxPos.Item1, auxPos.Item2);
                }
                foreach (GameObject s in casillas_ataque)
                {
                    (int, int) valor = s.GetComponent<engage_movement>().getPos();
                    (int, int) auxPos = (pieza.posX, pieza.posY);

                    movePiece(valor.Item1, valor.Item2);
                    float example = calculate_balance(s.GetComponent<engage_movement>().getPos());
                    if (example > balance)
                    {
                        balance = example;
                        best_pieza = pieza;
                        best_move = s.GetComponent<engage_movement>().getPos();
                    }

                    movePiece(auxPos.Item1, auxPos.Item2);
                }
            }
        }

        if (best_pieza != null)
        {
            pieceToMove = best_pieza;
        }
        else
        {
            endGame();
        }

        movePieceForReal(best_move.Item1, best_move.Item2);
        clean_tiles();//para que no se este repitiendo en el update
    }

    private float calculate_balance((int, int) pos)
    {
        float balance = 0;

        //los peones tendrán mas puntuación si avanzan
        //los caballostendrán mas puntuación en el centro 
        //los alfiles, torres, reina ganaran puntuación dependiendo la cantidad de movimientos
        //tus piezas atacadas dan balance negativo
        //atacar piezas da balance positivo
        //comer piezas da balance positivo

        foreach(piece e in ma_pisis) { //queremos que nuestras piezas esten, no queremos que sus piezas esten
            if(e.pieza_Color == static_stuff.pieza_color.Blanco)
            {
                balance -= e.value;
                if (e.movimientos_posibles(casillas_ocupadas) != null)
                {
                    balance -= 0.1f * e.movimientos_posibles(casillas_ocupadas).Count;
                }
                foreach ((int, int) p in e.casillas_de_ataque)//refuerzon negativo si atacamos sus piezas
                {
                    if (p.Item1 >= 0 && p.Item1 <= 7 && p.Item2 >= 0 && p.Item2 <= 7)
                    {
                        foreach (piece pieza in ma_pisis)
                        {
                            if (pieza.isthisYou(p.Item1, p.Item2))
                            {
                                balance -= pieza.value / 2;
                            }
                        }
                    }
                }
            }
            else
            {
                balance += e.value;
            }
            if (e.GetComponent<rey>())//retrasamos el rey
            {
                balance +=  e.posX * 0.1f;
            }
            else if (e.GetComponent<caballo>())//recompensamos a los caballos por estar en el centro
            {
                if(e.posX<2)balance -= 0.1f;
                if (e.posX >6) balance -= 0.1f;
                if (e.posY < 2) balance -= 0.1f;
                if (e.posY > 6) balance -= 0.1f;
            }
            else
            {
                if (e.movimientos_posibles(casillas_ocupadas)!= null)
                {
                    balance += 0.2f * e.movimientos_posibles(casillas_ocupadas).Count;
                }
            }

            
            if( enJaque == 2)
            {
                balance += 5;
            }
            else if(enJaque == 1)
            {
                balance -= 5;
            }
        }
        balance += 0.5f * atacandoA(pos.Item1, pos.Item2);
        return balance;
    }

    private int atacandoA(int X, int Y)
    {
        foreach(piece p in ma_pisis)
        {
            if (p.isthisYou(X, Y))
            {
                return p.value;
            }
        }
        return 0;
    }
    private void accesible_tiles(piece piece)
    {
        clean_tiles();//para que no se este repitiendo en el update

        //actualizamos el ataque
        for (int i = 0; i < ma_pisis.Count; i++)
        {
            ma_pisis[i].actualizar_ataque(casillas_ocupadas);

        }
        pieceToMove = piece;

        List<(int, int)> movimientos = pieceToMove.movimientos_posibles(casillas_ocupadas);
        if (movimientos != null && movimientos.Count > 0)
        {
            //añadilos las casillas resaltadas
            for (int i = 0; i < movimientos.Count; i++)
            {
                if (checkIfJaquePropio(movimientos[i]))
                {
                    //la pieza llamara al metodo movePiece si es clickada
                    casillas_alcanzables.Add(Instantiate(resaltar, tablero[movimientos[i].Item1].transform.GetChild(movimientos[i].Item2).transform.position, Quaternion.identity));
                    casillas_alcanzables[casillas_alcanzables.Count-1].GetComponent<engage_movement>().getLayout(this, movimientos[i].Item1, movimientos[i].Item2);
                }
            }
        }

        //ahora movimientos guarda las posiciones donde la pieza puede atacar
        movimientos = pieceToMove.casillas_de_ataque;
        if (movimientos.Count > 0)
        {   
            //generamos las casillas de ataque
            for (int i = 0; i < movimientos.Count; i++)
            {
                //nos aseguramos que las casillas caen dentro del tablero
                if (movimientos[i].Item1 >= 0 && movimientos[i].Item1 <= 7 && movimientos[i].Item2 >= 0 && movimientos[i].Item2 <= 7)
                {
                    if (pieceToMove.pieza_Color == static_stuff.pieza_color.Blanco)
                    {
                        if (casillas_ocupadas_negras[movimientos[i].Item1][movimientos[i].Item2])
                        {
                            if (checkIfJaquePropio(movimientos[i]))
                            {
                                casillas_ataque.Add(Instantiate(ataque, tablero[movimientos[i].Item1].transform.GetChild(movimientos[i].Item2).transform.position, Quaternion.identity));
                                casillas_ataque[casillas_ataque.Count - 1].GetComponent<engage_movement>().getLayout(this, movimientos[i].Item1, movimientos[i].Item2);
                            }

                        }
                    }
                    else if (casillas_ocupadas_blancas[movimientos[i].Item1][movimientos[i].Item2])
                    { 
                        if(checkIfJaquePropio(movimientos[i]))
                        {
                            casillas_ataque.Add(Instantiate(ataque, tablero[movimientos[i].Item1].transform.GetChild(movimientos[i].Item2).transform.position, Quaternion.identity));
                            casillas_ataque[casillas_ataque.Count-1].GetComponent<engage_movement>().getLayout(this, movimientos[i].Item1, movimientos[i].Item2);
                        }
                    }
                       
                }
            }
        }
    }

    private void movePiece(int posX, int posY)
    {           
        //actualizamos posicion
        if(pieceToMove.pieza_Color == static_stuff.pieza_color.Blanco)
        {
            casillas_ocupadas_blancas[pieceToMove.posX][pieceToMove.posY] = false;//actualizamos tanto piezas ocupadas como piezas ocupadas de su color
            casillas_ocupadas[pieceToMove.posX][pieceToMove.posY] = false;
            casillas_ocupadas_negras[pieceToMove.posX][pieceToMove.posY] = false;
            pieceToMove.posX = posX;
            pieceToMove.posY = posY;//movemos
            casillas_ocupadas[pieceToMove.posX][pieceToMove.posY] = true;
            casillas_ocupadas_blancas[pieceToMove.posX][pieceToMove.posY] = true;
            casillas_ocupadas_negras[pieceToMove.posX][pieceToMove.posY] = false;
        }
        else
        {
            casillas_ocupadas_blancas[pieceToMove.posX][pieceToMove.posY] = false;
            casillas_ocupadas_negras[pieceToMove.posX][pieceToMove.posY] = false;
            casillas_ocupadas[pieceToMove.posX][pieceToMove.posY] = false;
            pieceToMove.posX = posX;
            pieceToMove.posY = posY;
            casillas_ocupadas[pieceToMove.posX][pieceToMove.posY] = true;
            casillas_ocupadas_negras[pieceToMove.posX][pieceToMove.posY] = true;
            casillas_ocupadas_blancas[pieceToMove.posX][pieceToMove.posY] = false;
        }

        checkIfJaque();
    }


    //################################
    //en esta función se cambia turno
    //#################################
    public void movePieceForReal(int posX, int posY)
    {
        //si pisamos una pieza se muere
        for (int i = 0; i < ma_pisis.Count; i++)
        {
            if (ma_pisis[i].isthisYou(posX, posY))
            {
                piece pieze = ma_pisis[i];
                ma_pisis.RemoveAt(i);
                Destroy(pieze.gameObject);

            }
        }

        movePiece(posX, posY);

        if (pieceToMove.GetComponentInChildren<Peon_blanco>() && pieceToMove.posX >= 7 ||
            pieceToMove.GetComponentInChildren<Peon_negro>() && pieceToMove.posX <= 0) //si un peon llega al final
        {
            for (int i = 0; i < ma_pisis.Count; i++)
            {
                if (ma_pisis[i].isthisYou(posX, posY))
                {
                    piece pieze = ma_pisis[i];
                    ma_pisis.RemoveAt(i);
                    Destroy(pieze.gameObject);

                }
            }
            changePawn(posX, posY);
        }

        pieceToMove.transform.position = tablero[posX].transform.GetChild(posY).transform.position;//movemos visualmente

        cambiarTurno();

        if (haveILost())
        {
            endGame();
        }

        checkIfJaque();
    }

    private void changePawn(int posX, int posY)
    {
        if(pieceToMove.GetComponentInChildren<Peon_blanco>())
        {
            setUpPiece(pieces[5], posX, posY, true);
        }
        else
        {
            setUpPiece(pieces[11], posX, posY, false);
        }
    }
    private void changePawnTry()
    {
        if (pieceToMove.GetComponentInChildren<Peon_blanco>() && pieceToMove.posX >= 7)
        {
            pieceToMove.GetComponentInChildren<Peon_blanco>().seriousAttack(casillas_ocupadas);
        }
        else if (pieceToMove.GetComponentInChildren<Peon_negro>() && pieceToMove.posX <= 0)
        {
            pieceToMove.GetComponentInChildren<Peon_negro>().seriousAttack(casillas_ocupadas);
        }
    }

    private bool checkIfJaquePropio((int,int) moves)//checa si nuestro movimiento nos va a poner en jque
    {
        (int, int) aux = (pieceToMove.posX, pieceToMove.posY);

        bool a = casillas_ocupadas[moves.Item1][moves.Item2];
        bool ante = casillas_ocupadas_blancas[moves.Item1][moves.Item2];
        bool bajo = casillas_ocupadas_negras[moves.Item1][moves.Item2];

        movePiece(moves.Item1,moves.Item2);
        bool valid = true;
        if((pieceToMove.pieza_Color == static_stuff.pieza_color.Blanco && enJaque == 2) || (pieceToMove.pieza_Color == static_stuff.pieza_color.Negro && enJaque == 1))
        {
            valid = false;
        }
        movePiece(aux.Item1, aux.Item2);

        casillas_ocupadas[moves.Item1][moves.Item2] = a;
        casillas_ocupadas_blancas[moves.Item1][moves.Item2] = ante;
        casillas_ocupadas_negras[moves.Item1][moves.Item2] = bajo;

        checkIfJaque();

        return valid;
    }

    private void checkIfJaque()//checa si hemos hecho jaque
    {
        foreach (piece pieza in ma_pisis)
        {
            pieza.actualizar_ataque(casillas_ocupadas);
        }
        enJaque = 0;
        enJaqueButton.gameObject.SetActive(false);
        foreach (piece piece in ma_pisis)//todas las piezas checan todas las piezas
        {
            List<(int, int)> movimientos = piece.casillas_de_ataque;

            foreach (piece piece2 in ma_pisis)
            {
                if (piece2.king && piece2.pieza_Color != piece.pieza_Color) //si es el king enemigo checamos si le damos
                { 
                    foreach((int,int) move in movimientos)
                    {
                        if(piece2.isthisYou(move.Item1, move.Item2))
                        {
                            enJaqueButton.gameObject.SetActive(true);
                            if (piece.pieza_Color == static_stuff.pieza_color.Blanco)
                            {
                                enJaque = 1;
                                enJaqueText.color = Color.white;
                                enJaqueButtonImage.color = Color.black;
                            }
                            else
                            {
                                enJaque = 2;
                                enJaqueText.color = Color.black;
                                enJaqueButtonImage.color = Color.white;
                            }
                        }
                    }
                }
            }

        }
    }

    private bool haveILost()
    {
        int posibleMoves = 0;
        foreach (piece s in ma_pisis)
        {
            if ((s.pieza_Color == static_stuff.pieza_color.Blanco && turno) || (s.pieza_Color == static_stuff.pieza_color.Negro && !turno))
            {
                accesible_tiles(s);
                posibleMoves += casillas_alcanzables.Count + casillas_ataque.Count;

            }
        }
        return posibleMoves <= 0;

    }

    private void endGame()
    {
        if(enJaque == 0)
        {
            pantalla_final_text.text = "¡Empate!";
        }
        else if(enJaque == 1)
        {
            pantalla_final_text.text = "Gana el Blanco";
        }
        else
        {
            pantalla_final_text.text = "Gana el Negro";
        }
        pantalla_final.SetActive(true);
    }


    private void cambiarTurno()
    {
        clean_tiles();

        checkIfJaque();
        //solo sirve para cambiar de turno
        if (turno)
        {
            turno = false;
            button_image.color = Color.black;
            button_text.color = Color.white;
            button_text.text = "Negro";
        }
        else
        {
            turno = true;
            button_text.text = "Blanco";
            button_image.color = Color.white;
            button_text.color = Color.black;
        }
    }
    private void cambiarTurno(bool turno)
    {
        clean_tiles();
        //solo sirve para cambiar de turno
        if (!turno)
        {
            this.turno = false;
            button_image.color = Color.black;
            button_text.color = Color.white;
            button_text.text = "Negro";
        }
        else
        {
            this.turno = true;
            button_text.text = "Blanco";
            button_image.color = Color.white;
            button_text.color = Color.black;
        }
        checkIfJaque();
    }

    public void inicializar()
    {
        casillas_alcanzables = new List<GameObject>();
        casillas_ataque = new List<GameObject>();

        casillas_ocupadas = new bool[8][];
        casillas_ocupadas_blancas = new bool[8][];
        casillas_ocupadas_negras = new bool[8][];
        enJaque = 0;
        pantalla_final.SetActive(false);
        enJaqueButton.gameObject.SetActive(false);

        foreach(piece d in ma_pisis)
        {
            Destroy(d.gameObject);
        }

        for (int i = 0; i < tablero.Length; i++)
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

        ma_pisis.Clear();
        cambiarTurno(true);
        //inicializar piecezas
        setUpPieces();

    }
    private void setUpPieces()
    {
        for (int i = 0; i < 8; i++)
        {
            setUpPiece(pieces[0], 1, i, true);//peones blancos
            setUpPiece(pieces[6], 6, i, false);//peones negros
        }
            setUpmirrorPiece(pieces[1], 0, 0, true);//torres blancas
            setUpmirrorPiece(pieces[7], 7, 0, false);//torres negras
            setUpmirrorPiece(pieces[2], 0, 1, true);//caballos blancos
            setUpmirrorPiece(pieces[8], 7, 1, false);//caballos negros
            setUpmirrorPiece(pieces[3], 0, 2, true);//alfiles blancos
            setUpmirrorPiece(pieces[9], 7, 2, false);//alfiles negros
            setUpPiece(pieces[4], 0, 4, true);//rey blanco
            setUpPiece(pieces[10], 7, 4, false);//rey negro negra
            setUpPiece(pieces[5], 0, 3, true);//reina blanca
            setUpPiece(pieces[11], 7, 3, false);//reina negra
    }

    private void setUpPiece(GameObject piece, int posX, int posY, bool color)
    {
        //creamos e inicializamos la pieza
        piece pieza = Instantiate(piece, tablero[posX].transform.GetChild(posY).transform.position, Quaternion.identity).GetComponent<piece>();
        pieza.posX = posX;
        pieza.posY = posY;
        ma_pisis.Add(pieza);

        casillas_ocupadas[posX][posY] = true;

        if (color)
        {
            pieza.pieza_Color = static_stuff.pieza_color.Blanco;
            casillas_ocupadas_blancas[posX][posY] = true;
        }
        else
        {
            pieza.pieza_Color = static_stuff.pieza_color.Negro;
            casillas_ocupadas_negras[posX][posY] = true;
        }

    }
    private void setUpmirrorPiece(GameObject piece, int posX, int posY, bool color)
    {
        piece pieza = Instantiate(piece, tablero[posX].transform.GetChild(posY).transform.position, Quaternion.identity).GetComponent<piece>();
        pieza.posX = posX;
        pieza.posY = posY;
        piece pieza1 = Instantiate(piece, tablero[posX].transform.GetChild(7-posY).transform.position, Quaternion.identity).GetComponent<piece>();
        pieza1.posX = posX;
        pieza1.posY = 7-posY;

        ma_pisis.Add(pieza);
        ma_pisis.Add(pieza1);

        casillas_ocupadas[posX][posY] = true;
        casillas_ocupadas[posX][7-posY] = true;

        if (color)
        {
            pieza.pieza_Color = static_stuff.pieza_color.Blanco;
            pieza1.pieza_Color = static_stuff.pieza_color.Blanco;
            casillas_ocupadas_blancas[posX][posY] = true;
            casillas_ocupadas_blancas[posX][7 - posY] = true;
        }
        else
        {
            pieza.pieza_Color = static_stuff.pieza_color.Negro;
            pieza1.pieza_Color = static_stuff.pieza_color.Negro;
            casillas_ocupadas_negras[posX][posY] = true;
            casillas_ocupadas_negras[posX][7-posY] = true;
        }
    }


    //####################################
    //  RHEA methods
    //####################################

    private List<(piece,(int,int))> Rmutate(List<(piece, (int, int))> huesped)
    {
        for (int i = 0; i<GENS_TO_MUTATE; i++)
        {
            if (ma_pisis[i].pieza_Color == static_stuff.pieza_color.Negro)
            {
                accesible_tiles(ma_pisis[i]);
                if (casillas_ataque.Count + casillas_alcanzables.Count > 0)
                {
                    int x = Random.Range(0, casillas_ataque.Count + casillas_alcanzables.Count - 1);
                    if (x < casillas_ataque.Count)
                    {
                        (int, int) s = casillas_ataque[x].GetComponent<engage_movement>().getPos();
                        huesped[Random.Range(0, huesped.Count)] = (pieceToMove, s);
                    }
                    else
                    {
                        (int, int) s = casillas_alcanzables[x - casillas_ataque.Count].GetComponent<engage_movement>().getPos();
                        huesped[Random.Range(0, huesped.Count)] = (pieceToMove, s);
                    }
                }
            }
        }

        return huesped;
    }
    private List<(piece, (int, int))> Rrandom()
    {
        //tomamos las opciones posibles      
        List<int> listaDeOpciones = new List<int>();
        for (int i = 0; i < ma_pisis.Count; i++)
        {
            if (ma_pisis[i].pieza_Color == static_stuff.pieza_color.Negro)
            {
                accesible_tiles(ma_pisis[i]);
                if (casillas_ataque.Count + casillas_alcanzables.Count > 0)
                {
                    listaDeOpciones.Add(i);
                }
            }
        }

        List<(piece, (int, int))> population = new List<(piece, (int, int))>();
        for (int i = 0; i < GENETIC_GENS; i++)
        {
            accesible_tiles(ma_pisis[listaDeOpciones[Random.Range(0, listaDeOpciones.Count)]]);

            int x = Random.Range(0, casillas_ataque.Count + casillas_alcanzables.Count - 1);
            if (x < casillas_ataque.Count)
            {
                (int, int) s = casillas_ataque[x].GetComponent<engage_movement>().getPos();

                population.Add((pieceToMove, s));//extraemos una accion al azar
            }
            else
            {
                (int, int) s = casillas_alcanzables[x - casillas_ataque.Count].GetComponent<engage_movement>().getPos();
                population.Add((pieceToMove, s));//extraemos una accion al azar
            }
        }

        return population;//extraemos una accion al azar
    }

    private float Revaluate((piece, (int,int))individual)
    {
        accesible_tiles(individual.Item1);
        return calculate_balance(individual.Item2);
    }

    private List<(piece,(int,int))> individua_cross_over(List<(piece, (int, int))>a , List<(piece, (int, int))> b)
    {
        List<(piece, (int, int))> nuevo = new List<(piece, (int, int))>();
        bool turno = false; 
        for (int i = 0; i< a.Count; i++)
        {
            if (turno)
            {
                nuevo.Add(a[i]);
                turno = false;
            }
            else
            {
                nuevo.Add(b[i]);
                turno = true;
            }
        }
        return nuevo;
    }
}
