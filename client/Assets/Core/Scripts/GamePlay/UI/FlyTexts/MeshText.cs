using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class UGUIFloatingTextBatch : MonoBehaviour
{
    public Font font ; // ����
    public Material fontMaterial; // ����Ĳ���
    public Color color = Color.white; // �ı���ɫ
    public float floatSpeed = 50f; // ����Ʈ���ٶ�
    public float fadeDuration = 10f; // ����ʱ��
    public int numberOfTexts = 1000; // ���ɵ��ı�����
    public int currentNum = 1;
    private Mesh mesh;
    private CanvasRenderer canvasRenderer;
    private float timer = 0;
    public List<string> texts = new List<string>();

    // ÿ���ַ��Ķ�����Ϊ
    private Vector3[] positions;
    private Color[] colors;
    private float[] alphas; // ���ڿ���ÿ���ַ���͸����


    int totalCharacters;
    Vector3[] vertices; // ÿ���ַ�4������
    Vector2[] uvs;
    int[] triangles; // ÿ���ַ�2��������
    Color[] meshColors;
    Vector3 org;
    void Start()
    {
        Debug.Log("����");
        currentNum = texts.Count;
        org = transform.position;
        // Ϊÿ�� "Hello" ����������Ϣ
        totalCharacters = 10 * numberOfTexts;
        vertices = new Vector3[totalCharacters * 4]; // ÿ���ַ�4������
        uvs = new Vector2[vertices.Length];
        triangles = new int[totalCharacters * 6]; // ÿ���ַ�2��������
        meshColors = new Color[vertices.Length];

        // ��ʼ��λ�ú���ɫ����
        positions = new Vector3[numberOfTexts];
        colors = new Color[numberOfTexts];
        alphas = new float[numberOfTexts];

        for (int i = 0; i < numberOfTexts; i++)
        {
            // �����ʼλ��
            positions[i] = new Vector3(Random.Range(-500, 500), Random.Range(-300, 300), 0);
            colors[i] = color; // ��ʼ����ɫΪȫ��
            alphas[i] = 1f; // ��ʼ͸����Ϊ1����͸����
        }

        // ��ȡCanvasRenderer���
        canvasRenderer = GetComponent<CanvasRenderer>();

        // ���ɳ�ʼ����������
        GenerateMesh();

        // ���ò��ʣ����û��ָ��������ʣ�ʹ�������Դ���Ĭ�ϲ���
        if (fontMaterial != null)
        {
            canvasRenderer.SetMaterial(fontMaterial, null);
        }
        else
        {
            fontMaterial = font.material;
            canvasRenderer.SetMaterial(fontMaterial, null);
        }
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        //���ʱ�䳬������ʱ������������
        if (timer >= fadeDuration)
        {
            transform.position = org;
            timer = 0;
        }

    }
    //�ػ�
    public void GenerateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        float xOffset = 0f;
        int vertexOffset = 0;
        int triangleOffset = 0;

        for (int i = 0; i < texts.Count; i++)
        {
            // Ϊÿ�� "Hello" ��������
            Vector3 startPosition = positions[i];

            for (int j = 0; j < texts[i].Length; j++)
            {
                char c = texts[i][j];
                font.RequestCharactersInTexture(c.ToString(), font.fontSize, FontStyle.Normal);
                font.GetCharacterInfo(c, out CharacterInfo characterInfo, font.fontSize);

                // ����ÿ���ַ��Ķ���
                float xMin = startPosition.x + xOffset + characterInfo.minX;
                float xMax = startPosition.x + xOffset + characterInfo.maxX;
                float yMin = startPosition.y + characterInfo.minY;
                float yMax = startPosition.y + characterInfo.maxY;

                vertices[vertexOffset] = new Vector3(xMin, yMin, 0);
                vertices[vertexOffset + 1] = new Vector3(xMin, yMax, 0);
                vertices[vertexOffset + 2] = new Vector3(xMax, yMax, 0);
                vertices[vertexOffset + 3] = new Vector3(xMax, yMin, 0);

                // ����UV����
                uvs[vertexOffset] = characterInfo.uvBottomLeft;
                uvs[vertexOffset + 1] = characterInfo.uvTopLeft;
                uvs[vertexOffset + 2] = characterInfo.uvTopRight;
                uvs[vertexOffset + 3] = characterInfo.uvBottomRight;

                // ���ó�ʼ��ɫ
                meshColors[vertexOffset] = colors[i];
                meshColors[vertexOffset + 1] = colors[i];
                meshColors[vertexOffset + 2] = colors[i];
                meshColors[vertexOffset + 3] = colors[i];

                // ����������
                triangles[triangleOffset] = vertexOffset;
                triangles[triangleOffset + 1] = vertexOffset + 1;
                triangles[triangleOffset + 2] = vertexOffset + 2;
                triangles[triangleOffset + 3] = vertexOffset;
                triangles[triangleOffset + 4] = vertexOffset + 2;
                triangles[triangleOffset + 5] = vertexOffset + 3;

                vertexOffset += 4;
                triangleOffset += 6;

                xOffset += characterInfo.advance; // ��һ���ַ���Xƫ����
            }

            // ���� xOffset����ʼ��һ�� "Hello"
            xOffset = 0f;
        }

        // �����ɵĶ��㡢UV���ꡢ�����κ���ɫ��ֵ������
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.colors = meshColors;

        // ʹ��CanvasRenderer����������ΪUIԪ��
        canvasRenderer.SetMesh(mesh);
    }
    

    //ˢ�£���ʱ����
    //void UpdateMesh()
    //{
    //    Vector3[] vertices = mesh.vertices;
    //    Color[] meshColors = mesh.colors;

    //    float xOffset = 0f;
    //    int vertexOffset = 0;

    //    for (int i = 0; i < numberOfTexts; i++)
    //    {
    //        Vector3 startPosition = positions[i];

    //        for (int j = 0; j < helloText.Length; j++)
    //        {
    //            font.GetCharacterInfo(helloText[j], out CharacterInfo characterInfo, font.fontSize);

    //            float xMin = startPosition.x + xOffset + characterInfo.minX;
    //            float xMax = startPosition.x + xOffset + characterInfo.maxX;
    //            float yMin = startPosition.y + characterInfo.minY;
    //            float yMax = startPosition.y + characterInfo.maxY;

    //            vertices[vertexOffset] = new Vector3(xMin, yMin, 0);
    //            vertices[vertexOffset + 1] = new Vector3(xMin, yMax, 0);
    //            vertices[vertexOffset + 2] = new Vector3(xMax, yMax, 0);
    //            vertices[vertexOffset + 3] = new Vector3(xMax, yMin, 0);

    //            // ������ɫ
    //            meshColors[vertexOffset] = colors[i];
    //            meshColors[vertexOffset + 1] = colors[i];
    //            meshColors[vertexOffset + 2] = colors[i];
    //            meshColors[vertexOffset + 3] = colors[i];

    //            vertexOffset += 4;
    //            xOffset += characterInfo.advance;
    //        }

    //        // ���� xOffset
    //        xOffset = 0f;
    //    }

    //    // ��������Ķ������ɫ
    //    mesh.vertices = vertices;
    //    mesh.colors = meshColors;

    //    // ����CanvasRenderer
    //    canvasRenderer.SetMesh(mesh);
    //}
}