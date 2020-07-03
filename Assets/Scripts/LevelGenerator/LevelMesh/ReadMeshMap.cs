using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ReadMeshMap : ReaderMeshFile<char>
{
    protected TextAsset[] mapFiles;
    protected TextAsset textFile;
    protected List<string> lines;
    protected List<List<char>> letters = new List<List<char>>();
    protected string levelsPath = "Levels/Maps";

    public override bool ContainsFile
    {
        get { return mapFiles != null && mapFiles.Length > 0; }
    }

    public ReadMeshMap(TextAsset[] maps)
    {

        if (maps.Length == 0)
        {
            maps = Resources.LoadAll<TextAsset>(levelsPath);
        }

        if (maps.Length > 0)
        {

            if (maps.Length > 1)
            {
                textFile = maps[Random.Range(0, maps.Length)];
            }
            else
            {
                textFile = maps[0];
            }

            mapFiles = maps;
        }

        base.OnGenerateUv += ValueUv;
    }

    ~ReadMeshMap()
    {
        base.OnGenerateUv -= ValueUv;
    }

    public virtual Vector2 ValueUv(CellMeshData cellData)
    {
        //Calculave UV for the texture
        char value = characters[cellData.cellPosition.x, cellData.cellPosition.y];
        int charNumberValue = 0;

        if (value.Equals('.'))
        {
            charNumberValue = 100;
        } else if (value.Equals('T'))
        {
            charNumberValue = 0;
        }

        float normalizedValue = (float) charNumberValue / TEXTURE_MAP_MAX_VALUE;
        Vector2 gridValueUv = new Vector2(normalizedValue, 0f);

        return gridValueUv;
    }

    public override ReaderMeshFile<char> Parse()
    {
        int charWidth = 0, charHeight = 0;
        if (textFile != null)
        {
            var content = textFile.text.Split(System.Environment.NewLine.ToCharArray());
            lines = new List<string>(content);

            if (lines.Count > 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (Regex.IsMatch(lines[i], "^type"))
                    {

                        string[] lineCols = Regex.Split(lines[i], "\\s+");
                        if (lineCols[lineCols.Length - 1].Equals("octile"))
                        {
                            valid = true;
                        }
                        else
                        {
                            valid = false;
                        }
                    }

                    if (lines[i].StartsWith("height"))
                    {
                        string[] lineCols = Regex.Split(lines[i], "\\s+");
                        var height = lineCols[lineCols.Length - 1];
                        dimensions.height = int.Parse(height);
                    }

                    if (lines[i].StartsWith("width"))
                    {
                        string[] lineCols = Regex.Split(lines[i], "\\s+");
                        var width = lineCols[lineCols.Length - 1];
                        dimensions.width = int.Parse(width);
                    }

                    if (dimensions.width > 0 && dimensions.height > 0)
                    {
                        if (characters == null)
                        {
                            characters = new char[dimensions.width, dimensions.height];
                        }
                    }

                    if (lines[i].StartsWith("T") || lines[i].StartsWith("."))
                    {

                        for (int j = 0; j < lines[i].Length; j++)
                        {
                            charWidth = j;
                            characters[charWidth, charHeight] = lines[i][j];
                        }

                        charHeight++;
                    }
                }
            }

            if (IsValid && characters.GetLength(0) > 0 && characters.GetLength(1) > 0)
            {
                parsed = true;
            }
        }

        return base.Parse();
    }
}
