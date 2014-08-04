using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviour
{
    //Board
    public int[,] board;

    //Block
    public Transform block;

    public Material blockMaterial;

    //Spawn boolean
    public bool spawn;

    // Constant MovementTime for movement keys
    private const float movementTime = 0.15f;

    // Seconds before block will move left is held
    public float leftMovementTime = movementTime;

    // Seconds before block will move right is held
    public float rightMovementTime = movementTime;

    //Block fall speed
    public float blockFallSpeed;

    private const float SOFT_DROP_SPEED = .1f;

    //Game over level
    public int gameOverHeight = 22; //20 board + 2 edge

    //Current spawned shapes
    private List<Transform> shapes = new List<Transform>();

    private List<Transform> nextShapes = new List<Transform>();

    private List<Transform> shadowShapes = new List<Transform>();

    public Vector3[] shadowPosition;

    //Set true if game over
    private bool gameOver;

    private bool nextPiece;

    private bool softDrop;
    private bool shadowPiece;

    public bool holding = false;

    public bool onGround = false;

    //Current rotation of an object
    private int currentRot = 0;

    //Current pivot of the shape
    private GameObject pivot;

    private GameObject shadowPivot;

    public int score;

    private Piece piece;

    public int linesCleared;

    private int shadowShapeNumber;

    private int nextShapeNumber;

    private IEnumerator Start ()
    {
        //Set board width and height
        board = new int[12, 25];
        piece = Piece.None;
        shadowPosition = new Vector3[4];
        GenerateBoard();//Generate board
        SpawnShape();
        SpawnShadowShape();
        blockFallSpeed = Level.GetGameSpeed();

        yield return StartCoroutine("MoveDown", blockFallSpeed);
    }

    private void HardDrop ()
    {
        StopCoroutine("MoveDown");

        Vector3 a = shadowShapes[0].transform.position;
        Vector3 b = shadowShapes[1].transform.position;
        Vector3 c = shadowShapes[2].transform.position;
        Vector3 d = shadowShapes[3].transform.position;

        // sends the score method the difference in y position between the shape
        // and shadow shape before it drops.
        Score.IncreaseScoreHardDrop(Mathf.RoundToInt(shapes[3].position.y - d.y));

        shapes[0].position = a;
        shapes[1].position = b;
        shapes[2].position = c;
        shapes[3].position = d;

        StartCoroutine("MoveDown", blockFallSpeed);
    }

    private void Update ()
    {
        blockFallSpeed = Level.GetGameSpeed();
        // If there is block
        if (spawn && shapes.Count == 4)
        {
            if (!onGround)
            {
                StartCoroutine(MoveDownShadowShape());
            }
            else
            {
                StopCoroutine(MoveDownShadowShape());
            }

            // Reset Left Arrow timer
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                leftMovementTime = movementTime;
                holding = false;
            }

            //Move left continuously
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                if (!holding)
                {
                    MoveLeft();
                }

            //Move left
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                leftMovementTime -= Time.deltaTime;

                if (leftMovementTime <= 0)
                {
                    holding = true;
                    MoveLeft();
                    leftMovementTime = movementTime;
                }
            }

            // Reset right Arrow timer
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                rightMovementTime = movementTime;
                holding = false;
            }

            //Move right continuously
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }

            //Move right
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rightMovementTime -= Time.deltaTime;
                if (rightMovementTime <= 0)
                {
                    MoveRight();
                    rightMovementTime = movementTime;
                }
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                softDrop = false;
                StopCoroutine("MoveDown");
                StartCoroutine("MoveDown", blockFallSpeed);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //Move down fast
                StopCoroutine("MoveDown");
                softDrop = true;
                StartCoroutine("MoveDown", SOFT_DROP_SPEED);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //Rotate
                Rotate(shapes[0].transform, shapes[1].transform, shapes[2].transform, shapes[3].transform, RotateDirection.Right);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                //Rotate
                Rotate(shapes[0].transform, shapes[1].transform, shapes[2].transform, shapes[3].transform, RotateDirection.Left);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                HardDrop();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Rotate(shapes[0].transform, shapes[1].transform, shapes[2].transform, shapes[3].transform, RotateDirection.Left);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                Rotate(shapes[0].transform, shapes[1].transform, shapes[2].transform, shapes[3].transform, RotateDirection.Right);
            }
        }

        print(blockFallSpeed);
    }

    private IEnumerator MoveDown (float time)
    {
        while (true)
        {
            //Spawned blocks positions
            if (shapes.Count != 4)
            {
                yield break;
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
                Score.IncreaseScoreLine(linesCleared);
                CheckRow(gameOverHeight); //Check for game over
                //****************************************************

                linesCleared = 0;
                if (gameOver)
                {
                    StopAllCoroutines();
                    yield break;
                }

                shapes.Clear(); //Clear spawned blocks from array
                shadowShapes.Clear();
                DestroyShadowPiece();
                spawn = false; //Spawn a new block
                nextPiece = false;
                SpawnShape();
                SpawnShadowShape();
            }

            if (softDrop)
            {
                Score.IncreaseScoreSoftDrop();
            }

            yield return new WaitForSeconds(time);
        }
    }

    private IEnumerator MoveDownShadowShape ()
    {
        while (true)
        {
            //Spawned blocks positions
            if (shadowShapes.Count != 4)
            {
                yield break;
            }
            Vector3 a = shadowPosition[0];
            Vector3 b = shadowPosition[1];
            Vector3 c = shadowPosition[2];
            Vector3 d = shadowPosition[3];

            if (CheckMove(a, b, c, d) == true)
            { // Will we hit anything if we move block down(true = we can move)
                //Move block down by 1
                a = new Vector3(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y - 1.0f), a.z);
                b = new Vector3(Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y - 1.0f), b.z);
                c = new Vector3(Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y - 1.0f), c.z);
                d = new Vector3(Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y - 1.0f), d.z);

                shadowPivot.transform.position = new Vector3(shadowPivot.transform.position.x, shadowPivot.transform.position.y - 1, shadowPivot.transform.position.z);

                shadowPosition[0] = a;
                shadowPosition[1] = b;
                shadowPosition[2] = c;
                shadowPosition[3] = d;
            }
            else
            {
                //             print("we are hitting something");

                onGround = true;

                shadowShapes[0].renderer.material.color = new Color32(0, 0, 0, 100);
                shadowShapes[1].renderer.material.color = new Color32(0, 0, 0, 100);
                shadowShapes[2].renderer.material.color = new Color32(0, 0, 0, 100);
                shadowShapes[3].renderer.material.color = new Color32(0, 0, 0, 100);

                shadowShapes[0].transform.position = a;
                shadowShapes[1].transform.position = b;
                shadowShapes[2].transform.position = c;
                shadowShapes[3].transform.position = d;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private bool CheckMove (Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        try
        {
            //Check, if we move a block down will it hit something
            if (board[Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y - 1)] == 1)
            {
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
        }
        catch
        {
            return true;
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
                if (!holding)
                {
                    SoundManager.Play(SoundManager.SoundEffectTypes.ErrorSound);
                }

                return false;
            }
        }
        else
        {//Right
            if (board[Mathf.RoundToInt(a.x + 1), Mathf.RoundToInt(a.y)] == 1 || board[Mathf.RoundToInt(b.x + 1), Mathf.RoundToInt(b.y)] == 1 || board[Mathf.RoundToInt(c.x + 1), Mathf.RoundToInt(c.y)] == 1 || board[Mathf.RoundToInt(d.x + 1), Mathf.RoundToInt(d.y)] == 1)
            {
                SoundManager.Play(SoundManager.SoundEffectTypes.ErrorSound);
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
                        Material mat = new Material(blockMaterial);
                        cube.transform.position = new Vector3(x, y, 1);
                        cube.transform.localEulerAngles = new Vector3(0, 0, 180);
                        if (y >= 22)
                        {
                            mat.color = new Color32(110, 110, 110, 0);
                        }
                        else
                        {
                            mat.color = new Color32(110, 110, 110, 90);
                        }

                        cube.renderer.material = mat;

                        cube.transform.parent = boardObject.transform;
                    }
                    else if (y < board.GetLength(1) - 2)
                    {
                        board[x, y] = 1;
                    }
                }
                else if ((y < board.GetLength(1) - 2))
                {
                    //Left and right edge
                    board[x, y] = 1;
                }
            }
        }
    }

    private void SpawnShape ()
    {
        int height = board.GetLength(1) - 4;
        int xPos = board.GetLength(0) / 2 - 1;

        Transform go = GameObject.Find("NextPiece").transform;

        for (int i = 0; i < 2; i++)
        {
            int shape = UnityEngine.Random.Range(0, 7);//Random shape
            shape = 1;
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
                        shapes = GenerateIPiece(xPos, height - 2);
                    //OShape
                    else if (nextShapeNumber == 2)
                        shapes = GenerateOPiece(xPos, height);
                    //LShape
                    else if (nextShapeNumber == 3)
                        shapes = GenerateLPiece(xPos, height - 1);
                    //TShape
                    else if (nextShapeNumber == 4)
                        shapes = GenerateTPiece(xPos, height);
                    //JShape
                    else if (nextShapeNumber == 5)
                        shapes = GenerateJPiece(xPos, height - 1);
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
                        shapes = GenerateSPiece(xPos, height + 1);
                    //IShape
                    else if (shape == 1)
                        shapes = GenerateIPiece(xPos, height - 1);
                    //OShape
                    else if (shape == 2)
                        shapes = GenerateOPiece(xPos, height + 1);
                    //LShape
                    else if (shape == 3)
                        shapes = GenerateLPiece(xPos, height);
                    //TShape
                    else if (shape == 4)
                        shapes = GenerateTPiece(xPos, height + 1);
                    //JShape
                    else if (shape == 5)
                        shapes = GenerateJPiece(xPos, height);
                    //ZShape
                    else
                        shapes = GenerateZPiece(xPos, height + 1);

                    shadowShapeNumber = shape;
                }
            }
            else if (i == 1)
            {
                nextPiece = true;

                if (go.childCount == 4)
                {
                    foreach (Transform child in go)
                        Destroy(child.gameObject);
                    nextShapes.Clear();
                    go.localScale = new Vector3(1f, 1f, 1f);
                }

                float height2 = go.transform.position.y;
                float xPos2 = go.transform.position.x;

                go.localEulerAngles = new Vector3(0, 0, 0);
                //SShape
                if (shape == 0)
                    nextShapes = GenerateSPiece(xPos2, height2);
                //IShape
                else if (shape == 1)
                {
                    nextShapes = GenerateIPiece(xPos2 + .5f, height2 - 1.5f);
                    Rotate(nextShapes[0], nextShapes[1], nextShapes[2], nextShapes[3], RotateDirection.Left);
                }
                //OShape
                else if (shape == 2)
                    nextShapes = GenerateOPiece(xPos2 - .5f, height2);
                //LShape
                else if (shape == 3)
                {
                    nextShapes = GenerateLPiece(xPos2, height2 - 1f);
                    Rotate(nextShapes[0], nextShapes[1], nextShapes[2], nextShapes[3], RotateDirection.Left);
                }
                //TShape
                else if (shape == 4)
                    nextShapes = GenerateTPiece(xPos2, height2);
                //JShape
                else if (shape == 5)
                {
                    nextShapes = GenerateJPiece(xPos2, height2 - 1f);
                    Rotate(nextShapes[0], nextShapes[1], nextShapes[2], nextShapes[3], RotateDirection.Right);
                }
                //ZShape
                else
                    nextShapes = GenerateZPiece(xPos2, height2);

                nextShapeNumber = shape;

                //print("next Piece Created!");

                go.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            }
        }

        spawn = true;
        currentRot = 0;
        nextPiece = false;
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

        shadowPosition[0] = shadowShapes[0].transform.position;
        shadowPosition[1] = shadowShapes[1].transform.position;
        shadowPosition[2] = shadowShapes[2].transform.position;
        shadowPosition[3] = shadowShapes[3].transform.position;

        if (shadowShapeNumber == 1)
            Rotate(shapes[0], shapes[1], shapes[2], shapes[3], RotateDirection.Right);
        else if (shadowShapeNumber == 3)
            Rotate(shapes[0], shapes[1], shapes[2], shapes[3], RotateDirection.Left);
        else if (shadowShapeNumber == 5)
            Rotate(shapes[0], shapes[1], shapes[2], shapes[3], RotateDirection.Right);
    }

    //Create a block at the position
    private Transform GenerateBlock (Vector3 pos, Piece piece)
    {
        Transform obj = null;
        Material mat = new Material(blockMaterial);

        switch (piece)
        {
            case Piece.Z:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                mat.color = new Color32(82, 255, 23, 255); // Green
                obj.renderer.material = mat;
                break;

            case Piece.J:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                mat.color = new Color32(30, 117, 185, 255); // Dark Teal
                obj.renderer.material = mat;
                break;

            case Piece.I:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                mat.color = new Color32(247, 147, 35, 255); // Orange
                obj.renderer.material = mat;
                break;

            case Piece.L:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                mat.color = new Color32(255, 37, 54, 255); // Red
                obj.renderer.material = mat;
                break;

            case Piece.S:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                mat.color = new Color32(53, 254, 253, 255); // Light Teal
                obj.renderer.material = mat;
                break;

            case Piece.O:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                mat.color = new Color32(200, 116, 253, 255); // Purple
                obj.renderer.material = mat;
                break;

            case Piece.T:
                obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
                obj.tag = "Block";
                mat.color = new Color32(253, 255, 55, 255); // Off Yellow
                obj.renderer.material = mat;
                break;
        }

        if (nextPiece)
            obj.parent = GameObject.Find("NextPiece").transform;
        else if (shadowPiece)
        {
            obj.tag = "ShadowBlock";
            obj.parent = GameObject.Find("ShadowPiece").transform;
            mat.color = new Color32(0, 0, 0, 0); // Black Low Transparent Shadow Piece
            obj.renderer.material = mat;
        }

        obj.transform.localEulerAngles = new Vector3(0, 0, 180);
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
                            //The row we need to destroy
                            if (height == y)
                            {
                                //Set empty space
                                board[xPos, height] = 0;

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
            Level.UpdateLinesLeft();
            linesCleared++;
            CheckRow(y); //We moved blocks down, check again this row
        }
        else if (y + 1 < board.GetLength(1) - 3)
        {
            CheckRow(y + 1); //Check row above this
        }
    }

    private void Rotate (Transform a, Transform b, Transform c, Transform d, RotateDirection rotateDirection)
    {
        ResetShadowPosition();

        if (nextPiece)
        {
            Transform go = GameObject.Find("NextPiece").transform;

            switch (rotateDirection)
            {
                case RotateDirection.Left:
                    go.localEulerAngles = new Vector3(0, 0, 90);
                    nextShapes[0].transform.localEulerAngles = new Vector3(0, 0, 90);
                    nextShapes[1].transform.localEulerAngles = new Vector3(0, 0, 90);
                    nextShapes[2].transform.localEulerAngles = new Vector3(0, 0, 90);
                    nextShapes[3].transform.localEulerAngles = new Vector3(0, 0, 90);
                    break;

                case RotateDirection.Right:
                    go.localEulerAngles = new Vector3(0, 0, -90);
                    nextShapes[0].transform.localEulerAngles = new Vector3(0, 0, -90);
                    nextShapes[1].transform.localEulerAngles = new Vector3(0, 0, -90);
                    nextShapes[2].transform.localEulerAngles = new Vector3(0, 0, -90);
                    nextShapes[3].transform.localEulerAngles = new Vector3(0, 0, -90);
                    break;
            }
        }
        else
        {
            //Set parent to pivot so we can rotate
            a.parent = pivot.transform;
            b.parent = pivot.transform;
            c.parent = pivot.transform;
            d.parent = pivot.transform;

            switch (rotateDirection)
            {
                case RotateDirection.Left:
                    currentRot += 90;
                    break;

                case RotateDirection.Right:
                    currentRot -= 90;
                    break;
            }

            if (currentRot == 360)
            { //Reset rotation
                currentRot = 0;
            }

            pivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

            a.parent = null;
            b.parent = null;
            c.parent = null;
            d.parent = null;

            //rotates the block itself so that texture is always facing the right way.
            shapes[0].transform.localEulerAngles = new Vector3(0, 0, 180);
            shapes[1].transform.localEulerAngles = new Vector3(0, 0, 180);
            shapes[2].transform.localEulerAngles = new Vector3(0, 0, 180);
            shapes[3].transform.localEulerAngles = new Vector3(0, 0, 180);

            if (CheckRotate(a.position, b.position, c.position, d.position) == false)
            {
                //Set parent to pivot so we can rotate
                a.parent = pivot.transform;
                b.parent = pivot.transform;
                c.parent = pivot.transform;
                d.parent = pivot.transform;

                switch (rotateDirection)
                {
                    case RotateDirection.Left:
                        currentRot -= 90;
                        break;

                    case RotateDirection.Right:
                        currentRot += 90;
                        break;
                }
                pivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

                a.parent = null;
                b.parent = null;
                c.parent = null;
                d.parent = null;
                //rotates the block itself so that texture is always facing the right way.
                shapes[0].transform.localEulerAngles = new Vector3(0, 0, 180);
                shapes[1].transform.localEulerAngles = new Vector3(0, 0, 180);
                shapes[2].transform.localEulerAngles = new Vector3(0, 0, 180);
                shapes[3].transform.localEulerAngles = new Vector3(0, 0, 180);
            }

            shadowPivot.transform.position = new Vector3(pivot.transform.position.x, pivot.transform.position.y, pivot.transform.position.z);
            shadowPosition[0] = new Vector3(shapes[0].position.x, shapes[0].position.y);
            shadowPosition[1] = new Vector3(shapes[1].position.x, shapes[1].position.y);
            shadowPosition[2] = new Vector3(shapes[2].position.x, shapes[2].position.y);
            shadowPosition[3] = new Vector3(shapes[3].position.x, shapes[3].position.y);

            onGround = false;
        }
    }

    private bool CheckRotate (Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        //         try
        //         {
        if (Mathf.RoundToInt(a.x) <= 0 || Mathf.RoundToInt(b.x) <= 0 || Mathf.RoundToInt(c.x) <= 0 || Mathf.RoundToInt(d.x) <= 0)
        {
            print("can't rotate");
            return false;
        }

        if (Mathf.RoundToInt(a.x) < board.GetLength(0) - 1)
        {//Check if block is in board
            print("is on board");
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
        //       }
        //         catch
        //         {
        //             Debug.LogWarning("Oops I rotated off the board; fix me please.");
        //             return false;
        //         }
        print("Can Rotate");
        return true; //We can rotate
    }

    private List<Transform> GenerateOPiece (float xPos, float height)
    {
        piece = Piece.O;

        List<Transform> list = new List<Transform>();

        if (!nextPiece && !shadowPiece)
            pivot.transform.position = new Vector3(xPos + 0.5f, height + 0.5f, 0);
        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos + 0.5f, height + 0.5f, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));

        //Debug.Log("Spawned O Shape");

        return list;
    }

    private List<Transform> GenerateTPiece (float xPos, float height)
    {
        piece = Piece.T;

        List<Transform> list = new List<Transform>();

        if (!nextPiece && !shadowPiece)
            pivot.transform.position = new Vector3(xPos, height, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));

        //Debug.Log("Spawned T Shape");

        return list;
    }

    private List<Transform> GenerateZPiece (float xPos, float height)
    {
        piece = Piece.Z;

        List<Transform> list = new List<Transform>();

        if (!nextPiece && !shadowPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height + 1, 0), piece));

        //Debug.Log("Spawned Z Shape");

        return list;
    }

    private List<Transform> GenerateIPiece (float xPos, float height)
    {
        piece = Piece.I;

        List<Transform> list = new List<Transform>();

        if (!nextPiece && !shadowPiece)
            pivot.transform.position = new Vector3(xPos + 0.5f, height + 1.5f, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos + 0.5f, height + 1.5f, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 3, 0), piece));

        //Debug.Log("Spawned I Shape");

        return list;
    }

    private List<Transform> GenerateSPiece (float xPos, float height)
    {
        piece = Piece.S;

        List<Transform> list = new List<Transform>();

        if (!nextPiece && !shadowPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height + 1, 0), piece));

        //Debug.Log("Spawned S Shape");

        return list;
    }

    private List<Transform> GenerateLPiece (float xPos, float height)
    {
        piece = Piece.L;

        List<Transform> list = new List<Transform>();

        if (!nextPiece && !shadowPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos + 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));

        //Debug.Log("Spawned L Shape");

        return list;
    }

    private List<Transform> GenerateJPiece (float xPos, float height)
    {
        piece = Piece.J;

        List<Transform> list = new List<Transform>();

        if (!nextPiece & !shadowPiece)
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

        if (shadowPiece)
            shadowPivot.transform.position = new Vector3(xPos, height + 1, 0);

        list.Add(GenerateBlock(new Vector3(xPos, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos - 1, height, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 1, 0), piece));
        list.Add(GenerateBlock(new Vector3(xPos, height + 2, 0), piece));

        //Debug.Log("Spawned J Shape");
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
        ResetShadowPosition();

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

            shadowPosition[0] = new Vector2(a.x, a.y);
            shadowPosition[1] = new Vector2(b.x, b.y);
            shadowPosition[2] = new Vector2(c.x, c.y);
            shadowPosition[3] = new Vector2(d.x, d.y);

            onGround = false;
        }
    }

    private void MoveRight ()
    {
        ResetShadowPosition();

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

            shadowPosition[0] = new Vector2(a.x, a.y);
            shadowPosition[1] = new Vector2(b.x, b.y);
            shadowPosition[2] = new Vector2(c.x, c.y);
            shadowPosition[3] = new Vector2(d.x, d.y);

            onGround = false;
        }
    }

    private void ResetShadowPosition ()
    {
        shadowPosition[0] = Vector3.zero;
        shadowPosition[1] = Vector3.zero;
        shadowPosition[2] = Vector3.zero;
        shadowPosition[3] = Vector3.zero;
    }

    private void OutputGrid ()
    {
        for (int x = 0; x < board.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < board.GetLength(1) - 1; y++)
            {
                print("[x:" + x + " | " + "y:" + y + "]");
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

internal enum RotateDirection
{
    Left,
    Right
}

/*
 * TODO: FIX THE FALLSPEED DEPENDING ON CURRENT LEVEL. (Completed)
 * TODO: MAKE THE SHADOW PIECE COMPLETELY TRANSPARENT UNTIL IT HAS TOUCHED THE GROUND.(Completed)
 * TODO: CLASSIC TETRIS USER INTERFACE CREATION. (Complete)
 *
 * TODO: WORK ON ROTATION SO THERE IS NO ERRORS WHEN ROTATING(Completed Kinda)
 * TODO: FIX GAME OVER GLITCH
*/