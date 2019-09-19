using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float upThrust = 1000f;

    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winJingle;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    enum State { Alive, Transcending, Dying};
    State state = State.Alive;

    bool godMode = false;

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
            RespondToThrustInput();
            ResopndToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
    }

    private void RespondToDebugInput()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            godMode = !godMode;
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            thrustParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float upThrustThisFrame = upThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * upThrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            thrustParticles.Play();
        }
    }

    private void ResopndToRotateInput()
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
        if(state != State.Alive || godMode) { return; } //ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //Do nothing
                break;
            case "Finish":
                StartWinSequence();
                break;
            default:
                StartDeathSeuence();
                break;
        }
    }

    private void StartDeathSeuence()
    {
        state = State.Dying;
        audioSource.Stop();
        thrustParticles.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("RespawnScene", levelLoadDelay);
    }

    private void StartWinSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        thrustParticles.Stop();
        audioSource.PlayOneShot(winJingle);
        winParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void RespawnScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
