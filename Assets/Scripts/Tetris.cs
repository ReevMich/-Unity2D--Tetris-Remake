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
    public float leftMovementTime = movementTime;

    // Seconds before block will move right is held
    public float rightMovementTime = movementTime;

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

    private bool shadowPiece;

    public bool holding = false;

    public bool onGround = false;

    //Current rotation of an object
    private int currentRot = 0;

    //Current pivot of the shape
    private GameObject pivot;

    private GameObject shadowPivot;

    //Array of all the color materials
    public Material[] materials;

    public int score;

    private Piece piece;

    private int shadowShapeNumber;

    private int nextShapeNumber;

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
        SpawnShadowShape();
        InvokeRepeating("MoveDown", blockFallSpeed, blockFallSpeed); //move block down
        //InvokeRepeating("MoveDownShadowShape", .05f, .05f); //move block down
    }

    private void Update ()
    {
        if (spawn && shapes.Count == 4)
        { //If there is block
            //Get spawned blocks positions
            Vector3 a = shapes[0].transform.position;
            Vector3 b = shapes[1].transform.position;
            Vector3 c = shapes[2].transform.position;
            Vector3 d = shapes[3].transform.position;

            if (!forceDown)
            {
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    leftMovementTime = movementTime;
                    holding = false;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    if (!holding)
                    {
                        MoveLeft();
                    }

                if (Input.GetKey(KeyCode.LeftArrow))
                {//Move left
                    leftMovementTime -= Time.deltaTime;

                    if (leftMovementTime <= 0)
                    {
                        holding = true;
                        MoveLeft();
                        leftMovementTime = movementTime;
                    }
                }

                if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    rightMovementTime = movementTime;
                    holding = false;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {//Move right
                    MoveRight();
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {//Move right
                    rightMovementTime -= Time.deltaTime;
                    if (rightMovementTime <= 0)
                    {
                        MoveRight();
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
                    RotateShadow(shadowShapes[0].transform, shadowShapes[1].transform, shadowShapes[2].transform, shadowShapes[3].transform);
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
            if (!onGround)
            {
                MoveDownShadowShape();
            }
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
            Destroy(shadowPivot.gameObject); // Destroy shadow pivot

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
            shadowShapes.Clear();
            DestroyShadowPiece();
            spawn = false; //Spawn a new block
            forceDown = false;
            nextPiece = false;

            SpawnShape();
            SpawnShadowShape();
        }
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
                    //SShape
                    if (nextShapeNumber == 0)
                        shapes = GenerateSPiece(xPos, height);
                    //IShape
                    else if (nextShapeNumber == 1)
                        shapes = GenerateIPiece(xPos, height);
                    //OShape
                    else if (nextShapeNumber == 2)
                        shapes = GenerateOPiece(xPos, height);
                    //LShape
                    else if (nextShapeNumber == 3)
                        shapes = GenerateLPiece(xPos, height);
                    //TShape
                    else if (nextShapeNumber == 4)
                        shapes = GenerateTPiece(xPos, height);
                    //JShape
                    else if (nextShapeNumber == 5)
                        shapes = GenerateJPiece(xPos, height);
                    //ZShape
                    else
                        shapes = GenerateZPiece(xPos, height);

                    shadowShapeNumber = nextShapeNumber;
                }
                else
                {
                    //Create a new pivot
                    pivot = new GameObject("RotateAround"); //Pivot of the shape

                    //SShape
                    if (shape == 0)
                        shapes = GenerateSPiece(xPos, height);
                    //IShape
                    else if (shape == 1)
                        shapes = GenerateIPiece(xPos, height);
                    //OShape
                    else if (shape == 2)
                        shapes = GenerateOPiece(xPos, height);
                    //LShape
                    else if (shape == 3)
                        shapes = GenerateLPiece(xPos, height);
                    //TShape
                    else if (shape == 4)
                        shapes = GenerateTPiece(xPos, height);
                    //JShape
                    else if (shape == 5)
                        shapes = GenerateJPiece(xPos, height);
                    //ZShape
                    else
                        shapes = GenerateZPiece(xPos, height);

                    shadowShapeNumber = shape;
                }
            }
            else if (i == 1)
            {
                nextPiece = true;

                Transform go = GameObject.Find("NextPiece").transform;

                if (go.childCount == 4)
                {
                    foreach (Transform child in go)
                        Destroy(child.gameObject);

                    nextShapes.Clear();
                }

                int height2 = (int)GameObject.Find("NextPiece").transform.position.y;
                int xPos2 = (int)GameObject.Find("NextPiece").transform.position.x;

                //SShape
                if (shape == 0)
                    nextShapes = GenerateSPiece(xPos2, height2);
                //IShape
                else if (shape == 1)
                    nextShapes = GenerateIPiece(xPos2, height2);
                //OShape
                else if (shape == 2)
                    nextShapes = GenerateOPiece(xPos2, height2);
                //LShape
                else if (shape == 3)
                    nextShapes = GenerateLPiece(xPos2, height2);
                //TShape
                else if (shape == 4)
                    nextShapes = GenerateTPiece(xPos2, height2);
                //JShape
                else if (shape == 5)
                    nextShapes = GenerateJPiece(xPos2, height2);
                //ZShape
                else
                    nextShapes = GenerateZPiece(xPos2, height2);

                nextShapeNumber = shape;
            }
        }

        spawn = true;
        currentRot = 0;
        nextPiece = false;
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
            obj.parent = GameObject.Find("NextPiece").transform;
        else if (shadowPiece)
        {
            obj.tag = "ShadowBlock";
            obj.parent = GameObject.Find("ShadowPiece").transform;
            obj.renderer.material = materials[7];
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
            //Start from bottom of the board(without edge and block spawn space)
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

    //Check specific column for match
    private void CheckColumn (int x)
    {
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

    private void RotateShadow (Transform a, Transform b, Transform c, Transform d)
    {
        //Set parent to pivot so we can rotate
        a.parent = shadowPivot.transform;
        b.parent = shadowPivot.transform;
        c.parent = shadowPivot.transform;
        d.parent = shadowPivot.transform;

        shadowPivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

        a.parent = null;
        b.parent = null;
        c.parent = null;
        d.parent = null;

        if (CheckRotate(a.position, b.position, c.position, d.position) == false)
        {
            //Set parent to pivot so we can rotate
            a.parent = shadowPivot.transform;
            b.parent = shadowPivot.transform;
            c.parent = shadowPivot.transform;
            d.parent = shadowPivot.transform;

            shadowPivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

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
        int height = board.GetLength(1) - 4;
        int xPos = board.GetLength(0) / 2 - 1;
        shadowPiece = true;
        shadowPivot = new GameObject("ShadowRotateAround");

        //SShape
        if (shadowShapeNumber == 0)
            shadowShapes = GenerateSPiece(xPos, height);
        //IShape
        else if (shadowShapeNumber == 1)
            shadowShapes = GenerateIPiece(xPos, height);
        //OShape
        else if (shadowShapeNumber == 2)
            shadowShapes = GenerateOPiece(xPos, height);
        //LShape
        else if (shadowShapeNumber == 3)
            shadowShapes = GenerateLPiece(xPos, height);
        //TShape
        else if (shadowShapeNumber == 4)
            shadowShapes = GenerateTPiece(xPos, height);
        //JShape
        else if (shadowShapeNumber == 5)
            shadowShapes = GenerateJPiece(xPos, height);
        //ZShape
        else
            shadowShapes = GenerateZPiece(xPos, height);

        shadowPiece = false;

        onGround = false;
    }

    private void MoveDownShadowShape ()
    {
        //Spawned blocks positions
        if (shadowShapes.Count != 4)
        {
            return;
        }
        Vector3 a = shadowShapes[0].transform.position;
        Vector3 b = shadowShapes[1].transform.position;
        Vector3 c = shadowShapes[2].transform.position;
        Vector3 d = shadowShapes[3].transform.position;

        if (CheckMove(a, b, c, d) == true)
        { // Will we hit anything if we move block down(true = we can move)
            //Move block down by 1
            a = new Vector3(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y - 1.0f), a.z);
            b = new Vector3(Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y - 1.0f), b.z);
            c = new Vector3(Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y - 1.0f), c.z);
            d = new Vector3(Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y - 1.0f), d.z);

            shadowPivot.transform.position = new Vector3(shadowPivot.transform.position.x, shadowPivot.transform.position.y - 1, shadowPivot.transform.position.z);

            shadowShapes[0].transform.position = a;
            shadowShapes[1].transform.position = b;
            shadowShapes[2].transform.position = c;
            shadowShapes[3].transform.position = d;
        }
        else
        {
            print("we are hitting something");
            onGround = true;
            HighestPoint();
        }
    }

    private List<Transform> GenerateOPiece (int xPos, int height)
    {
        piece = Piece.O;

        List<Transform> list = new List<Transform>();

        if (!nextPiece)
            pivot.transform.position = new Vector3(xPos + 0.5f, height + 0.5f, 0);
        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos + 0.5f, height + 0.5f, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));

        Debug.Log("Spawned O Shape");

        return list;
    }

    private List<Transform> GenerateTPiece (int xPos, int height)
    {
        piece = Piece.T;

        List<Transform> list = new List<Transform>();

        if (!nextPiece)
            pivot.transform.position = new Vector3(xPos, height, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));

        Debug.Log("Spawned T Shape");

        return list;
    }

    private List<Transform> GenerateZPiece (int xPos, int height)
    {
        piece = Piece.Z;

        List<Transform> list = new List<Transform>();

        if (!nextPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height + 1, 0), piece));

        Debug.Log("Spawned Z Shape");

        return list;
    }

    private List<Transform> GenerateIPiece (int xPos, int height)
    {
        piece = Piece.I;

        List<Transform> list = new List<Transform>();

        if (!nextPiece)
            pivot.transform.position = new Vector3(xPos + 0.5f, height + 1.5f, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos + 0.5f, height + 1.5f, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 3, 0), piece));

        Debug.Log("Spawned I Shape");

        return list;
    }

    private List<Transform> GenerateSPiece (int xPos, int height)
    {
        piece = Piece.S;

        List<Transform> list = new List<Transform>();

        if (!nextPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));

        Debug.Log("Spawned S Shape");

        return list;
    }

    private List<Transform> GenerateLPiece (int xPos, int height)
    {
        piece = Piece.L;

        List<Transform> list = new List<Transform>();

        if (!nextPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

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

        if (!nextPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));

        Debug.Log("Spawned J Shape");
        return list;
    }

    private void DestroyShadowPiece ()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("ShadowBlock");

        for (int i = 0; i < gos.Length; i++)
        {
            Destroy(gos[i].gameObject);
        }
    }

    private void MoveLeft ()
    {
        Vector3 a = shapes[0].transform.position;
        Vector3 b = shapes[1].transform.position;
        Vector3 c = shapes[2].transform.position;
        Vector3 d = shapes[3].transform.position;

        //Move left
        if (CheckUserMove(a, b, c, d, true))
        {   //Check if we can move it left
            a.x -= 1;
            b.x -= 1;
            c.x -= 1;
            d.x -= 1;

            pivot.transform.position = new Vector3(pivot.transform.position.x - 1, pivot.transform.position.y, pivot.transform.position.z);

            shapes[0].transform.position = a;
            shapes[1].transform.position = b;
            shapes[2].transform.position = c;
            shapes[3].transform.position = d;

            shadowPivot.transform.position = new Vector3(shadowPivot.transform.position.x - 1, shadowPivot.transform.position.y, shadowPivot.transform.position.z);

            //shadowShapes[0].transform.position = new Vector2(a.x, shadowShapes[0].transform.position.y);
            //shadowShapes[1].transform.position = new Vector2(b.x, shadowShapes[1].transform.position.y);
            //shadowShapes[2].transform.position = new Vector2(c.x, shadowShapes[2].transform.position.y);
            //shadowShapes[3].transform.position = new Vector2(d.x, shadowShapes[3].transform.position.y);

            shadowShapes[0].transform.position = new Vector2(a.x, a.y);
            shadowShapes[1].transform.position = new Vector2(b.x, b.y);
            shadowShapes[2].transform.position = new Vector2(c.x, c.y);
            shadowShapes[3].transform.position = new Vector2(d.x, d.y);

            onGround = false;
        }
    }

    private void MoveRight ()
    {
        Vector3 a = shapes[0].transform.position;
        Vector3 b = shapes[1].transform.position;
        Vector3 c = shapes[2].transform.position;
        Vector3 d = shapes[3].transform.position;

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

            shadowPivot.transform.position = new Vector3(shadowPivot.transform.position.x + 1, shadowPivot.transform.position.y, shadowPivot.transform.position.z);

            //shadowShapes[0].transform.position = new Vector2(a.x, shadowShapes[0].transform.position.y);
            //shadowShapes[1].transform.position = new Vector2(b.x, shadowShapes[1].transform.position.y);
            //shadowShapes[2].transform.position = new Vector2(c.x, shadowShapes[2].transform.position.y);
            //shadowShapes[3].transform.position = new Vector2(d.x, shadowShapes[3].transform.position.y);

            shadowShapes[0].transform.position = new Vector2(a.x, a.y);
            shadowShapes[1].transform.position = new Vector2(b.x, b.y);
            shadowShapes[2].transform.position = new Vector2(c.x, c.y);
            shadowShapes[3].transform.position = new Vector2(d.x, d.y);

            onGround = false;
        }
    }

    private void HighestPoint ()
    {
        int highPoint = 0;
        int highestPoint = 0;

        for (int x = 1; x < board.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < board.GetLength(1) - 1; y++)
            {
                if (board[x, y] == 1 && (x >= 1 && x <= 10 && y != 0))
                {
                    if (y > highPoint)
                    {
                        highestPoint = y;
                        print("Highest point on the board is " + highestPoint);
                    }
                    highPoint = y;
                    print("Board[" + x + "," + y + "]:" + board[x, y]);
                }
            }
        }
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
* TODO: WORK ON SHADOW SHAPE
*/