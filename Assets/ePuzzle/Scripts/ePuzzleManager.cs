using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro.Examples;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sprite = UnityEngine.Sprite;

public class ePuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject _puzzleImage;
    [SerializeField] private GameObject _imageViewport;
    [SerializeField] private GameObject _imageViewportParent;
    [SerializeField] private GameObject _imageBackground;
    Vector2 _imageSize;
    [SerializeField] private GameObject _puzzlePiece;
    [SerializeField] private GameObject _puzzleCanvas;
    [SerializeField] private GameObject _piecesParent;
    private List<GameObject> _pieces = new();
    [SerializeField] private List<Sprite> _puzzlePieces;
    private List<string> _puzzlePiecesNames = new();
    private Dictionary<string, Sprite> puzzlePieces = new();
    
    // Start is called before the first frame update
    void Start()
    {
        ResizeImage();
        LoadImage();
        CreatePieces();
        SetBackgroundTransparency();
        SetBackgroundColor();
        // Create the dictionary with the puzzle pieces to get them by code
        foreach (var puzzlePiece in _puzzlePieces)
        {
            var pieceCode = puzzlePiece.name;
            if (!_puzzlePiecesNames.Contains(puzzlePiece.name))
            {
                _puzzlePiecesNames.Add(puzzlePiece.name);
                puzzlePieces.Add(puzzlePiece.name, puzzlePiece);
            }
            else
            {
                Debug.Log($"List: The name {puzzlePiece.name} is already in the list");
            }
        }
        GenerateJigsawPuzzleMatrix(Configuration.ePuzzleRows, Configuration.ePuzzleColumns);
        SetPiecesPosition();
        MoveJigsawPieces();
        //StartCoroutine(MovePiecesPlaceholder());
    }

    private IEnumerator MovePiecesPlaceholder()
    {
        yield return new WaitForSeconds(3);
        switch (Configuration.ePuzzlePiecesPosition)
        {
            case (Configuration.Position.UpLeft):
                _piecesParent.transform.localPosition = new Vector3(-250, 125, 0);
                break;
            case Configuration.Position.UpRight:
                _piecesParent.transform.localPosition = new Vector3(250, 125, 0);
                break;
            case Configuration.Position.DownLeft:
                _piecesParent.transform.localPosition = new Vector3(-250, -125, 0);
                break;
            case Configuration.Position.DownRight:
                _piecesParent.transform.localPosition = new Vector3(250, -125, 0);
                break;
        }
    }

    private void SetPiecesPosition()
    {
        foreach (var p in _pieces) { p.GetComponent<mouseDrag>().SetInitialPosition(); }
    }

    private void MoveJigsawPieces()
    {
        foreach (var p in _pieces)
        {
            p.transform.SetParent(_piecesParent.transform);
        }
        switch (Configuration.ePuzzleImagePosition)
        {
            case (Configuration.Position.UpLeft):
                _imageViewportParent.transform.localPosition = new Vector3(-250, 125, 0);
                break;
            case Configuration.Position.UpRight:
                _imageViewportParent.transform.localPosition = new Vector3(250, 125, 0);
                break;
            case Configuration.Position.DownLeft:
                _imageViewportParent.transform.localPosition = new Vector3(-250, -125, 0);
                break;
            case Configuration.Position.DownRight:
                _imageViewportParent.transform.localPosition = new Vector3(250, -125, 0);
                break;
        }
        switch (Configuration.ePuzzleImagePosition)
        {
            case (Configuration.Position.UpLeft):
                _piecesParent.transform.localPosition = new Vector3(-250, 125, 0);
                break;
            case Configuration.Position.UpRight:
                _piecesParent.transform.localPosition = new Vector3(250, 125, 0);
                break;
            case Configuration.Position.DownLeft:
                _piecesParent.transform.localPosition = new Vector3(-250, -125, 0);
                break;
            case Configuration.Position.DownRight:
                _piecesParent.transform.localPosition = new Vector3(250, -125, 0);
                break;
        }
    }

    private void SetBackgroundTransparency()
    {
        _imageBackground.GetComponent<Image>().color = new Color(1, 1, 1, Configuration.ePuzzleImageTransparency);
        _puzzleImage.GetComponent<Image>().color = new Color(1, 1, 1, Configuration.ePuzzleImageTransparency);
    }

    private void SetBackgroundColor()
    {
        // set the background color of the main camera
        Camera myCamera = Camera.main;
        myCamera.backgroundColor = Configuration.ePuzzleBackgroundColor;
    }

    private void CreatePieces()
    {
        // create a new game object for each piece
        for (var i = 0; i < Configuration.ePuzzleColumns; i++)
        {
            for (var j = 0; j < Configuration.ePuzzleRows; j++)
            {
                var piece = Instantiate(_puzzlePiece, _puzzleCanvas.transform);
                
                _pieces.Add(piece);
                
                // Set the name of the piece
                piece.name = $"Piece {j} {i}";
                
                // Set the size of the piece
                piece.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    1*_imageSize.x / Configuration.ePuzzleColumns, 
                    1*_imageSize.y / Configuration.ePuzzleRows);
                
                // Resize the boxCollider
                BoxCollider2D boxCollider = piece.GetComponent<BoxCollider2D>();
                Vector2 imageSize = piece.GetComponent<RectTransform>().sizeDelta;
                boxCollider.size = new Vector2(imageSize.x, imageSize.y);
                boxCollider.offset = new Vector2(imageSize.x / 2, -imageSize.y / 2);
                
                // Set the size of the piece
                piece.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    1.56f*_imageSize.x / Configuration.ePuzzleColumns, 
                    1.56f*_imageSize.y / Configuration.ePuzzleRows);
                
                // Set the position of the piece
                piece.GetComponent<RectTransform>().localPosition = new Vector2(
                    _imageSize.x / Configuration.ePuzzleColumns * i,
                    -_imageSize.y / Configuration.ePuzzleRows * j);
                var localPosition = piece.transform.localPosition;
                localPosition = new Vector3(
                    localPosition.x+250-_imageSize.x/2, 
                    localPosition.y,
                    0);
                piece.transform.localPosition = localPosition;
                
                // offset problem
                var offset = (0.56f*_imageSize.x / Configuration.ePuzzleColumns) /2;
                localPosition = new Vector3(
                    localPosition.x - offset,
                    localPosition.y + offset,
                    localPosition.z);
                piece.transform.localPosition = localPosition;
                boxCollider.offset = new Vector2(boxCollider.offset.x + offset, boxCollider.offset.y - offset);
                
                var pieceViewport = Instantiate(_imageViewportParent, _puzzleCanvas.transform);
                pieceViewport.transform.parent = piece.transform;
            }
        }
    }

    public void LoadImage()
    {
        var folder = FileManager.jigsawPath;
        var pathPNG = Path.Combine(folder, Configuration.ePuzzleImage + ".png");
        var pathJPG = Path.Combine(folder, Configuration.ePuzzleImage + ".jpg");
        var pathJPEG = Path.Combine(folder, Configuration.ePuzzleImage + ".jpeg");
        string path=null;
        // Check if the image exists in the jigsaw folder and infer the type
        // Check if PNG exists
        if (File.Exists(pathPNG))
            path = pathPNG;
        if (File.Exists(pathJPG))
            path = pathJPG;
        if (File.Exists(pathJPEG))
            path = pathJPEG;
        if (path == null)
        {
            Debug.LogError("Image not found");
        }
        else
        {
            // Load the image
            Debug.Log($"Loading image from {path}");
            var bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            _puzzleImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            var blurredTexture = new Texture2D(2, 2);
            blurredTexture.LoadImage(bytes);
            // Reduce the size of the texture because the blur is very slow
            blurredTexture = ResizeTexture(blurredTexture, 500, 200);
            // Blur the image
            blurredTexture = LinearBlur.Blur(blurredTexture, 1,5);
            // Set the background image to be the same image
            _imageBackground.GetComponent<Image>().sprite = Sprite.Create(blurredTexture, new Rect(0, 0, blurredTexture.width, blurredTexture.height), Vector2.zero);
        }
    }

    private Texture2D ResizeTexture(Texture2D texture, int width, int height)
    {
        var resizedTexture = new Texture2D(width, height);
        var scale = new Vector2((float)texture.width / width, (float)texture.height / height);
        for (var y = 0; y < resizedTexture.height; y++)
        {
            for (var x = 0; x < resizedTexture.width; x++)
            {
                var newX = Mathf.FloorToInt(x * scale.x);
                var newY = Mathf.FloorToInt(y * scale.y);
                resizedTexture.SetPixel(x, y, texture.GetPixel(newX, newY));
            }
        }
        resizedTexture.Apply();
        return resizedTexture;
    }
    
    private void ResizeImage()
    {
        Debug.Log($"Resizing Image to {Configuration.ePuzzleAspectRatio}");
        RectTransform rectTransform = _imageViewport.GetComponent<RectTransform>();
        switch (Configuration.ePuzzleAspectRatio)
        {
            case Configuration.AspectRatio.Square:
                // Resize the image to a square
                rectTransform.sizeDelta = new Vector2(250, 250);
                break;
            case Configuration.AspectRatio.Landscape:
                // Resize the image to a landscape
                rectTransform.sizeDelta = new Vector2(333, 250);
                break;
            case Configuration.AspectRatio.Portrait:
                // Resize the image to a portrait
                rectTransform.sizeDelta = new Vector2(250, 333);
                break;
            case Configuration.AspectRatio.Widescreen:
                // Resize the image to a widescreen
                rectTransform.sizeDelta = new Vector2(500, 250);
                break;
        }
        _imageSize = rectTransform.sizeDelta;
    }
    
    private Sprite GetPiece(string pieceCode)
    {
        Debug.Log($"Getting piece {pieceCode} -----------------------------");
        string storedOrientation = "";
        Sprite foundPiece;
        // find sprite stored in the dictionary
        if (puzzlePieces.ContainsKey(pieceCode))
        {
            foundPiece =  puzzlePieces[pieceCode];
            storedOrientation = pieceCode;
            Debug.Log($"piece found for {pieceCode} storedOrientation: {storedOrientation}");
        }
        else
        {
            Debug.LogError($"No piece found for {pieceCode}");
            return null;
        }
        return foundPiece;
    }

    private void GenerateJigsawPuzzleMatrix(int rows, int cols)
    {
        // The matrix is a 2D array of strings
        // Each string is a piece code
        // The piece code is a string of 4 characters
        // Each character is a border of a piece
        // The borders are represented by a number
        // A border can be a flat edge (2), a knob (1) or a hole (0)
        // The order of the borders is: up, right, down, left
        // The first piece (top left) is always "2XX2" (X is random) because the up and left borders are edges
        // All pieces in the first row are "2XXX" because the up border is an edge
        // All pieces in the last row are "XX2X" because the down border is an edge
        // All pieces in the first column are "XXX2" because the left border is an edge
        // All pieces in the last column are "X2XX" because the right border is an edge
        // All other pieces can vary in their borders but they must match the borders of the pieces around them
        // So if a border is a knob (1), the border of the neighbor piece is a hole (0)
        // The matrix is generated randomly
        var matrix = new string[rows, cols];
        // Generate the first piece
        var firstPiece = "2" + Random.Range(0, 2) + Random.Range(0, 2) + "2";
        matrix[0, 0] = firstPiece;
        // Generate the first row
        for (var i = 1; i < cols; i++)
        {
            var previousPieceCode = matrix[0, i - 1];
            // previousPieceCode[1] is the right border of the previous piece
            var leftBorder = previousPieceCode[1].ToString()=="0"? "1" : "0";
            var upBorder = "2";
            var rightBorder = Random.Range(0, 2).ToString();
            if (i==cols-1)
                rightBorder = "2";
            var downBorder = Random.Range(0, 2).ToString();
            var pieceCode = upBorder + rightBorder + downBorder + leftBorder;
            matrix[0, i] = pieceCode;
        }
        // Generate the first column
        for (var i = 1; i < rows; i++)
        {
            var previousPieceCode = matrix[i - 1, 0];
            var leftBorder = "2";
            var upBorder = previousPieceCode[2].ToString() == "0" ? "1" : "0";
            var rightBorder = Random.Range(0, 2).ToString();
            var downBorder = Random.Range(0, 2).ToString();
            if (i == rows - 1)
                downBorder = "2";
            var pieceCode = upBorder + rightBorder + downBorder + leftBorder;
            matrix[i, 0] = pieceCode;
        }
        // Generate the rest of the matrix
        for (var i = 1; i < rows; i++)
        {
            for (var j = 1; j < cols; j++)
            {
                var previousUpPieceCode = matrix[i - 1, j];
                var previousLeftPieceCode = matrix[i, j - 1];
                var upBorder = previousUpPieceCode[2].ToString() == "0" ? "1" : "0";
                var leftBorder = previousLeftPieceCode[1].ToString() == "0" ? "1" : "0";
                var rightBorder = Random.Range(0, 2).ToString();
                // If the piece is in the last column, the right border is an edge (2)
                if (j == cols - 1)
                    rightBorder = "2";
                var downBorder = Random.Range(0, 2).ToString();
                // If the piece is in the last row, the down border is an edge (2)
                if (i == rows - 1)
                    downBorder = "2";
                var pieceCode = upBorder + rightBorder + downBorder + leftBorder;
                matrix[i, j] = pieceCode;
            }
        }
        // Generate the puzzle pieces
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                //if (i!=0 && j!=0)
                //    continue;
                var pieceCode = matrix[i, j];
                var piece = GetPiece(pieceCode);
                // Find the game object that represents the piece in the format "Piece i j" (e.g. Piece 0 0)
                var pieceName = "Piece " + i + " " + j;
                var pieceObject = GameObject.Find(pieceName);
                // Set the sprite of the piece
                pieceObject.GetComponent<Image>().sprite = piece;
            }
        }
        //print(matrix);
    }
}
