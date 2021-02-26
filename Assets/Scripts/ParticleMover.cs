using UnityEngine;

public enum ParticleType
{
    empty,
    solid,
    powder,
    liquid,
    gas
}

public struct Particle
{
    public Vector2Int position;
    public ParticleType particleType;
}


public class ParticleMover : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader;
    [SerializeField] GameObject particlePrefab;
    Particle[] particlesData;
    GameObject[] particleObjects;
    bool working = false;
    int height = 16;
    int width = 16;

    void Start()
    {
        particlesData = new Particle[width * height];
        particleObjects = new GameObject[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                particlesData[y * width + x] = CreateParticleAtPosition(x, y);
            }
        }
    }

    Particle CreateParticleAtPosition(int x, int y)
    {
        var particle = new Particle();
        particle.position = new Vector2Int(x, y);
        GameObject obj = Instantiate(particlePrefab);
        obj.transform.position = new Vector3(x, y, 0);
        if (Random.value > 0.7f)
        {
            obj.SetActive(true);
            particle.particleType = ParticleType.powder;
        }
        else
        {
            obj.SetActive(false);
            particle.particleType = ParticleType.empty;
        }
        particleObjects[y * width + x] = obj;
        return particle;
    }

    void UpdateParticles()
    {
        Particle[] newParticlesData = new Particle[particlesData.Length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int particleIndex = x + y * width;
                Particle particle = particlesData[particleIndex];
                if (particle.particleType != ParticleType.empty)
                {
                    if (!CheckNeighbor(particle.position.x, particle.position.y, 0, -1))
                    {
                        particle.position = new Vector2Int (particle.position.x, particle.position.y - 1);
                        newParticlesData[(particle.position.y - 1) * width + particle.position.x] = particle;
                    }
                    else
                    {
                        if (!CheckNeighbor(particle.position.x, particle.position.y, 0, 1))
                        {
                            if (!CheckNeighbor(particle.position.x, particle.position.y, -1, -1))
                            {
                                particle.position = new Vector2Int (particle.position.x - 1, particle.position.y - 1);
                                newParticlesData[(particle.position.y - 1) * width + particle.position.x - 1] = particle;
                            }
                            else if (!CheckNeighbor(particle.position.x, particle.position.y, 1, -1))
                            {
                                particle.position = new Vector2Int (particle.position.x + 1, particle.position.y - 1);
                                newParticlesData[(particle.position.y - 1) * width + particle.position.x + 1] = particle;
                            }
                            else
                            {
                                newParticlesData[particleIndex] = particle;
                            }
                        }
                        else
                        {
                            newParticlesData[particleIndex] = particle;
                        }
                    }
                }
                else
                {
                    newParticlesData[particleIndex] = particle;
                }
            }
        }
        particlesData = newParticlesData;
    }

    bool CheckNeighbor(int x, int y, int dx, int dy)
    {
        int neighborIndex = (y + dy) * width + (x + dx);
        if (x + dx > 0 && x + dx < width - 1 && y + dy > 0 && y + dy < height - 1)
        {
            return (particlesData[neighborIndex].particleType != ParticleType.empty);
        }
        else if (x + dx <= 0 || x + dx >= width - 1 || y + dy <= 0)
        {
            return true;
        }
        else if (y + dy >= height - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //void UpdateParticles()
    //{
    //    int groupSize = 16;
    //    ComputeBuffer particlesBuffer = new ComputeBuffer(particlesData.Length, sizeof(int) * 3);
    //    ComputeBuffer newParticlesBuffer = new ComputeBuffer(particlesData.Length, sizeof(int) * 3);
    //    particlesBuffer.SetData(particlesData);
    //    newParticlesBuffer.SetData(particlesData);
    //    computeShader.SetBuffer(0, "particles", particlesBuffer);
    //    computeShader.SetBuffer(0, "newParticles", newParticlesBuffer);
    //    computeShader.Dispatch(0, particlesData.Length / groupSize, 1, 1);

    //    particlesBuffer.GetData(particlesData);
    //    //newParticlesBuffer.GetData(particlesData);

    //    GameObject obj;
    //    Particle particle;

    //    for (int i = 0; i < particlesData.Length; i++)
    //    {
    //        particle = particlesData[i];
    //        obj = particleObjects[i];
    //        obj.SetActive(particle.particleType != ParticleType.empty);
    //    }
    //    particlesBuffer.Release();
    //    newParticlesBuffer.Release();
    //}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateParticles();
            working = !working;
        }
        if (working)
        {
        }
    }
}
