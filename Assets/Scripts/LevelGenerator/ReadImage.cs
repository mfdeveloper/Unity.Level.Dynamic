using System.Collections.Generic;
using UnityEngine;

public class ReadImage : ReaderFile
{
    protected Texture2D img;
    protected Texture2D[] imageFiles;
    protected List<Color> unknownColors = new List<Color>();

    protected string levelsPath = "Levels/Images";

    public override bool ContainsFile 
    {
        get => imageFiles != null && imageFiles.Length > 0;  
    }

    public ReadImage(LevelElements levelElements, Texture2D[] images = null)
    {
        this.levelElements = levelElements;

        if (images != null)
        {
            if (images.Length == 0)
            {
                images = Resources.LoadAll<Texture2D>(levelsPath);
            }

            if (images.Length > 0)
            {

                if (images.Length > 1)
                {
                    img = images[Random.Range(0, images.Length)];
                }
                else
                {
                    img = images[0];
                }

                imageFiles = images;
            }
        }
    }

    public override ReaderFile Parse()
    {
        if (img != null)
        {

            dimensions = (img.width, img.height);

            /**
             * TODO: Refactor this to better PERFORMANCE.
             *       If the image contains many pixels,
             *       can be so slow to generate level
             *       
             * TODO: Verify why Color[] cannot
             *       be assigned to objec[] array
             */
            Color[] pixels = img.GetPixels();
            foreach (var pixel in pixels)
            {
                characters.Add(pixel);
            }
        }

        return this;
    }

    protected override void AddElements(Vector3[] spawnPositions, Transform parent)
    {
        int count = 0;

        foreach (Vector3 pos in spawnPositions)
        {

            if (characters[count] is Color)
            {
                Color? pixel = characters[count] as Color?;

                if (pixel.Equals(Color.white))
                {
                    if (levelElements.ground != null)
                    {
                        Object.Instantiate(levelElements.ground, pos, Quaternion.identity, parent);
                    }
                }
                else if (pixel.Equals(Color.black))
                {
                    if (levelElements.wall != null)
                    {
                        Object.Instantiate(levelElements.wall, pos, Quaternion.identity, parent);
                    }
                } else
                {
                    unknownColors.Add(pixel.GetValueOrDefault());

                    var hexaDecimal = ColorUtility.ToHtmlStringRGBA(pixel.GetValueOrDefault());
                    Debug.LogWarningFormat("The COLOR is unknown to parse => HEXADECIMAL: #{0}, RGB: {1}", hexaDecimal, pixel?.ToString());
                }

                count++;
            }
        }
    }

    public virtual void DebugGrayImage(Texture2D img)
    {
#if UNITY_EDITOR

        Color[] pixels = img.GetPixels();

        int black = 0, white = 0;

        foreach (var colorPixel in pixels)
        {
            if (colorPixel.Equals(Color.white))
            {
                white++;
            }
            else if (colorPixel.Equals(Color.black))
            {
                black++;
            }
        }

        if (black == 0 && white == 0)
        {
            Debug.LogWarningFormat("Maybe the image {0} is not in grayscale", img.name);
        }
        else
        {
            Debug.Log($"White = {white}");
            Debug.Log($"Black = {black}");
        }
#endif

    }
}
