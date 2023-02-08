using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float jumpHeight;
    public Transform groundCheck;
    bool isGrounded;
    Animator anim;
    public int curHp;
    int maxHp = 3;
    bool isHit = false;
    private Main main;
    public bool key = false;
    public bool canTP = true;
    public bool inWater = false;
    bool isClimbing = false;
    int coins = 0;
    bool canHit = true;
    public GameObject blueGem, greenGem;
    private Button Up;
    int gemCount = 0;
    float hitTimer = 0f;
    public Image PlayerCountdown;
    float insideTimer = -1f;
    public float insideTimerUp = 30f;
    public Image insideCountdown;
    private Inventory inventory;
    private SoundDefector soundeffector;
    public Joystick joystick;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Main>();
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        inventory = main.gameObject.GetComponent<Inventory>();
        soundeffector = main.gameObject.GetComponent<SoundDefector>();
        curHp = maxHp;
    }
    // Update is called once per frame
    void Update()
    {
            if (inWater && !isClimbing)
            {
                anim.SetInteger("State", 4);
                isGrounded = true;
                if (joystick.Horizontal >= 0.05f || joystick.Horizontal <= -0.05f)
                    Flip();
            }
            else
            {
                CheckGround();
                if (joystick.Horizontal < 0.05f && joystick.Horizontal > -0.05f && (isGrounded) && !isClimbing)
                {
                    anim.SetInteger("State", 1);
                }
                else
                {
                    Flip();
                    if (isGrounded && !isClimbing)
                        anim.SetInteger("State", 2);
                }
            }

            if (insideTimer >= 0f)
            {
                insideTimer += Time.deltaTime;
                if (insideTimer >= insideTimerUp)
                {
                    insideTimer = 0f;
                    RecountHp(-1);
                }
                else
                    insideCountdown.fillAmount = 1 - (insideTimer / insideTimerUp);
            }
            if (transform.position.y < -20f)
                RecountHp(-3);

    }
    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
            soundeffector.PlayJumpSound();


        }
    }
    void FixedUpdate()
    {
        if (joystick.Horizontal >= 0.2f)
            rb.velocity = new Vector2(speed, rb.velocity.y);
        else if (joystick.Horizontal <= -0.2f)
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        else
            rb.velocity = new Vector2(0f, rb.velocity.y);


    }
    void Flip()
    {
        if (joystick.Horizontal >= 0.05f)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (joystick.Horizontal <= 0.05f)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
    void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        isGrounded = colliders.Length > 1;
        if (!isGrounded && !isClimbing)
            anim.SetInteger("State", 3);
    }
    public void RecountHp(int deltaHp)
    {

        if (deltaHp < 0 && canHit)
        {
            soundeffector.PlayDamageSound();
            PlayerPrefs.SetInt("add", PlayerPrefs.GetInt("add") + 1);
            Debug.Log(PlayerPrefs.GetInt("add"));
            curHp = curHp + deltaHp;
            StopCoroutine(OnHit());
            canHit = false;
            isHit = true;
            StartCoroutine(OnHit());
        }
        else if (deltaHp > 0)
        {
            if (curHp >= maxHp)
                curHp = maxHp;
            else
                curHp = (curHp + deltaHp);
        }
        if (curHp <= 0)
        { 
            GetComponent<Animator>().SetBool("DEAD", true);
            Invoke("Lose", 1.5f);

            
        }
    }
    IEnumerator OnHit()
    {
        if (isHit)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g - 0.04f, GetComponent<SpriteRenderer>().color.b - 0.04f);
        }
        else
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g + 0.04f, GetComponent<SpriteRenderer>().color.b + 0.04f);
        if (GetComponent<SpriteRenderer>().color.g >= 1f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            canHit = true;
            StopCoroutine(OnHit());
        }

        if (GetComponent<SpriteRenderer>().color.g <= 0)
        {
            isHit = false;
            GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
        }
        yield return new WaitForSeconds(0.02f);
        StartCoroutine(OnHit());
    }
    void Lose()
    {
        main.GetComponent<Main>().Lose();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Key")
        {
            Destroy(collision.gameObject);
            key = true;
            inventory.Add_key();
        }
        if (collision.gameObject.tag == "Door")
        {
            if (collision.gameObject.GetComponent<Door>().isOpen && canTP)
            {
                collision.gameObject.GetComponent<Door>().Teleport(gameObject);
                canTP = false;
                StartCoroutine(TPwait());
            }
            else if (key)
                collision.gameObject.GetComponent<Door>().Unlock();

        }
        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            coins++;
            soundeffector.PlayCoinSound();
        }
        if (collision.gameObject.tag == "Heart")
        {
            Destroy(collision.gameObject);
            //RecountHp(1);
            inventory.Add_hp();
        }
        if (collision.gameObject.tag == "Mushroom")
        {
            Destroy(collision.gameObject);
            RecountHp(-1);
        }
        if (collision.gameObject.tag == "BlueGem")
        {
            Destroy(collision.gameObject);
            //StartCoroutine(NoHit());
            inventory.Add_bg();
        }
        if (collision.gameObject.tag == "GreenGem")
        {
            Destroy(collision.gameObject);
            //StartCoroutine(SpeedBonus());
            inventory.Add_gg();
        }

        if (collision.gameObject.tag == "TimerButtonStart")
        {
            insideTimer = 0f;
        }
        if (collision.gameObject.tag == "TimerButtonStop")
        {
            insideTimer = -1f;
            insideCountdown.fillAmount = 0f;
        }
    }
    IEnumerator TPwait()
    {
        yield return new WaitForSeconds(1f);
        canTP = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isClimbing = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (Input.GetAxis("Vertical") == 0)
            {
                anim.SetInteger("State", 5);
            }
            else
            {
                anim.SetInteger("State", 6);
                transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.5f * Time.deltaTime);
            }
        }
        if (collision.gameObject.tag == "Icy")
        {
            if (rb.gravityScale == 1f)
            {
                rb.gravityScale = 7f;
                speed *= 0.25f;
            }
        }

        if (collision.gameObject.tag == "Lava")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= 3f)
            {
                hitTimer = 0f;
                PlayerCountdown.fillAmount = 1f;
                RecountHp(-1);
            }
            else
                PlayerCountdown.fillAmount = 1 - (hitTimer / 3f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            isClimbing = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        if (collision.gameObject.tag == "Icy")
        {
            if (rb.gravityScale == 7f)
            { 
                rb.gravityScale = 1f;
                speed *= 4f;
            }
        }
        if (collision.gameObject.tag == "Lava")
        {
            hitTimer = 0f;
            PlayerCountdown.fillAmount = 0f;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trampoline")
            StartCoroutine(TrampolineAnim(collision.gameObject.GetComponentInParent<Animator>()));
        if (collision.gameObject.tag == "Quicksand")
        {
            speed *= 0.25f;
            rb.mass *= 100f;
        }

    }
    IEnumerator TrampolineAnim(Animator an)
    {
        an.SetBool("isjump", true);
        yield return new WaitForSeconds(0.5f);
        an.SetBool("isjump", false);
    }
    IEnumerator NoHit()
    {
        gemCount++;
        blueGem.SetActive(true);
        CheckGems(blueGem);

        canHit = false;
        blueGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        float t = 4f;
        while (t > 0f)
        {
            canHit = false;
            t -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine(Invis(blueGem.GetComponent<SpriteRenderer>(),0.02f));
        yield return new WaitForSeconds(1f);
        canHit = true;

        gemCount--;
        blueGem.SetActive(false);
        CheckGems(greenGem);
    }
    IEnumerator SpeedBonus()
    {
        gemCount++;
        greenGem.SetActive(true);
        CheckGems(greenGem);

        speed = speed * 2;
        greenGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(4f);
        StartCoroutine(Invis(greenGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        speed = speed / 2;

        gemCount--;
        greenGem.SetActive(false);
        CheckGems(blueGem);
        
    }
    void CheckGems(GameObject obj)
    {
        if (gemCount == 1)
        obj.transform.localPosition = new Vector3(0f, 0.6f, obj.transform.localPosition.z);
        else if (gemCount == 2)
        {
            blueGem.transform.localPosition = new Vector3(-0.5f, 0.5f, blueGem.transform.localPosition.z);
            greenGem.transform.localPosition = new Vector3(0.5f, 0.5f, greenGem.transform.localPosition.z);
        }
    }
    IEnumerator Invis(SpriteRenderer spr, float time)
    {
        spr.color = new Color(1f, 1f, 1f, spr.color.a - time * 2);
        yield return new WaitForSeconds(time);
        if (spr.color.a > 0)
        {
            StartCoroutine(Invis(spr, time));
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Quicksand")
        {
            speed *= 4f;
            rb.mass *= 0.01f;
        }
    }
    public int GetCoins()
    {
        return coins;
    }
    public int GetHP()
    {
        return curHp;
    }
    public void BlueGem()
    {
        StartCoroutine(NoHit());
    }
    public void GreenGem()
    {
        StartCoroutine(SpeedBonus());
    }
}