using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Level from image(s)")]
    [SerializeField]
    protected Texture2D[] images;

    [Header("Level from .map text file")]
    [SerializeField]
    protected TextAsset[] mapsText;

    [SerializeField]
    protected LevelElements levelElements;

    private ReaderFile reader;

    public virtual ReaderFile Reader
    {
        get
        {
            if (reader == null)
            {
                reader = new ReadMap(levelElements, mapsText);
                if (!reader.ContainsFile)
                {
                    reader = new ReadImage(levelElements, images);
                }

                if (!reader.ContainsFile)
                {
                    return null;
                }
            }

            return reader;
        }
    }

    void Awake()
    {
        // This is called here for PERFORMANCE improvements
        reader = Reader;

        if (reader != null)
        {
            reader = reader.Parse();
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        if (reader != null)
        {
            reader.Generate(parent: transform);
        }
    }
}
