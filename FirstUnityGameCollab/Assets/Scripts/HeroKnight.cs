using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float _speed = 4.0f;
    [SerializeField] float _jumpForce = 7.5f;
    [SerializeField] float _rollForce = 6.0f;
    [SerializeField] bool _noBlood = false;
    [SerializeField] GameObject _slideDust;
    [Header("Effects")]
    [SerializeField] GameObject _RunStopDust;
    [SerializeField] GameObject _JumpDust;
    [SerializeField] GameObject _LandingDust;

    private Animator _animator;
    private Rigidbody2D _body2d;
    private Sensor_HeroKnight _groundSensor;
    private Sensor_HeroKnight _wallSensorR1;
    private Sensor_HeroKnight _wallSensorR2;
    private Sensor_HeroKnight _wallSensorL1;
    private Sensor_HeroKnight _wallSensorL2;
    private AudioSource _audioSource;
    private AudioManager_PrototypeHero _audioManager;
    private bool _isWallSliding = false;
    private bool _grounded = false;
    private bool _rolling = false;
    private int _facingDirection = 1;
    private int _currentAttack = 0;
    private float _timeSinceAttack = 0.0f;
    private float _delayToIdle = 0.0f;
    private float _rollDuration = 8.0f / 14.0f;
    private float _rollCurrentTime;

    void Start ()
    {
        _animator = GetComponent<Animator>();
        _body2d = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager_PrototypeHero.instance;
        _groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        _wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        _wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        _wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        _wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        // Increase timer that controls attack combo
        _timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(_rolling)
            _rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(_rollCurrentTime > _rollDuration)
            _rolling = false;

        //Check if character just landed on the ground
        if (!_grounded && _groundSensor.State())
        {
            _grounded = true;
            _animator.SetBool("Grounded", _grounded);
            AE_Landing();
        }

        //Check if character just started falling
        if (_grounded && !_groundSensor.State())
        {
            _grounded = false;
            _animator.SetBool("Grounded", _grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            _facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            _facingDirection = -1;
        }

        // Move
        if (!_rolling )
            _body2d.velocity = new Vector2(inputX * _speed, _body2d.velocity.y);

        //Set AirSpeed in animator
        _animator.SetFloat("AirSpeedY", _body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        _isWallSliding = (_wallSensorR1.State() && _wallSensorR2.State()) || (_wallSensorL1.State() && _wallSensorL2.State());
        _animator.SetBool("WallSlide", _isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !_rolling)
        {
            _animator.SetBool("noBlood", _noBlood);
            _animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !_rolling)
            _animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0) && _timeSinceAttack > 0.25f && !_rolling)
        {
            _currentAttack++;

            // Loop back to one after third attack
            if (_currentAttack > 3)
                _currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (_timeSinceAttack > 1.0f)
                _currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            _animator.SetTrigger("Attack" + _currentAttack);

            // Reset timer
            _timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !_rolling)
        {
            _animator.SetTrigger("Block");
            _animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            _animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !_rolling && !_isWallSliding)
        {
            _rolling = true;
            _animator.SetTrigger("Roll");
            _body2d.velocity = new Vector2(_facingDirection * _rollForce, _body2d.velocity.y);
        }
            

        //Jump
        else if (Input.GetKeyDown("space") && _grounded && !_rolling)
        {
            _animator.SetTrigger("Jump");
            _grounded = false;
            _animator.SetBool("Grounded", _grounded);
            _body2d.velocity = new Vector2(_body2d.velocity.x, _jumpForce);
            _groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            _delayToIdle = 0.05f;
            _animator.SetInteger("AnimState", 1);
        }
        //Idle
        else
        {
            // Prevents flickering transitions to idle
            _delayToIdle -= Time.deltaTime;
                if(_delayToIdle < 0)
                    _animator.SetInteger("AnimState", 0);
        }
    }

    void SpawnDustEffect(GameObject dust, float dustXOffset = 0)
    {
        if (dust != null)
        {
            // Set dust spawn position
            Vector3 dustSpawnPosition = transform.position + new Vector3(dustXOffset * _facingDirection, 0.0f, 0.0f);
            GameObject newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity) as GameObject;
            // Turn dust in correct X direction
            newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(_facingDirection, 1, 1);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (_facingDirection == 1)
            spawnPosition = _wallSensorR2.transform.position;
        else
            spawnPosition = _wallSensorL2.transform.position;

        if (_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(_facingDirection, 1, 1);
        }
    }

    void AE_footstep()
    {
        _audioManager.PlaySound("Footstep");
    }

    void AE_Jump()
    {
        _audioManager.PlaySound("Jump");
        // Spawn Dust
        SpawnDustEffect(_JumpDust);
    }

    void AE_Landing()
    {
        _audioManager.PlaySound("Landing");
        // Spawn Dust
        SpawnDustEffect(_LandingDust);
    }

    void AE_Slash()
    {
        _audioManager.PlaySound("Slash");
    }

    void AE_Slide()
    {
        _audioManager.PlaySound("Slide");
    }

    void AE_Death()
    {
        _audioManager.PlaySound("Death");
    }

    void AE_Hurt()
    {
        _audioManager.PlaySound("Hurt");
    }
}
