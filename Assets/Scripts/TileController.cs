using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    [SerializeField] SpriteRenderer tileImage;
    [SerializeField] GameObject highlightPrefab;
    [SerializeField] ParticleSystem starVFXPrefab;
    [SerializeField] AudioClip pressSFX;
    
    private GameManager gameManager;
    private Rigidbody rigidbody;
    private BoxCollider boxCollider;
    private AudioSource audioSource;

    public Sprite tileSprite;

    private bool isPressed = false;
    private bool isMoving = false;

    private float highlightY = 3;
    private float limitAngle_1 = 70;
    private float limitAngle_2 = 290;
    private float angleDiff = 10;
    private float offset = 0.2f;
    private float lerpTime = 5f;
    private float scaleValue = 0.6f;

    private Vector3 currentPosition;
    private Vector3 destinationPosition;
    private Vector3 scaleDestination;

    // Start is called before the first frame update
    void Start()
    {
        tileImage.sprite = tileSprite;

        rigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = GameObject.Find("Game_SFX").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed && !gameManager.isPausing)
        {
            highlightPrefab.SetActive(true);
            MoveHighLight();
        }
        else
        {
            highlightPrefab.SetActive(false);
        }

        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, destinationPosition, lerpTime * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, scaleDestination, lerpTime * Time.deltaTime);
        }
    }

    private bool CheckEuler_X()
    {
        float valueToCheck = Mathf.Abs(transform.rotation.eulerAngles.x);
        if (valueToCheck > limitAngle_1 && valueToCheck < limitAngle_2)
        {
            return false;
        }
        return true;
    }

    private bool CheckZEuler_Z()
    {
        float valueToCheck = Mathf.Abs(transform.rotation.eulerAngles.z);
        if (valueToCheck > limitAngle_1 - angleDiff && valueToCheck < limitAngle_2 + angleDiff)
        {
            return false;
        }
        return true;
    }

    private float FindNearestValue(float value)
    {
        float temp1 = Mathf.Abs(value - limitAngle_1);
        float temp2 = Mathf.Abs(value - limitAngle_2);
        float temp3 = Mathf.Abs(value + limitAngle_1);
        float temp4 = Mathf.Abs(value + limitAngle_2);
        float result = Mathf.Min(temp1, temp2, temp3, temp4);
        if(value < 0)
        {
            return -result;
        }
        return result;
    }

    private void FixedUpdate()
    {
        if (isPressed)
        {
            StopRigidbody();
            FocusOn();
        }
        else
        {
            if (!CheckEuler_X())
            {
                float minDistanceToRotate = FindNearestValue(transform.eulerAngles.x);

                if(minDistanceToRotate < 0)
                {
                    minDistanceToRotate = -90;
                }
                else
                {
                    minDistanceToRotate = 90;
                }

                Vector3 rotation = new Vector3(transform.eulerAngles.x + minDistanceToRotate, transform.eulerAngles.y, transform.eulerAngles.z);
                transform.Rotate(rotation);
            }
            if (!CheckZEuler_Z())
            {
                float minDistanceToRotate = FindNearestValue(transform.eulerAngles.z) - 10;

                if (minDistanceToRotate < 0)
                {
                    minDistanceToRotate = -90;
                }
                else
                {
                    minDistanceToRotate = 90;
                }

                Vector3 rotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + minDistanceToRotate);
                transform.Rotate(rotation);
            }
        }
    }

    private void OnMouseOver()
    {
        if (!gameManager.isPausing)
        {
            if (Input.GetMouseButton(0))
            {
                if (!isPressed)
                {
                    currentPosition = transform.position;

                    audioSource.PlayOneShot(pressSFX);
                }
                isPressed = true;
            }
            else
            {
                isPressed = false;

                if (Input.GetMouseButtonUp(0))
                {
                    gameManager.AddSlot(this);
                }
            }
        }
    }

    private void OnMouseExit()
    {
        isPressed = false;
    }

    private void StopRigidbody()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.Sleep();
    }

    private void FocusOn()
    {
        Vector3 position = new Vector3(currentPosition.x, currentPosition.y + offset, currentPosition.z);
        transform.position = position;

        Vector3 rotation = new Vector3(0, transform.eulerAngles.y, 0);
        transform.eulerAngles = rotation;
    }

    private void MoveHighLight()
    {
        Vector3 position = new Vector3(transform.position.x, highlightY, transform.position.z);
        highlightPrefab.transform.position = position;

        Vector3 rotation = new Vector3(0, transform.eulerAngles.y, 0);
        highlightPrefab.transform.eulerAngles = rotation;
    }

    public void MoveToPosition(Transform pos, bool moveToSlot)
    {
        isMoving = true;

        StartCoroutine(TurnOffRigidbodyMoment(moveToSlot));

        destinationPosition = new Vector3(pos.position.x, 0.2f, pos.position.z);
        scaleDestination = new Vector3(pos.localScale.x, pos.localScale.y, pos.localScale.z);
        transform.eulerAngles = pos.eulerAngles;

        if (moveToSlot)
        {
            scaleDestination = new Vector3(scaleValue, scaleValue, scaleValue);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    IEnumerator TurnOffRigidbodyMoment(bool moveToSlot)
    {
        StopRigidbody();

        boxCollider.enabled = false;
        rigidbody.useGravity = false;
        transform.eulerAngles = Vector3.zero;

        if (!moveToSlot)
        {
            yield return new WaitForSeconds(1);

            boxCollider.enabled = true;
            rigidbody.useGravity = true;
        }
    }

    public void ReturnTile()
    {
        GameObject temp = new GameObject();
        temp.transform.position = currentPosition;
        MoveToPosition(temp.transform, false);
    }

    public void Destroy()
    {
        ParticleSystem starVFX = Instantiate(starVFXPrefab, transform.position, transform.rotation);
        Destroy(starVFX, starVFX.duration);
        Destroy(gameObject);
    }
}
