//using UnityEngine;

//public struct PlanetData
//{
//    public Vector3 position;
//    public Color color;
//}

//public class CubePositioner : MonoBehaviour
//{
//    const int cubeStructSize = sizeof(float) * 7;

//    [SerializeField] private GameObject cubePrefab;
//    [SerializeField] private ComputeShader computeShader;
//    [SerializeField] private int repetitions = 1000;
//    private PlanetData[] data;
//    private GameObject[] gameObjects;
//    private MeshRenderer[] meshRenderers;
//    private float timeOffset = 0;
//    private bool working = false;

//    void Start()
//    {
//        GenerateCubes();
//    }

//    void Update()
//    {
//        timeOffset += 5 * Time.deltaTime;
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            working = !working;
//        }
//        if (working)
//        {
//            RandomizeCubes();
//        }
//        if (Input.GetKeyDown(KeyCode.LeftShift))
//        {
//            float time = Time.time;
//            RandomizeCubesCPU();
//            print(Time.time - time);
//        }
//    }

//    void GenerateCubes()
//    {
//        gameObjects = new GameObject[100 * 50];
//        data = new PlanetData[gameObjects.Length];
//        meshRenderers = new MeshRenderer[gameObjects.Length];
//        for (int i = 0; i < data.Length; i++)
//        {
//            CreateCube(i);
//        }
//    }

//    void CreateCube(int index)
//    {
//        GameObject cube = Instantiate(cubePrefab);
//        cube.transform.position = new Vector3(index / 50, index % 50, 0);
//        MeshRenderer meshRenderer = cube.GetComponent<MeshRenderer>();
//        PlanetData cubeData = new PlanetData();
//        cubeData.position = cube.transform.position;
//        //meshRenderer.material.SetColor("_Color", new Color(Random.value, Random.value, Random.value, 1));
//        cubeData.color = meshRenderer.material.color;
//        data[index] = cubeData;
//        gameObjects[index] = cube;
//        meshRenderers[index] = meshRenderer;
//    }

//    void RandomizeCubes()
//    {
//        int groupSize = 10;
//        ComputeBuffer cubesBuffer = new ComputeBuffer(data.Length, cubeStructSize);

//        cubesBuffer.SetData(data);
//        computeShader.SetBuffer(0, "cubes", cubesBuffer);
//        computeShader.SetFloat("resolution", data.Length);
//        computeShader.SetFloat("time", timeOffset);
//        computeShader.SetFloat("repetitions", repetitions);
//        computeShader.Dispatch(0, data.Length / groupSize, 1, 1);

//        cubesBuffer.GetData(data);

//        GameObject obj;
//        MeshRenderer meshRenderer;
//        PlanetData cubeData;
//        for (int i = 0; i < data.Length; i++)
//        {
//            obj = gameObjects[i];
//            meshRenderer = meshRenderers[i];
//            cubeData = data[i];
//            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, cubeData.position.z);
//            meshRenderer.material.SetColor("_Color", cubeData.color);
//        }

//        cubesBuffer.Dispose();
//    }

//    void RandomizeCubesCPU()
//    {
//        float z = 0;
//        float r = 0;
//        float g = 0;
//        float b = 0;

//        GameObject obj;
//        MeshRenderer meshRenderer;
//        for (int i = 0; i < gameObjects.Length; i++)
//        {
//            for (int k = 0; k < repetitions; k++)
//            {
//                z = Random.value - 0.5f;
//                r = Random.value;
//                g = Random.value;
//                b = Random.value;
//            }
//            obj = gameObjects[i];
//            meshRenderer = meshRenderers[i];
//            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, z);
//            meshRenderer.material.SetColor("_Color", new Color(r, g, b, 1));
//        }
//    }
//}
