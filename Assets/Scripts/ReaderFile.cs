using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReaderFile
{
    protected LevelElements levelElements;
    protected List<object> characters = new List<object>();
    protected (int width, int height) dimensions = default;
    protected bool valid = false;
    protected bool parsed = false;

    public bool IsParsed
    {
        get => parsed;
        protected set { }
    }

    public bool IsValid
    {
        get => valid;
    }

    public virtual void Generate(Transform parent)
    {
        if (!IsParsed)
        {
            try
            {
                Parse();
            }
            catch (NotImplementedException)
            {
                Debug.LogFormat("The class {0} file reader not contains a Parse() method implementation", GetType().Name);
            }
        }

        if (characters.Count == 0 || (dimensions.width == 0 && dimensions.height == 0))
        {
            return;
        }

        Vector3[] spawnPositions = new Vector3[characters.Count];

        Vector3 startingSpawnPosition = new Vector3(-Mathf.Round(dimensions.width / 2), 0 , -Mathf.Round(dimensions.height / 2));
        Vector3 currentSpawnPos = startingSpawnPosition;

        int count = 0;

        for (int z = 0; z < dimensions.height; z++)
        {
            for (int x = 0; x < dimensions.width; x++)
            {
                /**
                 * Avoid "Index out of bounds" if the .map/.txt file contains
                 * a bigger resolution (width and height) greater than
                 * letters/pixels
                 */
                if (count < spawnPositions.Length)
                {

                    spawnPositions[count] = currentSpawnPos;
                    count++;

                    currentSpawnPos.x++;
                }
            }

            currentSpawnPos.x = startingSpawnPosition.x;
            currentSpawnPos.z++;
        }

        AddElements(spawnPositions, parent);
    }

    public abstract bool ContainsFile
    {
        get;
    }

    /// <summary>
    /// Execute a file parse and transform into C#
    /// data structures like arrays or dictionaries 
    /// 
    /// <br/><br/>
    /// <b>PS:</b> Override this method is optional. 
    /// If your file type not needs execute data parsers/transformations
    /// not override them
    /// </summary>
    /// <returns></returns>
    public virtual ReaderFile Parse() {
        throw new System.NotImplementedException();
    }

    protected abstract void AddElements(Vector3[] spawnPositions, Transform parent);
}
