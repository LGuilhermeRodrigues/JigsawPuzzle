using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
    
    // Start is called before the first frame update
    void Start()
    {
        ResizeImage();
        LoadImage();
        CreatePieces();
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
                // Set the position of the piece
                piece.GetComponent<RectTransform>().localPosition = new Vector2(
                    _imageSize.x / Configuration.ePuzzleHorizontalSize * i,
                    -_imageSize.y / Configuration.ePuzzleVerticalSize * j);
                // Transform the piece to be the same opsition of the image viewport
                /*piece.transform.Translate(
                    _imageViewport.transform.position);*/
                /*piece.transform.Translate(    - new Vector3(
                        166.5f,
                        125, 0));*/
                piece.transform.localPosition = new Vector3(
                    piece.transform.localPosition.x+250-_imageSize.x/2, 
                    piece.transform.localPosition.y,
                    0);
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
}
