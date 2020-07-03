using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LevelMeshGenerator : MonoBehaviour
{
    [Header("Level from .map text file")]
    [SerializeField]
    protected TextAsset[] mapsText;

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    private ReaderMeshFile<char> reader;

    public virtual ReaderMeshFile<char> Reader
    {
        get
        {
            if (reader == null)
            {
                reader = new ReadMeshMap(mapsText);

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
        meshFilter = GetComponent<MeshFilter>();

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
           meshFilter.mesh = reader.Generate(parent: transform);
        }
    }
}
