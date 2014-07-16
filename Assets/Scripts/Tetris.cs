using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviour
{
    //Board
    public int[,] board;

    //Block
    public Transform block;

    //Spawn boolean
    public bool spawn;

    // Constant MovementTime for movement keys
    private const float movementTime = 0.15f;

    // Seconds before block will move left is held
    private float leftMovementTime = movementTime;

    // Seconds before block will move right is held
    private float rightMovementTime = movementTime;

    //Seconds before next block spawn
    public float nextBlockSpawnTime = 0.5f;

    //Block fall speed
    public float blockFallSpeed = 0.5f;

    //Game over level
    public int gameOverHeight = 22; //20 board + 2 edge

    //Current spawned shapes
    private List<Transform> shapes = new List<Transform>();

    private List<Transform> nextShapes = new List<Transform>();

    private List<Transform> shadowShapes = new List<Transform>();

    //Set true if game over
    private bool gameOver;

    //Set forceDown mode
    private bool forceDown;

    private bool nextPiece;

    //Current rotation of an object
    private int currentRot = 0;

    //Current pivot of the shape
    private GameObject pivot;

    //Array of all the color materials
    public Material[] materials;

    public int score;

    private Piece piece;

    private void Start ()
    {
        //Default board is 10x16

        //1+10+1 - Side edge

        //+2 - Space for spawning
        //+1 - Top edge
        //20 - Height
        //+1 - Down edge

        board = new int[12, 24];//Set board width and height
        piece = Piece.None;
        GenerateBoard();//Generate board
        SpawnShape();
        InvokeRepeating("MoveDown", blockFallSpeed, blockFallSpeed); //move block down
    }

    private void Update ()
    {
        if (spawn && shapes.Count == 4)
        { //If there is block
            //Get spawned blocks positions
            Vector3 a = shapes[0].transform.position;
            Vector3 b = shapes[1].transform.position;
            Vector3 d = shapes[2].transform.position;
            Vector3 c = shapes[3].transform.position;

            if (!forceDown)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {//Move left
                    if (CheckUserMove(a, b, c, d, true))
                    {//Check if we can move it left
                        a.x -= 1;
                        b.x -= 1;
                        c.x -= 1;
                        d.x -= 1;

                        pivot.transform.position = new Vector3(pivot.transform.position.x - 1, pivot.transform.position.y, pivot.transform.position.z);

                        shapes[0].transform.position = a;
                        shapes[1].transform.position = b;
                        shapes[2].transform.position = c;
                        shapes[3].transform.position = d;
                    }
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {//Move left
                    leftMovementTime -= Time.deltaTime;

                    if (leftMovementTime <= 0)
                    {
                        if (CheckUserMove(a, b, c, d, true))
                        {//Check if we can move it left
                            a.x -= 1;
                            b.x -= 1;
                            c.x -= 1;
                            d.x -= 1;

                            pivot.transform.position = new Vector3(pivot.transform.position.x - 1, pivot.transform.position.y, pivot.transform.position.z);

                            shapes[0].transform.position = a;
                            shapes[1].transform.position = b;
                            shapes[2].transform.position = c;
                            shapes[3].transform.position = d;
                        }
                        leftMovementTime = movementTime;
                    }
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {//Move right
                    if (CheckUserMove(a, b, c, d, false))
                    {//Check if we can move it right
                        a.x += 1;
                        b.x += 1;
                        c.x += 1;
                        d.x += 1;
                        pivot.transform.position = new Vector3(pivot.transform.position.x + 1, pivot.transform.position.y, pivot.transform.position.z);

                        shapes[0].transform.position = a;
                        shapes[1].transform.position = b;
                        shapes[2].transform.position = c;
                        shapes[3].transform.position = d;
                    }
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {//Move right
                    rightMovementTime -= Time.deltaTime;
                    if (rightMovementTime <= 0)
                    {
                        if (CheckUserMove(a, b, c, d, false))
                        {//Check if we can move it right
                            a.x += 1;
                            b.x += 1;
                            c.x += 1;
                            d.x += 1;
                            pivot.transform.position = new Vector3(pivot.transform.position.x + 1, pivot.transform.position.y, pivot.transform.position.z);

                            shapes[0].transform.position = a;
                            shapes[1].transform.position = b;
                            shapes[2].transform.position = c;
                            shapes[3].transform.position = d;
                        }
                        rightMovementTime = movementTime;
                    }
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    //Move down fast
                    MoveDown();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //Rotate
                    Rotate(shapes[0].transform, shapes[1].transform, shapes[2].transform, shapes[3].transform);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    forceDown = true;
                    MoveDown();
                }
            }
            else
            {
                MoveDown();
            }
        }

        if (!spawn && !gameOver)
        {//If nothing spawned, if game over = false, then spawn
            //StartCoroutine("Wait");
            //spawn = true;
            //Reset rotation
            //currentRot = 0;
        }
    }

    private void MoveDown ()
    {
        //Spawned blocks positions
        if (shapes.Count != 4)
        {
            return;
        }
        Vector3 a = shapes[0].transform.position;
        Vector3 b = shapes[1].transform.position;
        Vector3 c = shapes[2].transform.position;
        Vector3 d = shapes[3].transform.position;

        if (CheckMove(a, b, c, d) == true)
        { // Will we hit anything if we move block down(true = we can move)
            //Move block down by 1
            a = new Vector3(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y - 1.0f), a.z);
            b = new Vector3(Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y - 1.0f), b.z);
            c = new Vector3(Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y - 1.0f), c.z);
            d = new Vector3(Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y - 1.0f), d.z);

            pivot.transform.position = new Vector3(pivot.transform.position.x, pivot.transform.position.y - 1, pivot.transform.position.z);

            shapes[0].transform.position = a;
            shapes[1].transform.position = b;
            shapes[2].transform.position = c;
            shapes[3].transform.position = d;
        }
        else
        {
            //We hit something. Stop and mark current shape location as filled in board, also destroy last pivot game object

            Destroy(pivot.gameObject); //Destroy pivot

            //Set ID in board
            board[Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y)] = 1;
            board[Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y)] = 1;
            board[Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y)] = 1;
            board[Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y)] = 1;

            //****************************************************
            CheckRow(1); //Check for any match
            CheckRow(gameOverHeight); //Check for game over
            //****************************************************

            if (gameOver)
            {
                return;
            }
            shapes.Clear(); //Clear spawned blocks from array
            spawn = false; //Spawn a new block
            forceDown = false;

            SpawnShape();
        }
    }

    //Wait time before next block spawn
    private IEnumerator Wait ()
    {
        yield return new WaitForSeconds(nextBlockSpawnTime);
        SpawnShape();
        //SpawnShadowShape();
    }

    private bool CheckMove (Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        //Check, if we move a block down will it hit something
        if (board[Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y - 1)] == 1)
        {
            Score.IncreaseScoreBlock();
            UpdateScore();
            return false;
        }
        if (board[Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y - 1)] == 1)
        {
            return false;
        }
        if (board[Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y - 1)] == 1)
        {
            return false;
        }
        if (board[Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y - 1)] == 1)
        {
            return false;
        }

        return true;
    }

    private bool CheckUserMove (Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool dir)
    {
        //Check, if we move a block left/right will it hit something
        if (dir)
        {//Left
            if (board[Mathf.RoundToInt(a.x - 1), Mathf.RoundToInt(a.y)] == 1 || board[Mathf.RoundToInt(b.x - 1), Mathf.RoundToInt(b.y)] == 1 || board[Mathf.RoundToInt(c.x - 1), Mathf.RoundToInt(c.y)] == 1 || board[Mathf.RoundToInt(d.x - 1), Mathf.RoundToInt(d.y)] == 1)
            {
                return false;
            }
        }
        else
        {//Right
            if (board[Mathf.RoundToInt(a.x + 1), Mathf.RoundToInt(a.y)] == 1 || board[Mathf.RoundToInt(b.x + 1), Mathf.RoundToInt(b.y)] == 1 || board[Mathf.RoundToInt(c.x + 1), Mathf.RoundToInt(c.y)] == 1 || board[Mathf.RoundToInt(d.x + 1), Mathf.RoundToInt(d.y)] == 1)
            {
                return false;
            }
        }
        return true;
    }

    private void GenerateBoard ()
    {
        GameObject boardObject = GameObject.Find("Board");
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                if (x < 11 && x > 0)
                {
                    if (y > 0 && y < board.GetLength(1) - 2)
                    {
                        //Board
                        board[x, y] = 0;
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = new Vector3(x, y, 1);
                        Material material = new Material(Shader.Find("Diffuse"));
                        material.color = Color.grey;
                        cube.renderer.material = material;
                        cube.transform.parent = boardObject.transform;
                    }
                    else if (y < board.GetLength(1) - 2)
                    {
                        board[x, y] = 1;
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = new Vector3(x, y, 0);
                        Material material = new Material(Shader.Find("Diffuse"));
                        material.color = Color.black;
                        cube.renderer.material = material;
                        cube.transform.parent = boardObject.transform;
                        cube.collider.isTrigger = true;
                    }
                }
                else if ((y < board.GetLength(1) - 2))
                {
                    //Left and right edge
                    board[x, y] = 1;
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x, y, 0);
                    Material material = new Material(Shader.Find("Diffuse"));
                    material.color = Color.black;
                    cube.renderer.material = material;
                    cube.transform.parent = boardObject.transform;
                }
            }
        }
    }

    private void SpawnShape ()
    {
        int height = board.GetLength(1) - 4;
        int xPos = board.GetLength(0) / 2 - 1;

        for (int i = 0; i < 2; i++)
        {
            int shape = Random.Range(0, 7);//Random shape
            if (i == 0)
            {
                if (nextShapes.Count != 0)
                {
                    pivot = new GameObject("RotateAround"); //Pivot of the shape

                    nextPiece = false;
                    switch (piece)
                    {
                        case Piece.Z:
                            pivot.transform.position = new Vector3(xPos, height + 1, 0);
                            shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos - 1, height + 1, 0), piece));
                            break;

                        case Piece.J:
                            pivot.transform.position = new Vector3(xPos, height + 1, 0);
                            shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));
                            break;

                        case Piece.I:
                            pivot.transform.position = new Vector3(xPos + 0.5f, height + 1.5f, 0);

                            shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 3, 0), piece));
                            break;

                        case Piece.L:
                            pivot.transform.position = new Vector3(xPos, height + 1, 0);

                            shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));
                            break;

                        case Piece.S:
                            pivot.transform.position = new Vector3(xPos, height + 1, 0);

                            shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));
                            break;

                        case Piece.O:
                            pivot.transform.position = new Vector3(xPos + 0.5f, height + 0.5f, 0);

                            shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));
                            break;

                        case Piece.T:
                            pivot.transform.position = new Vector3(xPos, height, 0);

                            shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
                            shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));

                            break;
                    }
                }
                else
                {
                    //Create a new pivot
                    pivot = new GameObject("RotateAround"); //Pivot of the shape
                    nextPiece = false;
                    //SShape
                    if (shape == 0)
                    {
                        piece = Piece.S;
                        pivot.transform.position = new Vector3(xPos, height + 1, 0);

                        shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));

                        Debug.Log("Spawned SShape");
                    }
                    //IShape
                    else if (shape == 1)
                    {
                        piece = Piece.I;
                        pivot.transform.position = new Vector3(xPos + 0.5f, height + 1.5f, 0);

                        shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos, height + 3, 0), piece));

                        Debug.Log("Spawned IShape");
                    }
                    //OShape
                    else if (shape == 2)
                    {
                        piece = Piece.O;
                        pivot.transform.position = new Vector3(xPos + 0.5f, height + 0.5f, 0);

                        shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));

                        Debug.Log("Spawned OShape");
                    }
                    //LShape
                    else if (shape == 3)
                    {
                        shapes = GenerateLPiece(xPos, height);
                    }

                    //TShape
                    else if (shape == 4)
                    {
                        piece = Piece.T;
                        pivot.transform.position = new Vector3(xPos, height, 0);

                        shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));

                        Debug.Log("Spawned TShape");
                    }

                    //JShape
                    else if (shape == 5)
                    {
                        shapes = GenerateJPiece(xPos, height);
                    }

                    //ZShape
                    else
                    {
                        piece = Piece.Z;
                        pivot.transform.position = new Vector3(xPos, height + 1, 0);

                        shapes.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
                        shapes.Add(GenerateBlock(new Vector3(xPos - 1, height + 1, 0), piece));

                        Debug.Log("Spawned ZShape");
                    }
                }

                spawn = true;
                currentRot = 0;
            }
            else if (i == 1)
            {
                foreach (Transform child in GameObject.Find("NextPiece").transform)
                {
                    Destroy(child.gameObject);
                }
                nextShapes.Clear();

                int height2 = (int)GameObject.Find("NextPiece").transform.position.y;
                int xPos2 = (int)GameObject.Find("NextPiece").transform.position.x;

                nextPiece = true;

                //SShape
                if (shape == 0)
                {
                    piece = Piece.S;

                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 - 1, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 1, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 + 1, height2 + 1, 0), piece));

                    Debug.Log("Spawned SShape");
                }
                //IShape
                else if (shape == 1)
                {
                    piece = Piece.I;

                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 1, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 3, 0), piece));

                    Debug.Log("Spawned IShape");
                }
                //OShape
                else if (shape == 2)
                {
                    piece = Piece.O;

                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 + 1, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 1, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 + 1, height2 + 1, 0), piece));

                    Debug.Log("Spawned OShape");
                }
                //LShape
                else if (shape == 3)
                {
                    piece = Piece.L;

                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 + 1, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 1, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 2, 0), piece));

                    Debug.Log("Spawned JShape");
                }

                //TShape
                else if (shape == 4)
                {
                    piece = Piece.T;

                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 - 1, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 + 1, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 1, 0), piece));

                    Debug.Log("Spawned TShape");
                }

                //JShape
                else if (shape == 5)
                {
                    piece = Piece.J;

                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 - 1, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 1, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 2, 0), piece));

                    Debug.Log("Spawned LShape");
                }

                //ZShape
                else
                {
                    piece = Piece.Z;

                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 + 1, height2, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2, height2 + 1, 0), piece));
                    nextShapes.Add(GenerateBlock(new Vector3(xPos2 - 1, height2 + 1, 0), piece));

                    Debug.Log("Spawned ZShape");
                }

                currentRot = 0;
            }
        }
    }

    //Create a block at the position
    private Transform GenerateBlock (Vector3 pos, Piece piece)
    {
        Transform obj = null;
        switch (piece)
        {
            case Piece.Z:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                obj.renderer.material = materials[0];
                break;

            case Piece.J:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                obj.renderer.material = materials[1];
                break;

            case Piece.I:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                obj.renderer.material = materials[2];
                break;

            case Piece.L:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                obj.renderer.material = materials[3];
                break;

            case Piece.S:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                obj.renderer.material = materials[4];
                break;

            case Piece.O:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                obj.renderer.material = materials[5];
                break;

            case Piece.T:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                obj.renderer.material = materials[6];
                break;
        }

        if (nextPiece)
        {
            obj.parent = GameObject.Find("NextPiece").transform;
        }
        return obj;
    }

    //Check specific row for match
    private void CheckRow (int y)
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block"); //All blocks in the scene
        int count = 0; //Blocks found in a row

        for (int x = 1; x < board.GetLength(0) - 1; x++)
        {//Go through each block on this height
            if (board[x, y] == 1)
            {//If there is any block at this position
                count++;//We found +1 block
            }
        }

        if (y == gameOverHeight && count > 0)
        {//If the current height is game over height, and there is more than 0 block, then game over
            Debug.LogWarning("Game over");
            gameOver = true;
        }

        if (count == 10)
        {//The row is full
            //Start from bottom of the board(withouth edge and block spawn space)
            for (int cy = y; cy < board.GetLength(1) - 3; cy++)
            {
                for (int cx = 1; cx < board.GetLength(0) - 1; cx++)
                {
                    foreach (GameObject go in blocks)
                    {
                        int height = Mathf.RoundToInt(go.transform.position.y);
                        int xPos = Mathf.RoundToInt(go.transform.position.x);

                        if (xPos == cx && height == cy)
                        {
                            if (height == y)
                            {//The row we need to destroy
                                board[xPos, height] = 0;//Set empty space
                                Destroy(go.gameObject);
                            }
                            else if (height > y)
                            {
                                board[xPos, height] = 0;//Set old position to empty
                                board[xPos, height - 1] = 1;//Set new position
                                go.transform.position = new Vector3(xPos, height - 1, go.transform.position.z);//Move block down
                            }
                        }
                    }
                }
            }
            Score.IncreaseScoreLine();

            CheckRow(y); //We moved blocks down, check again this row
        }
        else if (y + 1 < board.GetLength(1) - 3)
        {
            CheckRow(y + 1); //Check row above this
        }
        UpdateScore();
    }

    private void Rotate (Transform a, Transform b, Transform c, Transform d)
    {
        //Set parent to pivot so we can rotate
        a.parent = pivot.transform;
        b.parent = pivot.transform;
        c.parent = pivot.transform;
        d.parent = pivot.transform;

        currentRot -= 90;//Add rotation
        if (currentRot == 360)
        { //Reset rotation
            currentRot = 0;
        }

        pivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

        a.parent = null;
        b.parent = null;
        c.parent = null;
        d.parent = null;

        if (CheckRotate(a.position, b.position, c.position, d.position) == false)
        {
            //Set parent to pivot so we can rotate
            a.parent = pivot.transform;
            b.parent = pivot.transform;
            c.parent = pivot.transform;
            d.parent = pivot.transform;

            currentRot += 90;
            pivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

            a.parent = null;
            b.parent = null;
            c.parent = null;
            d.parent = null;
        }
    }

    private bool CheckRotate (Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        if (Mathf.RoundToInt(a.x) < board.GetLength(0) - 1)
        {//Check if block is in board
            if (board[Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y)] == 1)
            {
                //If rotated block hit any other block or edge, after rotation
                print("Can't Rotate");
                return false; //Rotate in default position - previous
            }
        }
        else
        {//If the block is not in the board
            print("Can't Rotate");
            return false;//Do not rotate
        }
        if (Mathf.RoundToInt(b.x) < board.GetLength(0) - 1)
        {
            if (board[Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y)] == 1)
            {
                print("Can't Rotate");
                return false;
            }
        }
        else
        {
            print("Can't Rotate");
            return false;
        }
        if (Mathf.RoundToInt(c.x) < board.GetLength(0) - 1)
        {
            if (board[Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y)] == 1)
            {
                print("Can't Rotate");
                return false;
            }
        }
        else
        {
            print("Can't Rotate");
            return false;
        }
        if (Mathf.RoundToInt(d.x) < board.GetLength(0) - 1)
        {
            if (board[Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y)] == 1)
            {
                print("Can't Rotate");
                return false;
            }
        }
        else
        {
            print("Can't Rotate");
            return false;
        }
        print("Can Rotate");
        return true; //We can rotate
    }

    private void UpdateScore ()
    {
        score = Score.GetScore();
    }

    private void SpawnShadowShape ()
    {
        shadowShapes = shapes;
        Transform obj = null;
        Material shadowMat = null;
        piece = Piece.None;

        foreach (Transform shape in shadowShapes)
        {
            obj = (Transform)Instantiate(shape) as Transform;
            shadowMat = obj.renderer.material;
            shadowMat.color = new Color(shadowMat.color.r, shadowMat.color.g, shadowMat.color.b, .35f);
            obj.renderer.material = shadowMat;
        }
    }

    private void MoveDownShadowShape ()
    {
    }

    private List<Transform> GenerateLPiece (int xPos, int height)
    {
        piece = Piece.L;

        List<Transform> list = new List<Transform>();

        pivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));

        Debug.Log("Spawned L Shape");

        return list;
    }

    private List<Transform> GenerateJPiece (int xPos, int height)
    {
        piece = Piece.J;

        List<Transform> list = new List<Transform>();

        pivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));

        Debug.Log("Spawned J Shape");
        return list;
    }
}

internal enum Piece
{
    Z,
    J,
    I,
    L,
    S,
    O,
    T,
    None
}

/*
* TODO: Make Methods for each piece
* TODO: WORK ON SHADOW SHAPE
*/