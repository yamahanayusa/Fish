using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject testObject_;
    [SerializeField] private GameObject baseObject_;
    private GameObject[] toulObjes_ = new GameObject[3];
    private ToulBase[] touls_ = new ToulBase[3];
    private ToulBase selectToul_;
    private int selectNum_ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < touls_.Length; i++)
        {
            toulObjes_[i] = Instantiate(baseObject_, transform);
            touls_[i] = toulObjes_[i].GetComponent<ToulBase>();
            toulObjes_[i].gameObject.SetActive(false);
        }
        selectToul_ = touls_[selectNum_];
        selectToul_.gameObject.SetActive(true);
        ChangeToul(1);
        AddToul(testObject_);
        ChangeToul(0);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTest();

        selectToul_.Use();
    }

    bool AddToul(GameObject obj)
    {
        if (touls_[selectNum_].GetToulType() == ToulType.Fist)
        {
            Destroy(toulObjes_[selectNum_]);
            toulObjes_[selectNum_] = Instantiate(obj, transform);
            touls_[selectNum_] = toulObjes_[selectNum_].GetComponent<ToulBase>();
            selectToul_ = toulObjes_[selectNum_].GetComponent<ToulBase>();
            if(toulObjes_[selectNum_].activeSelf == false)
            {
                toulObjes_[selectNum_].SetActive(true);
            }

            Debug.Log("成功");
            return true;
        }

        return false;
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

    void ChangeToul(int num)
    {
        if (touls_.Length <= num || num == selectNum_) return;
        selectToul_.ToulReset();

        selectToul_.gameObject.SetActive(false);

        selectToul_ = touls_[num];
        selectToul_.gameObject.SetActive(true);
        selectNum_ = num;
        Debug.Log("持ち物を");
        Debug.Log(num);
        Debug.Log("に変更");
    }
}
