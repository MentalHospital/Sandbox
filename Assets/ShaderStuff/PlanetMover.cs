using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Sandbox;

public class PlanetMover : MonoBehaviour
{
    [SerializeField] PlanetUIInput inputUI;
    const int planetStructSize = sizeof(float) * 10 + sizeof(int) * 2;
    [SerializeField] private int height = 10;
    [SerializeField] private int width = 50;
    [SerializeField] private int depth = 20;
    [SerializeField] private float distance = 10f;
    [SerializeField] private float G = .1f;
    [SerializeField] private float minStartSpeed = 5f;
    [SerializeField] private float maxStartSpeed = 10f;
    [SerializeField] private float minMass = 1f;
    [SerializeField] private float maxMass = 10f;
    [Space]
    [SerializeField] private float totalMass = 0f;
    [SerializeField] private float kinetic = 0f;
    [Space]
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private PlanetData[] planetStructData;
    [SerializeField] private GameObject[] gameObjects;
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private float deltaTime = 0;
    [SerializeField] private bool simulationActive = false;
    [SerializeField] private bool sphericalGeneration = true;
    [SerializeField] private float[] masses;

    private void Start()
    {
        inputUI.OnGenerateButtonClick +=
            () =>
            {
                ResetDimensions(inputUI.Width, inputUI.Height, inputUI.Depth);
                ResetPlanetProperties();
                RegeneratePlanets();
            };
        inputUI.OnClearButtonClick +=
            () =>
            {
                DestroyAllPlanets();
            };
        inputUI.OnSphericalToggleClick +=
            (bool b) =>
            {
                sphericalGeneration = b;
            };
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            simulationActive = !simulationActive;
        }
        if (simulationActive)
        {
            UpdatePlanets();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            File.Delete(Application.persistentDataPath + "/saves/save.mhs");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SavePlanets();
        }
    }

    private void SavePlanets()
    {
        FileStream fileStream = File.OpenWrite(Application.persistentDataPath + "/saves/save.mhs");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, planetStructData);
        fileStream.Close();
    }

    //private void OnApplicationQuit()
    //{
    //    SavePlanets();
    //}

    private void LoadPlanetsData()
    {
        gameObjects = new GameObject[height * width * depth];
        meshRenderers = new MeshRenderer[gameObjects.Length];
        masses = new float[gameObjects.Length];
        for (int i = 0; i < planetStructData.Length; i++)
        {
            var go = Instantiate(planetPrefab);
            go.transform.position = planetStructData[i].position;
            gameObjects[i] = go;
            masses[i] = planetStructData[i].mass;
            meshRenderers[i] = go.GetComponent<MeshRenderer>();
            go.SetActive(planetStructData[i].isEnabled == 1);
            meshRenderers[i].material.SetColor("_Color", new Color(planetStructData[i].color.x, planetStructData[i].color.y, planetStructData[i].color.z, 1f));
        }
    }

    public void LoadPlanets()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/saves/save.mhs"))
            {
                FileStream fileStream = File.OpenRead(Application.persistentDataPath + "/saves/save.mhs");
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                planetStructData = (PlanetData[])binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                LoadPlanetsData();
            }
            else
            {
                var s = File.Create(Application.persistentDataPath + "/saves/save.mhs");
                s.Close();
                GeneratePlanets();
            }
        }
        catch (System.Runtime.Serialization.SerializationException e)
        {
            Debug.LogError(e.Message);
            GeneratePlanets();
        }
    }

    public void RegeneratePlanets()
    {
        if (gameObjects.Length != 0)
        {
            DestroyAllPlanets();
        }
        GeneratePlanets();
    }

    private void ResetDimensions(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    private void ResetPlanetProperties()
    {
        minMass = inputUI.MinMass;
        maxMass = inputUI.MaxMass;
        G = inputUI.G;
        minStartSpeed = inputUI.MinStartSpeed;
        maxStartSpeed = inputUI.MaxStartSpeed;
    }

    private void GeneratePlanets()
    {
        gameObjects = new GameObject[height * width * depth];
        planetStructData = new PlanetData[gameObjects.Length];
        meshRenderers = new MeshRenderer[gameObjects.Length];
        masses = new float[gameObjects.Length];
        for (int i = 0; i < planetStructData.Length; i++)
        {
            CreatePlanet(i);
        }
    }

    public void DestroyAllPlanets()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
        simulationActive = false;
    }

    private void CreatePlanet(int index)
    {
        GameObject planet = Instantiate(planetPrefab);
        Vector3 center = (Vector3.forward * depth + Vector3.up * height + Vector3.right * width) * distance / 2;
        int x = (index % (width * height)) % width;
        int y = (index % (width * height)) / width;
        int z = index / (width * height);

        planet.transform.position = new Vector3(x, y, z) * distance - center;

        if (sphericalGeneration)
        {
            float maxDiag = Mathf.Max(
              Mathf.Abs(planet.transform.position.x),
              Mathf.Abs(planet.transform.position.y),
              Mathf.Abs(planet.transform.position.z)
              );

            planet.transform.position = planet.transform.position.normalized * maxDiag;
        }

        MeshRenderer meshRenderer = planet.GetComponent<MeshRenderer>();
        PlanetData planetData = new PlanetData();
        planetData.position = planet.transform.position;
        planetData.velocity = Random.Range(minStartSpeed, maxStartSpeed) * Random.onUnitSphere;
        planetData.mass = Random.Range(minMass, maxMass);
        totalMass += planetData.mass;
        planetData.isEnabled = 1;
        planetData.collided = 0;
        meshRenderer.material.SetColor("_Color", new Color((planetData.mass - minMass) / maxMass, 1 - (planetData.mass - minMass) / maxMass, 0f, 1));
        planetData.color = new Vector3(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b);
        //meshRenderer.material.SetColor("_Color", new Color(Random.value, Random.value, Random.value, 1));
        planetStructData[index] = planetData;
        gameObjects[index] = planet;
        meshRenderers[index] = meshRenderer;
    }

    private void UpdatePlanets()
    {
        int groupSize = 10;
        ComputeBuffer planetsBuffer = new ComputeBuffer(planetStructData.Length, planetStructSize);
        planetsBuffer.SetData(planetStructData);

        computeShader.SetBuffer(0, "planets", planetsBuffer);
        computeShader.SetFloat("dTime", deltaTime);
        computeShader.SetFloat("G", G);
        computeShader.SetInt("numPlanets", planetStructData.Length);
        computeShader.Dispatch(0, planetStructData.Length / groupSize, 1, 1);

        planetsBuffer.GetData(planetStructData);

        GameObject obj;
        PlanetData planetData;
        int c = 0;
        totalMass = 0;
        kinetic = 0;
        for (int i = 0; i < planetStructData.Length; i++)
        {
            obj = gameObjects[i];
            planetData = planetStructData[i];
            if (planetData.isEnabled == 1)
            {
                totalMass += planetData.mass;
                kinetic += planetData.mass * ((Vector3)planetData.velocity).sqrMagnitude * 0.5f;
                c++;
                obj.transform.position = planetData.position;
                meshRenderers[i].material.SetColor("_Color", new Color(planetData.color.x, planetData.color.y, planetData.color.z, 1f));
            }
            else
            {
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }
        //print(c);
        planetsBuffer.Release();
    }

}
