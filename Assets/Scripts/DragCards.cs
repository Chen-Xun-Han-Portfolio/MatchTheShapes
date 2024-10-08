using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragCards : MonoBehaviour
{
    public static DragCards instance;

    private Vector3 screenPoint;
    private Vector2 sizeCard;
    public int startPosNumOwn;
    [SerializeField] private GameObject holdCard;
    [SerializeField] private GameObject noHoldCard;
    public int holeShapesNum;

    private Vector3 dropPlacePosition;

    public bool isAtPlace = false;
    public bool isCorrectHole = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
       sizeCard = this.gameObject.GetComponent<BoxCollider2D>().size;
    }

    private void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
    }

    private void OnMouseDrag()
    {
        if (GameManager.instance.gameIsReady == true && GameManager.instance.unablePick == false)
        {
            //Debug.Log("can drag");
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            this.transform.position = curPosition;
            noHoldCard.gameObject.SetActive(false);
            holdCard.gameObject.SetActive(true);
            GetComponent<BoxCollider2D>().size = new Vector2(80f, 80f);
        }
    }

    private void OnMouseUp()
    {
        if (isAtPlace == true)
        {
            changeCards();

            if (isCorrectHole == true)
            {
                GameManager.instance.UpdateScore(true);
            }
            else
            {
                GameManager.instance.UpdateScore(false);
            }
        }

        this.gameObject.transform.position = GameManager.instance.startPos[startPosNumOwn].GetComponent<RectTransform>().position;
        noHoldCard.gameObject.SetActive(true);
        holdCard.gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().size = sizeCard;

        isAtPlace = false;
        isCorrectHole = false;


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("shapeHole") && other.gameObject == GameManager.instance.holeShapes[holeShapesNum])
        {
            Debug.Log("Correct");
            dropPlacePosition = other.GetComponent<RectTransform>().position;
            isAtPlace = true;
            isCorrectHole = true;
        }
        else if(other.gameObject.CompareTag("shapeHole") && other.gameObject != GameManager.instance.holeShapes[holeShapesNum])
        {
            Debug.Log("Wrong");
            dropPlacePosition = other.GetComponent<RectTransform>().position;
            isAtPlace = true;
            isCorrectHole = false;
        }
    }

    public void changeCards()
    {
        int randomCards = Random.Range(0, 3);
        //cardInHole();
        StartCoroutine("cardInHoleE");

        if (startPosNumOwn == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == randomCards)
                {
                    GameManager.instance.cardListChange1[randomCards].SetActive(true);
                    Debug.Log("Open");
                }
                else
                {
                    GameManager.instance.cardListChange1[i].SetActive(false);
                    Debug.Log("close");
                }
            }
        }
        else if (startPosNumOwn == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == randomCards)
                {
                    GameManager.instance.cardListChange2[randomCards].SetActive(true);
                }
                else
                {
                    GameManager.instance.cardListChange2[i].SetActive(false);
                }
            }
        }
        else if (startPosNumOwn == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == randomCards)
                {
                    GameManager.instance.cardListChange3[randomCards].SetActive(true);
                }
                else
                {
                    GameManager.instance.cardListChange3[i].SetActive(false);
                }
            }
        }
    }

    public void cardInHole()
    {
        StartCoroutine("cardInHoleE");
    }

    IEnumerator cardInHoleE()
    {
        GameManager.instance.holeShapes[holeShapesNum].GetComponent<RectTransform>().DOShakeAnchorPos(1, 20, 20, 90);
        yield return new WaitForSeconds(1);
        GameManager.instance.holeShapes[holeShapesNum].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
    }
}
