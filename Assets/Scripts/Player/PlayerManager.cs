using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Scripts")]
    public Movement _movement;
    public PlayerAttack _playerAttack;
    public QuestService _questService;

    [Header("Variables")]
    public bool IsInventoryOpen = false;
    public bool AllowMovement = true;
    public bool isTutorial = false;
    public bool IsHiding = false;
    public bool IsBugSprayActive = false;
    public bool IsCrouching = false;

    [Header("Components")]
    public GameObject Inventory;
    public Rigidbody2D _rigidBody;
    public SpriteAnimator _animator;
    public IntroductionService introductionService;
    public GameObject OrangeJuiceTimer;
    public GameObject BugSprayTimer;
    public GameObject BugSprayEffect;

    void Start()
    {
        _movement = GetComponent<Movement>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<SpriteAnimator>();
        _playerAttack = GetComponent<PlayerAttack>();
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();
        if(SceneManager.GetActiveScene().name == "Demo")
        {
            isTutorial = true;
            introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
            introductionService.StartTutorial();
        }
        AllowMovement = true;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            InventoryButton();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
    }

    public void Crouch()
    {
        if(IsCrouching == false)
        {
            IsCrouching = true;
            AllowMovement = false;
            _animator.Play("Crouching");
        }
        else
        {
            IsCrouching = false;
            AllowMovement = true;
        }
    }

    public void InventoryButton()
    {
        if(IsInventoryOpen == false)
        {
            IsInventoryOpen = true;
            Inventory.SetActive(IsInventoryOpen);
            _rigidBody.linearVelocityX = 0;
            _rigidBody.linearVelocityY = 0;
            _animator.Play("Idle");
            AllowMovement = false;
            if(isTutorial == true)
            {
                introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
                introductionService.InventoryTutorial();
            }
        }
        else
        {
            IsInventoryOpen = false;
            Inventory.SetActive(IsInventoryOpen);
            AllowMovement = true;
            if(isTutorial == true)
            {
                introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
                introductionService.ItemTutorial();
            }
        }
    }

    public void PickAxeUp()
    {
        _animator.Play("AquireAxe");
        _questService.SatisfyQuest("Axe");
    }

    public void BugSprayUsed()
    {
        IsBugSprayActive = true;
        BugSprayTimer.SetActive(true);
        BugSprayEffect.SetActive(true);
    }

    public void BugSprayEnded()
    {
        IsBugSprayActive = false;
        BugSprayTimer.SetActive(false);
        BugSprayEffect.SetActive(false);
    }
}
