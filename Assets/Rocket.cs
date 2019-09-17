using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float upThrust = 1000f;

    enum State { Alive, Transcending, Dying};
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float upThrustThisFrame = upThrust * Time.deltaTime;

            rigidBody.AddRelativeForce(Vector3.up * upThrustThisFrame);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; //We freeze rotation to take manual control of it

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-(Vector3.forward) * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; //Resume physics control on rotation
    }


    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive) { return; } //ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 2f);
                break;
            default:
                state = State.Dying;
                Invoke("RespawnScene", 1f);
                break;
        }
    }

    private void RespawnScene()
    {
        SceneManager.LoadScene(0);
    }

    private static void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
