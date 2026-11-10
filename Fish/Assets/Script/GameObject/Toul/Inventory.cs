using System;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject testObject_;
    [SerializeField] private GameObject handBaseObject_;
    private GameObject handObject_;
    private GameObject[] toulObjes_ = new GameObject[3];
    private ToulBase[] touls_ = new ToulBase[3];
    private ToulBase selectToul_;
    private int selectNum_ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handObject_ = Instantiate(handBaseObject_, transform);
        for (int i = 0; i < touls_.Length; i++)
        {
            toulObjes_[i] = handObject_;
            touls_[i] = toulObjes_[i].GetComponent<ToulBase>();
            toulObjes_[i].gameObject.SetActive(false);
        }
        selectToul_ = touls_[selectNum_];
        selectToul_.gameObject.SetActive(true);
        ChangeToul(1);
        NewToul(testObject_);
        ChangeToul(0);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTest();

        if (selectToul_.GetToulType() == ToulType.Fist)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 5.0f))
                {
                    if (hit.collider.gameObject.tag.Contains("Item"))
                    {
                        AddToul(hit.collider.gameObject);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ReleaseToul(selectToul_.gameObject);
            }
        }

        selectToul_.Use();
    }

    bool NewToul(GameObject obj)
    {
        if (touls_[selectNum_].GetToulType() == ToulType.Fist)
        {
            toulObjes_[selectNum_] = Instantiate(obj, transform);
            touls_[selectNum_] = toulObjes_[selectNum_].GetComponent<ToulBase>();
            selectToul_ = touls_[selectNum_];
            SetHand(selectToul_);

            if (toulObjes_[selectNum_].activeSelf == false)
            {
                toulObjes_[selectNum_].SetActive(true);
            }

            return true;
        }

        return false;
    }

    bool AddToul(GameObject obj)
    {
        if (touls_[selectNum_].GetToulType() == ToulType.Fist)
        {
            toulObjes_[selectNum_] = obj;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
            Collider[] cols = GetComponentsInChildren<Collider>();
            foreach (Collider col in cols)
            {
                col.enabled = false;
            }

            touls_[selectNum_] = toulObjes_[selectNum_].GetComponent<ToulBase>();
            toulObjes_[selectNum_].transform.parent = transform;
            selectToul_ = touls_[selectNum_];
            SetHand(selectToul_);

            Debug.Log("アイテム取得");
            return true;
        }
        return false;
    }

    void ReleaseToul(GameObject obj)
    {
        GameObject itemList = GameObject.Find("ItemList");
        obj.transform.parent = itemList.transform;

        toulObjes_[selectNum_] = handObject_;
        touls_[selectNum_] = handObject_.GetComponent<ToulBase>();
        selectToul_ = touls_[selectNum_];

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = true;
        }

        ReleaseHand(obj.GetComponent<ToulBase>());
    }

    void ChangeToul(int num)
    {
        if (touls_.Length <= num || num == selectNum_) return;
        selectToul_.ToulReset();
        ReleaseHand(selectToul_);
        selectToul_.gameObject.SetActive(false);

        selectToul_ = touls_[num];
        SetHand(selectToul_);
        selectToul_.gameObject.SetActive(true);
        selectNum_ = num;

        Debug.Log("持ち物を");
        Debug.Log(num);
        Debug.Log("に変更");
    }


    void SetHand(ToulBase obj)
    {
        Fists hand = handObject_.GetComponent<Fists>();
        if(obj.GetToulType() == ToulType.Fish)
        {
            obj.transform.parent = hand.GetRHandObject().transform;
        }
        else
        {
            hand.GetRHandObject().transform.parent = obj.GetRHand().transform;
            hand.GetRHandObject().transform.localPosition = Vector3.zero;
            hand.GetLHandObject().transform.parent = obj.GetLHand().transform;
            hand.GetLHandObject().transform.localPosition = Vector3.zero;
        }
    }

    void ReleaseHand(ToulBase obj)
    {
        Fists hand = handObject_.GetComponent<Fists>();

        if (obj.GetToulType() == ToulType.Fish)
        {
            obj.transform.parent = transform;
        }
        hand.GetRHandObject().transform.parent = hand.GetRHand().transform;
        hand.GetRHandObject().transform.localPosition = Vector3.zero;
        hand.GetLHandObject().transform.parent = hand.GetLHand().transform;
        hand.GetLHandObject().transform.localPosition = Vector3.zero;
    }

    void ChangeTest()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeToul(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeToul(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeToul(2);
        }

    }

}
