using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ReadMap : ReaderFile
{
    protected TextAsset[] mapFiles;

    protected TextAsset textFile;

    protected string levelsPath = "Levels/Maps";

    protected List<string> lines;

    public override bool ContainsFile
    {
        get { return mapFiles != null && mapFiles.Length > 0; }
    }

    public ReadMap(LevelElements levelElements,  TextAsset[] maps)
    {
        this.levelElements = levelElements;

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
    }

    public override ReaderFile Parse()
    {
        if (textFile != null)
        {
            var content = textFile.text.Split(System.Environment.NewLine.ToCharArray());
            lines = new List<string>(content);

            if (lines.Count > 0)
            {

                foreach (var line in lines)
                {
                    if (Regex.IsMatch(line, "^type"))
                    {

                        string[] lineCols = Regex.Split(line, "\\s+");
                        if (lineCols[lineCols.Length - 1].Equals("octile"))
                        {
                            valid = true;
                        } else
                        {
                            valid = false;
                        }
                    }

                    if (line.StartsWith("height"))
                    {
                        string[] lineCols = Regex.Split(line, "\\s+");
                        var height = lineCols[lineCols.Length - 1];
                        dimensions.height = int.Parse(height);
                    }

                    if (line.StartsWith("width"))
                    {
                        string[] lineCols = Regex.Split(line, "\\s+");
                        var width = lineCols[lineCols.Length - 1];
                        dimensions.width = int.Parse(width);
                    }

                    if (line.StartsWith("T") || line.StartsWith("."))
                    {
                        foreach (var letter in line)
                        {
                            characters.Add(letter);
                        }
                    }
                }
            }

            if (IsValid && characters.Count > 0)
            {
                parsed = true;
            }
        }

        return this;
    }

    protected override void AddElements(Vector3[] spawnPositions, Transform parent)
    {
        int count = 0;

        foreach (Vector3 pos in spawnPositions)
        {
            if (characters[count] is char)
            {
                char? letter = characters[count] as char?;

                if (letter.Equals('.'))
                {
                    if (levelElements.ground != null)
                    {
                        Object.Instantiate(levelElements.ground, pos, Quaternion.identity, parent);
                    }
                }
                else if (letter.Equals('T'))
                {
                    if (levelElements.wall != null)
                    {
                        Object.Instantiate(levelElements.wall, pos, Quaternion.identity, parent);
                    }
                }

                count++;
            }

        }
    }
}
