using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
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
    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _puzzlePieceViewport;
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

        /*var _puzzlePiecesExtended = new List<Sprite>();
        foreach (var sprite in _puzzlePieces)
        {
            var up = sprite.name[0].ToString();
            var right = sprite.name[1].ToString();
            var down = sprite.name[2].ToString();
            var left = sprite.name[3].ToString();
            var filename = $"OriginalSprite";
            var countCopy = 0;
            while (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png")))
                filename = $"OriginalSprite({countCopy++})";
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png");
            var bytes = sprite.texture.EncodeToPNG();
            //File.WriteAllBytes(path, bytes);
            _puzzlePiecesExtended.Add(sprite);
            // generate a 90 rotated sprite
            var rotatedSprite = RotatePiece(sprite);
            rotatedSprite.name = $"{left}{up}{right}{down}";
            _puzzlePiecesExtended.Add(rotatedSprite);
            // Save the rotatedSprite Into the computer local storage
            filename = $"RotatedSprite";
            countCopy = 0;
            while (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png")))
                filename = $"RotatedSprite({countCopy++})";
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png");
            bytes = rotatedSprite.texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            // generate a 180 rotated sprite
            rotatedSprite = RotatePiece(rotatedSprite);
            rotatedSprite.name = $"{down}{left}{up}{right}";
            while (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png")))
                filename = $"RotatedSprite({countCopy++})";
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png");
            bytes = rotatedSprite.texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            _puzzlePiecesExtended.Add(rotatedSprite);
            // generate a 270 rotated sprite
            rotatedSprite = RotatePiece(rotatedSprite);
            rotatedSprite.name = $"{right}{down}{left}{up}";
            while (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png")))
                filename = $"RotatedSprite({countCopy++})";
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png");
            bytes = rotatedSprite.texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            _puzzlePiecesExtended.Add(rotatedSprite);
            // generate a 360 rotated sprite
            filename = $"OriginalSprite";
            countCopy = 0;
            rotatedSprite = RotatePiece(rotatedSprite);
            rotatedSprite.name = $"{up}{right}{down}{left}";
            while (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png")))
                filename = $"OriginalSprite({countCopy++})";
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.png");
            bytes = rotatedSprite.texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            //_puzzlePiecesExtended.Add(rotatedSprite);
        }*/
        
        // Create the dictionary
        //foreach (var puzzlePiece in _puzzlePiecesExtended)
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
        //GenerateJigsawPuzzleMatrix(Configuration.ePuzzleHorizontalSize, Configuration.ePuzzleVerticalSize);
        GenerateJigsawPuzzleMatrix(5, 5);
    }

    private void SetBackgroundTransparency()
    {
        _imageBackground.GetComponent<Image>().color = new Color(1, 1, 1, Configuration.ePuzzleImageTransparency);
        _puzzleImage.GetComponent<Image>().color = new Color(1, 1, 1, Configuration.ePuzzleImageTransparency);
    }

    private void CreatePieces()
    {
        // create a new game object for each piece
        for (var i = 0; i < Configuration.ePuzzleHorizontalSize; i++)
        {
            for (var j = 0; j < Configuration.ePuzzleVerticalSize; j++)
            {
                var piece = Instantiate(_puzzlePiece, _canvas.transform);
                // Set the name of the piece
                piece.name = $"Piece {i} {j}";
                // Set the size of the piece
                piece.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    _imageSize.x / Configuration.ePuzzleHorizontalSize, 
                    _imageSize.y / Configuration.ePuzzleVerticalSize);
                // Resize the boxCollider
                piece.GetComponent<BoxColliderResizer>().ResizeBoxCollider();
                // Set the position of the piece
                piece.GetComponent<RectTransform>().localPosition = new Vector2(
                    _imageSize.x / Configuration.ePuzzleHorizontalSize * i,
                    -_imageSize.y / Configuration.ePuzzleVerticalSize * j);
                var localPosition = piece.transform.localPosition;
                localPosition = new Vector3(
                    localPosition.x+250-_imageSize.x/2, 
                    localPosition.y,
                    0);
                piece.transform.localPosition = localPosition;
                var pieceViewport = Instantiate(_imageViewportParent, _canvas.transform);
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
    
    public void ResizeImage()
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

    private Sprite RotatePiece(Sprite piece, int angle = 90, bool clockwise = true)
    {
        if (angle == 90)
        {
            var texture_ = new Texture2D((int)piece.rect.width, (int)piece.rect.height);
            var pixels_ = piece.texture.GetPixels((int)piece.rect.x, (int)piece.rect.y, (int)piece.rect.width, (int)piece.rect.height);
            for (var i = 0; i < texture_.width; i++)
            {
                for (var j = 0; j < texture_.height; j++)
                {
                    texture_.SetPixel(j, texture_.width - i - 1, pixels_[i * texture_.width + j]);
                }
            }
            texture_.Apply();
            var rotatedSprite_ = Sprite.Create(texture_, new Rect(0, 0, texture_.width, texture_.height), Vector2.zero);
            return rotatedSprite_;
        }
        
        if (clockwise)
        {
            Debug.Log($"Rotating piece {piece.name} by {angle} degrees clockwise");
        }
        else
        {
            Debug.Log($"Rotating piece {piece.name} by {angle} degrees counter-clockwise");
        }
        if (angle == 0)
        {
            return piece;
        }
        // Create a new texture
        var texture = new Texture2D((int)piece.rect.width, (int)piece.rect.height);
        // Get the pixels of the sprite
        var pixels = piece.texture.GetPixels((int)piece.rect.x, (int)piece.rect.y, (int)piece.rect.width, (int)piece.rect.height);
        // Rotate the pixels
        switch (angle)
        {
            case 90:
                if (clockwise)
                {
                    for (var i = 0; i < texture.width; i++)
                    {
                        for (var j = 0; j < texture.height; j++)
                        {
                            texture.SetPixel(i, j, pixels[(texture.height - 1 - j) * texture.width + i]);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < texture.width; i++)
                    {
                        for (var j = 0; j < texture.height; j++)
                        {
                            texture.SetPixel(i, j, pixels[j * texture.width + (texture.height - 1 - i)]);
                        }
                    }
                }
                break;
            case 180:
                for (var i = 0; i < texture.width; i++)
                {
                    for (var j = 0; j < texture.height; j++)
                    {
                        texture.SetPixel(i, j, pixels[(texture.width - 1 - i) + (texture.height - 1 - j) * texture.width]);
                    }
                }
                break;
            case 270:
                if (clockwise)
                {
                    for (var i = 0; i < texture.width; i++)
                    {
                        for (var j = 0; j < texture.height; j++)
                        {
                            texture.SetPixel(i, j, pixels[j * texture.width + (texture.height - 1 - i)]);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < texture.width; i++)
                    {
                        for (var j = 0; j < texture.height; j++)
                        {
                            texture.SetPixel(i, j, pixels[(texture.height - 1 - j) * texture.width + i]);
                        }
                    }
                }
                break;
        }
        // Apply the pixels to the texture
        texture.Apply();
        // Create a new sprite
        var rotatedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return rotatedSprite;
    }

    private Sprite FlipPieceVertically(Sprite piece)
    {
        // Create a new texture
        var texture = new Texture2D((int)piece.rect.width, (int)piece.rect.height);
        // Get the pixels of the sprite
        var pixels = piece.texture.GetPixels((int)piece.rect.x, (int)piece.rect.y, (int)piece.rect.width, (int)piece.rect.height);
        // Flip the pixels
        for (var i = 0; i < texture.width; i++)
        {
            for (var j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, pixels[(texture.width - 1 - i) + j * texture.width]);
            }
        }
        // Apply the pixels to the texture
        texture.Apply();
        // Create a new sprite
        var flippedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return flippedSprite;
    }
    
    private Sprite FlipPieceHorizontally(Sprite piece)
    {
        // Create a new texture
        var texture = new Texture2D((int)piece.rect.width, (int)piece.rect.height);
        // Get the pixels of the sprite
        var pixels = piece.texture.GetPixels((int)piece.rect.x, (int)piece.rect.y, (int)piece.rect.width, (int)piece.rect.height);
        // Flip the pixels
        for (var i = 0; i < texture.width; i++)
        {
            for (var j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, pixels[i + (texture.height - 1 - j) * texture.width]);
            }
        }
        // Apply the pixels to the texture
        texture.Apply();
        // Create a new sprite
        var flippedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return flippedSprite;
    }

    private Sprite GetPiece(string pieceCode)
    {
        Debug.Log($"Getting piece {pieceCode} -----------------------------");
        var up = pieceCode[0].ToString();
        var right = pieceCode[1].ToString();
        var down = pieceCode[2].ToString();
        var left = pieceCode[3].ToString();
        var possibleOrientations = new List<string>();
        possibleOrientations.Add(up + right + down + left);
        //possibleOrientations.Add(left + up + right + down);
        //possibleOrientations.Add(down + left + up + right);
        //possibleOrientations.Add(right + down + left + up);
        //Debug.Log($"Possible orientations: {possibleOrientations[0]}, {possibleOrientations[1]}, {possibleOrientations[2]}, {possibleOrientations[3]}");
        string storedOrientation = "";
        // find witch sprite stored in the dictionary
        foreach (var orientation in possibleOrientations)
        {
            if (puzzlePieces.ContainsKey(orientation))
            {
                var foundPiecee =  puzzlePieces[orientation];
                storedOrientation = orientation;
                Debug.Log($"piece found for {pieceCode} storedOrientation: {storedOrientation}");
                break;
            }
        }

        if (storedOrientation == "")
        {
            Debug.LogError($"No piece found for {pieceCode}");
            return GetPiece("0000");
        }
        // Check which rotation is needed to match the pieceCode
        var rotation = 0;
        if (storedOrientation != pieceCode)
        {
            if (storedOrientation == left + up + right + down)
            {
                Debug.Log($"Need rotation: {270} - {left}{up}{right}{down} - 90 anticlockwise");
                rotation = 270;
            }

            if (storedOrientation == down + left + up + right)
            {
                Debug.Log($"Need rotation: {180} - {down}{left}{up}{right}");
                rotation = 180;
            }

            if (storedOrientation == right + down + left + up)
            {
                Debug.Log($"Need rotation: {90} - {right}{down}{left}{up}");
                rotation = 90;
            }
        }
        Debug.Log($"End. Need rotation: {rotation}");
        // Rotate the sprite
        Sprite piece;
        //piece = RotatePiece(puzzlePieces[storedOrientation], rotation, rotation==270?false:true);
        piece = puzzlePieces[storedOrientation];
        Debug.Log($"End. Need rotation: {rotation}");
        return piece;
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
                var pieceName = "Piece " + j + " " + i;
                var pieceObject = GameObject.Find(pieceName);
                // Set the sprite of the piece
                pieceObject.GetComponent<Image>().sprite = piece;
            }
        }
        print(matrix);
    }

    private int checkPieceCount = 0;
    public void CheckPieces()
    {
        
    }
}
