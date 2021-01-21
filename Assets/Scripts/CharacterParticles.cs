using UnityEngine;
public enum Type
{
    HURT,
    FOOTSTEP
}
public class CharacterParticles : MonoBehaviour
{
    public GameObject hurtParticle;
    public GameObject footstepsParticle;

    public void PlayParticle(Type type)
    {
        GameObject instance;
        switch (type)
        {
            case Type.HURT:
                instance = Instantiate(hurtParticle, transform);
                Destroy(instance, 1f);
                break;
            case Type.FOOTSTEP:
                instance = Instantiate(footstepsParticle, transform);
                Destroy(instance, 1f);
                break;
        }
        
    }
}
